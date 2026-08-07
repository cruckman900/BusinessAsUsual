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
            new("Platform", "UserManagement", "User Management", new []
            {
                new SubmoduleDefinition("Users", "Users"),
                new SubmoduleDefinition("Roles", "Roles"),
                new SubmoduleDefinition("Permissions", "Permissions")
            }),

            new("Platform", "AuditLogs", "Audit Logs", new []
            {
                new SubmoduleDefinition("SystemEvents", "System Events"),
                new SubmoduleDefinition("SecurityEvents", "Security Events")
            }),

            new("Platform", "Notifications", "Notifications", new []
            {
                new SubmoduleDefinition("Email", "Email"),
                new SubmoduleDefinition("SMS", "SMS"),
                new SubmoduleDefinition("Push", "Push Notifications")
            }),

            new("Platform", "Reporting", "Reporting & Analytics", new []
            {
                new SubmoduleDefinition("Dashboards", "Dashboards"),
                new SubmoduleDefinition("Exports", "Exports"),
                new SubmoduleDefinition("KPIs", "KPIs")
            }),

            new("Platform", "Integrations", "Integrations", new []
            {
                new SubmoduleDefinition("APIKeys", "API Keys"),
                new SubmoduleDefinition("Webhooks", "Webhooks"),
                new SubmoduleDefinition("Connectors", "Connectors")
            }),

            new("Platform", "Settings", "System Settings", new []
            {
                new SubmoduleDefinition("CompanyProfile", "Company Profile"),
                new SubmoduleDefinition("Preferences", "Preferences")
            }),

            new("Platform", "Localization", "Localization", new []
            {
                new SubmoduleDefinition("Languages", "Languages"),
                new SubmoduleDefinition("Regions", "Regions")
            }),

            // ============================================================
            // FINANCIAL
            // ============================================================
            new("Financial", "finance", "Finance", new []
            {
                new SubmoduleDefinition("AccountsReceivable", "Accounts Receivable"),
                new SubmoduleDefinition("AccountsPayable", "Accounts Payable"),
                new SubmoduleDefinition("GeneralLedger", "General Ledger"),
                new SubmoduleDefinition("Banking", "Banking"),
                new SubmoduleDefinition("Payments", "Payments"),
                new SubmoduleDefinition("Payroll", "Payroll"),
                new SubmoduleDefinition("Reports", "Reports")
            }),

            // ============================================================
            // SALES & CRM
            // ============================================================
            new("Sales", "crm", "CRM", new []
            {
                new SubmoduleDefinition("Leads", "Leads"),
                new SubmoduleDefinition("Opportunities", "Opportunities"),
                new SubmoduleDefinition("Customers", "Customers"),
                new SubmoduleDefinition("Activities", "Activities"),
                new SubmoduleDefinition("EmailTemplates", "Email Templates"),
                new SubmoduleDefinition("Reports", "Reports"),
                new SubmoduleDefinition("Settings", "Settings")
            }),

            new("Sales", "sales", "Sales", new []
            {
                new SubmoduleDefinition("Quotes", "Quotes"),
                new SubmoduleDefinition("Orders", "Orders"),
                new SubmoduleDefinition("Customers", "Customers"),
                new SubmoduleDefinition("Reports", "Reports")
            }),

            // ============================================================
            // OPERATIONS
            // ============================================================
            new("Operations", "inventory", "Inventory", new []
            {
                new SubmoduleDefinition("Products", "Products"),
                new SubmoduleDefinition("Warehouses", "Warehouses"),
                new SubmoduleDefinition("Stock", "Stock Management"),
                new SubmoduleDefinition("PurchaseOrders", "Purchase Orders"),
                new SubmoduleDefinition("Suppliers", "Suppliers"),
                new SubmoduleDefinition("Reports", "Reports")
            }),

            new("Operations", "services", "Services", new []
            {
                new SubmoduleDefinition("ServiceCatalog", "Service Catalog"),
                new SubmoduleDefinition("Providers", "Providers"),
                new SubmoduleDefinition("Appointments", "Appointments"),
                new SubmoduleDefinition("Reports", "Reports")
            }),

            // ============================================================
            // HR & PEOPLE
            // ============================================================
            new("HR", "hr", "Human Resources", new []
            {
                new SubmoduleDefinition("Employees", "Employee Management"),
                new SubmoduleDefinition("Departments", "Departments"),
                new SubmoduleDefinition("Recruiting", "Recruiting"),
                new SubmoduleDefinition("Performance", "Performance"),
                new SubmoduleDefinition("Training", "Training"),
                new SubmoduleDefinition("Timekeeping", "Timekeeping"),
                new SubmoduleDefinition("HRAdministration", "HR Administration"),
                new SubmoduleDefinition("Reports", "Reports")
            }),

            // ============================================================
            // DOCUMENTS & COMMUNICATION
            // ============================================================
            new("Documents", "Documents", "Document Management", new []
            {
                new SubmoduleDefinition("Storage", "Storage"),
                new SubmoduleDefinition("Sharing", "Sharing")
            }),

            new("Documents", "Messaging", "Messaging", new []
            {
                new SubmoduleDefinition("Conversations", "Conversations"),
                new SubmoduleDefinition("Channels", "Channels")
            }),

            new("Documents", "KnowledgeBase", "Knowledge Base", new []
            {
                new SubmoduleDefinition("Articles", "Articles"),
                new SubmoduleDefinition("Categories", "Categories")
            }),

            new("Documents", "FileStorage", "File Storage", new []
            {
                new SubmoduleDefinition("Uploads", "Uploads"),
                new SubmoduleDefinition("Folders", "Folders")
            }),

            // ============================================================
            // INDUSTRY-SPECIFIC
            // ============================================================
            new("Healthcare", "Patients", "Patients", new []
            {
                new SubmoduleDefinition("Records", "Records"),
                new SubmoduleDefinition("Visits", "Visits")
            }),

            new("Healthcare", "ClinicalNotes", "Clinical Notes", new []
            {
                new SubmoduleDefinition("SOAP", "SOAP Notes"),
                new SubmoduleDefinition("Charts", "Charts")
            }),

            new("Hospitality", "Reservations", "Reservations", new []
            {
                new SubmoduleDefinition("Bookings", "Bookings"),
                new SubmoduleDefinition("Calendar", "Calendar")
            }),

            new("Hospitality", "Events", "Events", new []
            {
                new SubmoduleDefinition("Planning", "Planning"),
                new SubmoduleDefinition("Staffing", "Staffing")
            }),

            new("Mining", "Safety", "Safety", new []
            {
                new SubmoduleDefinition("Incidents", "Incidents"),
                new SubmoduleDefinition("Training", "Training")
            }),

            new("Logistics", "Dispatch", "Dispatch", new []
            {
                new SubmoduleDefinition("Assignments", "Assignments"),
                new SubmoduleDefinition("Tracking", "Tracking")
            }),

            new("ProfessionalServices", "Contracts", "Contracts", new []
            {
                new SubmoduleDefinition("Templates", "Templates"),
                new SubmoduleDefinition("Approvals", "Approvals")
            }),

            new("ProfessionalServices", "FieldService", "Field Service", new []
            {
                new SubmoduleDefinition("Visits", "Visits"),
                new SubmoduleDefinition("Reports", "Reports")
            })
        };
    }
}