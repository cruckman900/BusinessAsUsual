using Microsoft.EntityFrameworkCore;
using Platform.Application.DTOs.Import;
using Platform.Application.Interfaces;
using Platform.Domain.Entities;
using Platform.Infrastructure.Data;

namespace Platform.Infrastructure.Services;

public class MappingTemplateService : IMappingTemplateService
{
    private readonly PlatformDbContext _dbContext;

    public MappingTemplateService(PlatformDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<MappingTemplateDto>> GetTemplatesAsync(Guid companyId, string tableName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MappingTemplates
            .Where(t => t.CompanyId == companyId && t.TableName == tableName)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new MappingTemplateDto
            {
                Id = t.Id,
                CompanyId = t.CompanyId,
                TableName = t.TableName,
                TemplateName = t.TemplateName,
                Description = t.Description,
                IsShared = t.IsShared,
                CreatedBy = t.CreatedBy,
                CreatedAt = t.CreatedAt,
                UpdatedBy = t.UpdatedBy,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<MappingTemplateDetailDto?> GetTemplateByIdAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        var template = await _dbContext.MappingTemplates
            .Where(t => t.Id == templateId)
            .Select(t => new MappingTemplateDetailDto
            {
                Id = t.Id,
                CompanyId = t.CompanyId,
                TableName = t.TableName,
                TemplateName = t.TemplateName,
                Description = t.Description,
                ConfigurationJson = t.ConfigurationJson,
                IsShared = t.IsShared,
                CreatedBy = t.CreatedBy,
                CreatedAt = t.CreatedAt,
                UpdatedBy = t.UpdatedBy,
                UpdatedAt = t.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return template;
    }

    public async Task<Guid> SaveTemplateAsync(SaveMappingTemplateDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        if (dto.Id.HasValue)
        {
            // Update existing template
            var template = await _dbContext.MappingTemplates
                .FirstOrDefaultAsync(t => t.Id == dto.Id.Value && t.CompanyId == dto.CompanyId, cancellationToken);

            if (template == null)
                throw new InvalidOperationException($"Template {dto.Id} not found for company {dto.CompanyId}");

            template.TemplateName = dto.TemplateName;
            template.Description = dto.Description;
            template.ConfigurationJson = dto.ConfigurationJson;
            template.IsShared = dto.IsShared;
            template.UpdatedBy = userId;
            template.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return template.Id;
        }
        else
        {
            // Create new template
            var template = new MappingTemplate
            {
                Id = Guid.NewGuid(),
                CompanyId = dto.CompanyId,
                TableName = dto.TableName,
                TemplateName = dto.TemplateName,
                Description = dto.Description,
                ConfigurationJson = dto.ConfigurationJson,
                IsShared = dto.IsShared,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.MappingTemplates.Add(template);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return template.Id;
        }
    }

    public async Task<bool> DeleteTemplateAsync(Guid templateId, Guid companyId, CancellationToken cancellationToken = default)
    {
        var template = await _dbContext.MappingTemplates
            .FirstOrDefaultAsync(t => t.Id == templateId && t.CompanyId == companyId, cancellationToken);

        if (template == null)
            return false;

        _dbContext.MappingTemplates.Remove(template);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<MappingTemplateDto>> GetAccessibleTemplatesAsync(Guid companyId, Guid userId, string tableName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MappingTemplates
            .Where(t => t.CompanyId == companyId 
                     && t.TableName == tableName 
                     && (t.IsShared || t.CreatedBy == userId))
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new MappingTemplateDto
            {
                Id = t.Id,
                CompanyId = t.CompanyId,
                TableName = t.TableName,
                TemplateName = t.TemplateName,
                Description = t.Description,
                IsShared = t.IsShared,
                CreatedBy = t.CreatedBy,
                CreatedAt = t.CreatedAt,
                UpdatedBy = t.UpdatedBy,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
