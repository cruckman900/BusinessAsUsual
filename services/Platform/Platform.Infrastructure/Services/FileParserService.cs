using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Platform.Application.DTOs.Import;
using Platform.Application.Services;

namespace Platform.Infrastructure.Services;

/// <summary>
/// Implementation of file parsing for various formats
/// </summary>
public class FileParserService : IFileParserService
{
    private readonly ILogger<FileParserService> _logger;
    private const int MaxFileSizeMB = 50;
    private const int MaxRows = 100000; // Safety limit

    public FileParserService(ILogger<FileParserService> logger)
    {
        _logger = logger;
    }

    public ImportFileType DetectFileType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".csv" => ImportFileType.Csv,
            ".xlsx" or ".xls" => ImportFileType.Excel,
            ".txt" => ImportFileType.TabDelimited,
            ".tsv" => ImportFileType.TabDelimited,
            ".psv" => ImportFileType.PipeSeparated,
            _ => ImportFileType.Csv // Default to CSV
        };
    }

    public async Task<bool> ValidateFileAsync(Stream fileStream, string fileName)
    {
        try
        {
            // Check file size
            if (fileStream.Length > MaxFileSizeMB * 1024 * 1024)
            {
                _logger.LogWarning("File {FileName} exceeds maximum size of {MaxSize}MB", fileName, MaxFileSizeMB);
                return false;
            }

            // Check if it's a known file type
            var fileType = DetectFileType(fileName);

            // Try to read first few bytes to ensure it's not corrupted
            var buffer = new byte[100];
            var bytesRead = await fileStream.ReadAsync(buffer);
            fileStream.Position = 0; // Reset position

            return bytesRead > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating file {FileName}", fileName);
            return false;
        }
    }

    public async Task<ParsedData> ParseFileAsync(Stream fileStream, string fileName, ImportFileType fileType)
    {
        try
        {
            if (!await ValidateFileAsync(fileStream, fileName))
            {
                return new ParsedData
                {
                    FileName = fileName,
                    Errors = new List<string> { "File validation failed" }
                };
            }

            return fileType switch
            {
                ImportFileType.Excel => await ParseExcelAsync(fileStream, fileName),
                ImportFileType.Csv => await ParseCsvAsync(fileStream, fileName, ','),
                ImportFileType.TabDelimited => await ParseCsvAsync(fileStream, fileName, '\t'),
                ImportFileType.PipeSeparated => await ParseCsvAsync(fileStream, fileName, '|'),
                _ => throw new ArgumentException($"Unsupported file type: {fileType}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing file {FileName}", fileName);
            return new ParsedData
            {
                FileName = fileName,
                Errors = new List<string> { $"Parse error: {ex.Message}" }
            };
        }
    }

    public async Task<ParsedData> ParseTextAsync(string content, ImportFileType fileType)
    {
        try
        {
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return fileType switch
            {
                ImportFileType.Csv => await ParseCsvAsync(memoryStream, "paste.csv", ','),
                ImportFileType.TabDelimited => await ParseCsvAsync(memoryStream, "paste.txt", '\t'),
                ImportFileType.PipeSeparated => await ParseCsvAsync(memoryStream, "paste.psv", '|'),
                _ => throw new ArgumentException($"Text parsing not supported for type: {fileType}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing text content");
            return new ParsedData
            {
                Errors = new List<string> { $"Parse error: {ex.Message}" }
            };
        }
    }

    private async Task<ParsedData> ParseCsvAsync(Stream stream, string fileName, char delimiter)
    {
        var result = new ParsedData { FileName = fileName };

        try
        {
            using var reader = new StreamReader(stream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter.ToString(),
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null, // Don't throw on missing fields
                BadDataFound = context =>
                {
                    _logger.LogWarning("Bad data at row {Row}, field {Field}: {RawRecord}", 
                        context.Context.Parser.Row, context.Field, context.RawRecord);
                    result.Errors.Add($"Bad data at row {context.Context.Parser.Row}: {context.RawRecord}");
                }
            };

            using var csv = new CsvReader(reader, config);

            // Read header
            await csv.ReadAsync();
            csv.ReadHeader();
            result.Headers = csv.HeaderRecord?.ToList() ?? new List<string>();

            _logger.LogInformation("CSV headers found in {FileName}: {Headers}", 
                fileName, string.Join(", ", result.Headers));

            if (result.Headers.Count == 0)
            {
                result.Errors.Add("No headers found in file");
                _logger.LogWarning("No headers found in CSV file {FileName}", fileName);
                return result;
            }

            // Log any duplicate headers
            var duplicates = result.Headers.GroupBy(h => h).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicates.Any())
            {
                _logger.LogWarning("Duplicate headers found in {FileName}: {Duplicates}", 
                    fileName, string.Join(", ", duplicates));
                result.Errors.Add($"Duplicate headers found: {string.Join(", ", duplicates)}");
            }

            // Read rows
            int rowNumber = 1;
            while (await csv.ReadAsync())
            {
                if (result.Rows.Count >= MaxRows)
                {
                    result.Errors.Add($"Maximum row limit ({MaxRows}) reached. File truncated.");
                    _logger.LogWarning("Maximum row limit ({MaxRows}) reached for {FileName}", MaxRows, fileName);
                    break;
                }

                var row = new Dictionary<string, string>();
                foreach (var header in result.Headers)
                {
                    try
                    {
                        var value = csv.GetField(header) ?? string.Empty;
                        row[header] = value;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error reading field '{Header}' at row {RowNumber} in {FileName}", 
                            header, rowNumber, fileName);
                        row[header] = string.Empty;
                    }
                }
                result.Rows.Add(row);
                rowNumber++;
            }

            _logger.LogInformation("Parsed CSV file {FileName}: {RowCount} rows, {ColumnCount} columns",
                fileName, result.RowCount, result.Headers.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing CSV file {FileName}", fileName);
            result.Errors.Add($"CSV parse error: {ex.Message}");
        }

        return result;
    }

    private async Task<ParsedData> ParseExcelAsync(Stream stream, string fileName)
    {
        var result = new ParsedData { FileName = fileName };

        try
        {
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1); // Use first worksheet

            if (worksheet == null)
            {
                result.Errors.Add("No worksheets found in Excel file");
                return result;
            }

            var firstRow = worksheet.FirstRowUsed();
            if (firstRow == null)
            {
                result.Errors.Add("Excel file is empty");
                return result;
            }

            // Read headers from first row
            foreach (var cell in firstRow.CellsUsed())
            {
                result.Headers.Add(cell.GetString());
            }

            if (result.Headers.Count == 0)
            {
                result.Errors.Add("No headers found in Excel file");
                return result;
            }

            // Read data rows
            var rows = worksheet.RowsUsed().Skip(1); // Skip header row
            foreach (var row in rows)
            {
                if (result.Rows.Count >= MaxRows)
                {
                    result.Errors.Add($"Maximum row limit ({MaxRows}) reached. File truncated.");
                    break;
                }

                var rowData = new Dictionary<string, string>();
                for (int i = 0; i < result.Headers.Count; i++)
                {
                    var cellIndex = i + 1; // Excel is 1-based
                    var cell = row.Cell(cellIndex);
                    var value = cell.IsEmpty() ? string.Empty : cell.GetString();
                    rowData[result.Headers[i]] = value;
                }
                result.Rows.Add(rowData);
            }

            _logger.LogInformation("Parsed Excel file {FileName}: {RowCount} rows, {ColumnCount} columns",
                fileName, result.RowCount, result.Headers.Count);

            await Task.CompletedTask; // To satisfy async signature
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Excel file {FileName}", fileName);
            result.Errors.Add($"Excel parse error: {ex.Message}");
        }

        return result;
    }
}
