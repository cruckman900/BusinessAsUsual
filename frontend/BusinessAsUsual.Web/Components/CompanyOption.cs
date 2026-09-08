namespace BusinessAsUsual.Web.Components;

/// <summary>
/// Represents a selectable company/tenant option, used by the <c>CompanyPicker</c> component
/// and the login portal.
/// </summary>
/// <param name="CompanyId">Unique identifier of the company.</param>
/// <param name="Name">Display name of the company.</param>
/// <param name="DbName">Tenant database name associated with the company.</param>
public record CompanyOption(Guid CompanyId, string Name, string DbName);
