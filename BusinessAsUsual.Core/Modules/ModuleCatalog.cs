namespace BusinessAsUsual.Core.Modules
{
    /// <summary>
    /// Central catalog of all platform modules and submodules.
    /// IMPORTANT: Keep in sync with ModuleDiscoveryService.GetFallbackModules() in frontend shell.
    /// See docs/MODULE_CATALOG_UNIFIED_REFERENCE.md for maintenance protocol.
    /// </summary>
    public static class ModuleCatalog
    {
        public static IReadOnlyList<ModuleDefinition> AllModules => new List<ModuleDefinition>
        {
            // ============================================================
            // PLATFORM (System-level, cross-cutting)
            // ============================================================
            new(Guid.Parse("10000000-0000-0000-0000-000000000001"), "Platform", "UserManagement", "User Management", new []
            {
                new SubmoduleDefinition(Guid.Parse("10000000-0001-0000-0000-000000000001"), "Users", "Users"),
                new SubmoduleDefinition(Guid.Parse("10000000-0001-0000-0000-000000000002"), "Roles", "Roles"),
                new SubmoduleDefinition(Guid.Parse("10000000-0001-0000-0000-000000000003"), "Permissions", "Permissions")
            }),

            new(Guid.Parse("10000000-0000-0000-0000-000000000002"), "Platform", "AuditLogs", "Audit Logs", new []
            {
                new SubmoduleDefinition(Guid.Parse("10000000-0002-0000-0000-000000000001"), "SystemEvents", "System Events"),
                new SubmoduleDefinition(Guid.Parse("10000000-0002-0000-0000-000000000002"), "SecurityEvents", "Security Events")
            }),

            new(Guid.Parse("10000000-0000-0000-0000-000000000003"), "Platform", "Notifications", "Notifications", new []
            {
                new SubmoduleDefinition(Guid.Parse("10000000-0003-0000-0000-000000000001"), "Email", "Email"),
                new SubmoduleDefinition(Guid.Parse("10000000-0003-0000-0000-000000000002"), "SMS", "SMS"),
                new SubmoduleDefinition(Guid.Parse("10000000-0003-0000-0000-000000000003"), "Push", "Push Notifications")
            }),

            new(Guid.Parse("10000000-0000-0000-0000-000000000004"), "Platform", "Reporting", "Reporting & Analytics", new []
            {
                new SubmoduleDefinition(Guid.Parse("10000000-0004-0000-0000-000000000001"), "Dashboards", "Dashboards"),
                new SubmoduleDefinition(Guid.Parse("10000000-0004-0000-0000-000000000002"), "Exports", "Exports"),
                new SubmoduleDefinition(Guid.Parse("10000000-0004-0000-0000-000000000003"), "KPIs", "KPIs")
            }),

            new(Guid.Parse("10000000-0000-0000-0000-000000000005"), "Platform", "Integrations", "Integrations", new []
            {
                new SubmoduleDefinition(Guid.Parse("10000000-0005-0000-0000-000000000001"), "APIKeys", "API Keys"),
                new SubmoduleDefinition(Guid.Parse("10000000-0005-0000-0000-000000000002"), "Webhooks", "Webhooks"),
                new SubmoduleDefinition(Guid.Parse("10000000-0005-0000-0000-000000000003"), "Connectors", "Connectors")
            }),

            new(Guid.Parse("10000000-0000-0000-0000-000000000006"), "Platform", "Settings", "System Settings", new []
            {
                new SubmoduleDefinition(Guid.Parse("10000000-0006-0000-0000-000000000001"), "CompanyProfile", "Company Profile"),
                new SubmoduleDefinition(Guid.Parse("10000000-0006-0000-0000-000000000002"), "Preferences", "Preferences")
            }),

            new(Guid.Parse("10000000-0000-0000-0000-000000000007"), "Platform", "Localization", "Localization", new []
            {
                new SubmoduleDefinition(Guid.Parse("10000000-0007-0000-0000-000000000001"), "Languages", "Languages"),
                new SubmoduleDefinition(Guid.Parse("10000000-0007-0000-0000-000000000002"), "Regions", "Regions")
            }),

            // ============================================================
            // FINANCIAL
            // ============================================================
            new(Guid.Parse("20000000-0000-0000-0000-000000000001"), "Financial", "finance", "Finance", new []
            {
                new SubmoduleDefinition(Guid.Parse("20000000-0001-0000-0000-000000000001"), "AccountsReceivable", "Accounts Receivable"),
                new SubmoduleDefinition(Guid.Parse("20000000-0001-0000-0000-000000000002"), "AccountsPayable", "Accounts Payable"),
                new SubmoduleDefinition(Guid.Parse("20000000-0001-0000-0000-000000000003"), "GeneralLedger", "General Ledger"),
                new SubmoduleDefinition(Guid.Parse("20000000-0001-0000-0000-000000000004"), "Banking", "Banking"),
                new SubmoduleDefinition(Guid.Parse("20000000-0001-0000-0000-000000000005"), "Payments", "Payments"),
                new SubmoduleDefinition(Guid.Parse("20000000-0001-0000-0000-000000000006"), "Payroll", "Payroll"),
                new SubmoduleDefinition(Guid.Parse("20000000-0001-0000-0000-000000000007"), "Reports", "Reports")
            }),

            // ============================================================
            // SALES & CRM
            // ============================================================
            new(Guid.Parse("30000000-0000-0000-0000-000000000001"), "Sales", "crm", "CRM", new []
            {
                new SubmoduleDefinition(Guid.Parse("30000000-0001-0000-0000-000000000001"), "Leads", "Leads"),
                new SubmoduleDefinition(Guid.Parse("30000000-0001-0000-0000-000000000002"), "Opportunities", "Opportunities"),
                new SubmoduleDefinition(Guid.Parse("30000000-0001-0000-0000-000000000003"), "Customers", "Customers"),
                new SubmoduleDefinition(Guid.Parse("30000000-0001-0000-0000-000000000004"), "Activities", "Activities"),
                new SubmoduleDefinition(Guid.Parse("30000000-0001-0000-0000-000000000005"), "EmailTemplates", "Email Templates"),
                new SubmoduleDefinition(Guid.Parse("30000000-0001-0000-0000-000000000006"), "Reports", "Reports"),
                new SubmoduleDefinition(Guid.Parse("30000000-0001-0000-0000-000000000007"), "Settings", "Settings")
            }),

            new(Guid.Parse("30000000-0000-0000-0000-000000000002"), "Sales", "sales", "Sales", new []
            {
                new SubmoduleDefinition(Guid.Parse("30000000-0002-0000-0000-000000000001"), "Quotes", "Quotes"),
                new SubmoduleDefinition(Guid.Parse("30000000-0002-0000-0000-000000000002"), "Orders", "Orders"),
                new SubmoduleDefinition(Guid.Parse("30000000-0002-0000-0000-000000000003"), "Customers", "Customers"),
                new SubmoduleDefinition(Guid.Parse("30000000-0002-0000-0000-000000000004"), "Reports", "Reports")
            }),

            // ============================================================
            // OPERATIONS
            // ============================================================
            new(Guid.Parse("40000000-0000-0000-0000-000000000001"), "Operations", "inventory", "Inventory", new []
            {
                new SubmoduleDefinition(Guid.Parse("40000000-0001-0000-0000-000000000001"), "Products", "Products"),
                new SubmoduleDefinition(Guid.Parse("40000000-0001-0000-0000-000000000002"), "Warehouses", "Warehouses"),
                new SubmoduleDefinition(Guid.Parse("40000000-0001-0000-0000-000000000003"), "Stock", "Stock Management"),
                new SubmoduleDefinition(Guid.Parse("40000000-0001-0000-0000-000000000004"), "PurchaseOrders", "Purchase Orders"),
                new SubmoduleDefinition(Guid.Parse("40000000-0001-0000-0000-000000000005"), "Suppliers", "Suppliers"),
                new SubmoduleDefinition(Guid.Parse("40000000-0001-0000-0000-000000000006"), "Reports", "Reports")
            }),

            new(Guid.Parse("40000000-0000-0000-0000-000000000002"), "Operations", "services", "Services", new []
            {
                new SubmoduleDefinition(Guid.Parse("40000000-0002-0000-0000-000000000001"), "ServiceCatalog", "Service Catalog"),
                new SubmoduleDefinition(Guid.Parse("40000000-0002-0000-0000-000000000002"), "Providers", "Providers"),
                new SubmoduleDefinition(Guid.Parse("40000000-0002-0000-0000-000000000003"), "Appointments", "Appointments"),
                new SubmoduleDefinition(Guid.Parse("40000000-0002-0000-0000-000000000004"), "Reports", "Reports")
            }),

            // ============================================================
            // HR & PEOPLE
            // ============================================================
            new(Guid.Parse("50000000-0000-0000-0000-000000000001"), "HR", "hr", "Human Resources", new []
            {
                new SubmoduleDefinition(Guid.Parse("50000000-0001-0000-0000-000000000001"), "Employees", "Employee Management"),
                new SubmoduleDefinition(Guid.Parse("50000000-0001-0000-0000-000000000002"), "Departments", "Departments"),
                new SubmoduleDefinition(Guid.Parse("50000000-0001-0000-0000-000000000003"), "Recruiting", "Recruiting"),
                new SubmoduleDefinition(Guid.Parse("50000000-0001-0000-0000-000000000004"), "Performance", "Performance"),
                new SubmoduleDefinition(Guid.Parse("50000000-0001-0000-0000-000000000005"), "Training", "Training"),
                new SubmoduleDefinition(Guid.Parse("50000000-0001-0000-0000-000000000006"), "Timekeeping", "Timekeeping"),
                new SubmoduleDefinition(Guid.Parse("50000000-0001-0000-0000-000000000007"), "HRAdministration", "HR Administration"),
                new SubmoduleDefinition(Guid.Parse("50000000-0001-0000-0000-000000000008"), "Reports", "Reports")
            }),

            // ============================================================
            // DOCUMENTS & COMMUNICATION
            // ============================================================
            new(Guid.Parse("60000000-0000-0000-0000-000000000001"), "Documents", "Documents", "Document Management", new []
            {
                new SubmoduleDefinition(Guid.Parse("60000000-0001-0000-0000-000000000001"), "Storage", "Storage"),
                new SubmoduleDefinition(Guid.Parse("60000000-0001-0000-0000-000000000002"), "Sharing", "Sharing")
            }),

            new(Guid.Parse("60000000-0000-0000-0000-000000000002"), "Documents", "Messaging", "Messaging", new []
            {
                new SubmoduleDefinition(Guid.Parse("60000000-0002-0000-0000-000000000001"), "Conversations", "Conversations"),
                new SubmoduleDefinition(Guid.Parse("60000000-0002-0000-0000-000000000002"), "Channels", "Channels")
            }),

            new(Guid.Parse("60000000-0000-0000-0000-000000000003"), "Documents", "KnowledgeBase", "Knowledge Base", new []
            {
                new SubmoduleDefinition(Guid.Parse("60000000-0003-0000-0000-000000000001"), "Articles", "Articles"),
                new SubmoduleDefinition(Guid.Parse("60000000-0003-0000-0000-000000000002"), "Categories", "Categories")
            }),

            new(Guid.Parse("60000000-0000-0000-0000-000000000004"), "Documents", "FileStorage", "File Storage", new []
            {
                new SubmoduleDefinition(Guid.Parse("60000000-0004-0000-0000-000000000001"), "Uploads", "Uploads"),
                new SubmoduleDefinition(Guid.Parse("60000000-0004-0000-0000-000000000002"), "Folders", "Folders")
            }),

            // ============================================================
            // INDUSTRY-SPECIFIC
            // ============================================================
            new(Guid.Parse("70000000-0000-0000-0000-000000000001"), "Healthcare", "Patients", "Patients", new []
            {
                new SubmoduleDefinition(Guid.Parse("70000000-0001-0000-0000-000000000001"), "Records", "Records"),
                new SubmoduleDefinition(Guid.Parse("70000000-0001-0000-0000-000000000002"), "Visits", "Visits")
            }),

            new(Guid.Parse("70000000-0000-0000-0000-000000000002"), "Healthcare", "ClinicalNotes", "Clinical Notes", new []
            {
                new SubmoduleDefinition(Guid.Parse("70000000-0002-0000-0000-000000000001"), "SOAP", "SOAP Notes"),
                new SubmoduleDefinition(Guid.Parse("70000000-0002-0000-0000-000000000002"), "Charts", "Charts")
            }),

            new(Guid.Parse("80000000-0000-0000-0000-000000000001"), "Hospitality", "Reservations", "Reservations", new []
            {
                new SubmoduleDefinition(Guid.Parse("80000000-0001-0000-0000-000000000001"), "Bookings", "Bookings"),
                new SubmoduleDefinition(Guid.Parse("80000000-0001-0000-0000-000000000002"), "Calendar", "Calendar")
            }),

            new(Guid.Parse("80000000-0000-0000-0000-000000000002"), "Hospitality", "Events", "Events", new []
            {
                new SubmoduleDefinition(Guid.Parse("80000000-0002-0000-0000-000000000001"), "Planning", "Planning"),
                new SubmoduleDefinition(Guid.Parse("80000000-0002-0000-0000-000000000002"), "Staffing", "Staffing")
            }),

            new(Guid.Parse("90000000-0000-0000-0000-000000000001"), "Mining", "Safety", "Safety", new []
            {
                new SubmoduleDefinition(Guid.Parse("90000000-0001-0000-0000-000000000001"), "Incidents", "Incidents"),
                new SubmoduleDefinition(Guid.Parse("90000000-0001-0000-0000-000000000002"), "Training", "Training")
            }),

            new(Guid.Parse("A0000000-0000-0000-0000-000000000001"), "Logistics", "Dispatch", "Dispatch", new []
            {
                new SubmoduleDefinition(Guid.Parse("A0000000-0001-0000-0000-000000000001"), "Assignments", "Assignments"),
                new SubmoduleDefinition(Guid.Parse("A0000000-0001-0000-0000-000000000002"), "Tracking", "Tracking")
            }),

            new(Guid.Parse("B0000000-0000-0000-0000-000000000001"), "ProfessionalServices", "Contracts", "Contracts", new []
            {
                new SubmoduleDefinition(Guid.Parse("B0000000-0001-0000-0000-000000000001"), "Templates", "Templates"),
                new SubmoduleDefinition(Guid.Parse("B0000000-0001-0000-0000-000000000002"), "Approvals", "Approvals")
            }),

            new(Guid.Parse("B0000000-0000-0000-0000-000000000002"), "ProfessionalServices", "FieldService", "Field Service", new []
            {
                new SubmoduleDefinition(Guid.Parse("B0000000-0002-0000-0000-000000000001"), "Visits", "Visits"),
                new SubmoduleDefinition(Guid.Parse("B0000000-0002-0000-0000-000000000002"), "Reports", "Reports")
            })
        };
    }
}