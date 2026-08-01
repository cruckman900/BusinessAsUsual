namespace Finance.Application.DTOs;

/// <summary>Employee wage configuration.</summary>
public sealed class EmployeeWageDto
{
    public required string EmployeeId { get; set; }
    public decimal HourlyRate { get; set; }
}

/// <summary>Global payroll tax and deduction rates.</summary>
public sealed class PayrollRatesDto
{
    /// <summary>Default hourly rate for employees without a specific rate.</summary>
    public decimal DefaultHourlyRate { get; set; }

    /// <summary>Flat tax rate (e.g. 0.20 = 20%).</summary>
    public decimal TaxRate { get; set; }

    /// <summary>Flat other-deduction rate (e.g. 0.05 = 5% for benefits).</summary>
    public decimal DeductionRate { get; set; }
}

/// <summary>Request to update an employee's hourly rate.</summary>
public sealed class UpdateEmployeeWageRequest
{
    public required string EmployeeId { get; set; }
    public decimal HourlyRate { get; set; }
}

/// <summary>Request to update global tax/deduction rates.</summary>
public sealed class UpdatePayrollRatesRequest
{
    public decimal? DefaultHourlyRate { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? DeductionRate { get; set; }
}
