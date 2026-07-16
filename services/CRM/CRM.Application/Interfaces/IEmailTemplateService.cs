using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

public interface IEmailTemplateService
{
    Task<IEnumerable<EmailTemplateDto>> GetAllTemplatesAsync();
    Task<IEnumerable<EmailTemplateDto>> GetTemplatesByCategoryAsync(string category);
    Task<EmailTemplateDto?> GetTemplateByIdAsync(string id);
    Task<IEnumerable<string>> GetCategoriesAsync();
    Task<EmailTemplateDto> CreateTemplateAsync(CreateEmailTemplateRequest request);
    Task<EmailTemplateDto> UpdateTemplateAsync(string id, UpdateEmailTemplateRequest request);
    Task DeleteTemplateAsync(string id);

    /// <summary>
    /// Resolve {{Token}} merge fields in the supplied text using the provided values.
    /// Unknown tokens are left untouched so authors can spot typos.
    /// </summary>
    string ApplyMergeFields(string text, IReadOnlyDictionary<string, string?> values);
}
