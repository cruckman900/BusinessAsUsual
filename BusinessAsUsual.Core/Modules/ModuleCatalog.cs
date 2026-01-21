using System.Collections.Generic;

namespace BusinessAsUsual.Core.Modules
{
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
            new("Financial", "Accounting", "Accounting", new []
            {
                new SubmoduleDefinition("ChartOfAccounts", "Chart of Accounts"),
                new SubmoduleDefinition("JournalEntries", "Journal Entries")
            }),

            new("Financial", "GeneralLedger", "General Ledger", new []
            {
                new SubmoduleDefinition("Ledgers", "Ledgers"),
                new SubmoduleDefinition("Reconciliation", "Reconciliation")
            }),

            new("Financial", "AccountsReceivable", "Accounts Receivable", new []
            {
                new SubmoduleDefinition("Invoices", "Invoices"),
                new SubmoduleDefinition("Collections", "Collections")
            }),

            new("Financial", "AccountsPayable", "Accounts Payable", new []
            {
                new SubmoduleDefinition("Bills", "Bills"),
                new SubmoduleDefinition("VendorPayments", "Vendor Payments")
            }),

            new("Financial", "Billing", "Billing", new []
            {
                new SubmoduleDefinition("Recurring", "Recurring Billing"),
                new SubmoduleDefinition("OneTime", "One-Time Billing")
            }),

            new("Financial", "Invoicing", "Invoicing", new []
            {
                new SubmoduleDefinition("Templates", "Templates"),
                new SubmoduleDefinition("Delivery", "Delivery")
            }),

            new("Financial", "Payments", "Payments", new []
            {
                new SubmoduleDefinition("Gateways", "Gateways"),
                new SubmoduleDefinition("Reconciliation", "Reconciliation")
            }),

            new("Financial", "Payroll", "Payroll", new []
            {
                new SubmoduleDefinition("PayRuns", "Pay Runs"),
                new SubmoduleDefinition("Deductions", "Deductions")
            }),

            new("Financial", "Banking", "Banking", new []
            {
                new SubmoduleDefinition("Accounts", "Bank Accounts"),
                new SubmoduleDefinition("Transactions", "Transactions")
            }),

            new("Financial", "Budgeting", "Budgeting", new []
            {
                new SubmoduleDefinition("Forecasts", "Forecasts"),
                new SubmoduleDefinition("Allocations", "Allocations")
            }),

            new("Financial", "Taxation", "Taxation", new []
            {
                new SubmoduleDefinition("Rates", "Tax Rates"),
                new SubmoduleDefinition("Filings", "Filings")
            }),

            // ============================================================
            // SALES & CRM
            // ============================================================
            new("Sales", "CRM", "CRM", new []
            {
                new SubmoduleDefinition("Leads", "Leads"),
                new SubmoduleDefinition("Opportunities", "Opportunities")
            }),

            new("Sales", "Customers", "Customers", new []
            {
                new SubmoduleDefinition("Profiles", "Profiles"),
                new SubmoduleDefinition("History", "History")
            }),

            new("Sales", "Quotes", "Quotes & Estimates", new []
            {
                new SubmoduleDefinition("Drafts", "Drafts"),
                new SubmoduleDefinition("Approvals", "Approvals")
            }),

            new("Sales", "Orders", "Orders", new []
            {
                new SubmoduleDefinition("SalesOrders", "Sales Orders"),
                new SubmoduleDefinition("Fulfillment", "Fulfillment")
            }),

            new("Sales", "Subscriptions", "Subscriptions", new []
            {
                new SubmoduleDefinition("Plans", "Plans"),
                new SubmoduleDefinition("Renewals", "Renewals")
            }),

            new("Sales", "POS", "Point of Sale", new []
            {
                new SubmoduleDefinition("Registers", "Registers"),
                new SubmoduleDefinition("Receipts", "Receipts")
            }),

            new("Sales", "Products", "Products", new []
            {
                new SubmoduleDefinition("Catalog", "Catalog"),
                new SubmoduleDefinition("Variants", "Variants")
            }),

            new("Sales", "Menu", "Menu Management", new []
            {
                new SubmoduleDefinition("Items", "Items"),
                new SubmoduleDefinition("Categories", "Categories")
            }),

            new("Sales", "CustomerPortal", "Customer Portal", new []
            {
                new SubmoduleDefinition("Access", "Access"),
                new SubmoduleDefinition("SelfService", "Self Service")
            }),

            // ============================================================
            // OPERATIONS
            // ============================================================
            new("Operations", "Inventory", "Inventory", new []
            {
                new SubmoduleDefinition("StockLevels", "Stock Levels"),
                new SubmoduleDefinition("Adjustments", "Adjustments")
            }),

            new("Operations", "Warehousing", "Warehousing", new []
            {
                new SubmoduleDefinition("Bins", "Bins"),
                new SubmoduleDefinition("Transfers", "Transfers")
            }),

            new("Operations", "Purchasing", "Purchasing", new []
            {
                new SubmoduleDefinition("PurchaseOrders", "Purchase Orders"),
                new SubmoduleDefinition("Receipts", "Receipts")
            }),

            new("Operations", "Procurement", "Procurement", new []
            {
                new SubmoduleDefinition("Vendors", "Vendors"),
                new SubmoduleDefinition("Contracts", "Contracts")
            }),

            new("Operations", "Suppliers", "Suppliers", new []
            {
                new SubmoduleDefinition("Profiles", "Profiles"),
                new SubmoduleDefinition("Ratings", "Ratings")
            }),

            new("Operations", "Equipment", "Equipment", new []
            {
                new SubmoduleDefinition("Assets", "Assets"),
                new SubmoduleDefinition("Maintenance", "Maintenance")
            }),

            new("Operations", "Maintenance", "Maintenance", new []
            {
                new SubmoduleDefinition("Schedules", "Schedules"),
                new SubmoduleDefinition("WorkOrders", "Work Orders")
            }),

            new("Operations", "Vehicles", "Vehicles", new []
            {
                new SubmoduleDefinition("Fleet", "Fleet"),
                new SubmoduleDefinition("Maintenance", "Maintenance")
            }),

            new("Operations", "FleetManagement", "Fleet Management", new []
            {
                new SubmoduleDefinition("Dispatch", "Dispatch"),
                new SubmoduleDefinition("Tracking", "Tracking")
            }),

            new("Operations", "Logistics", "Logistics", new []
            {
                new SubmoduleDefinition("Shipments", "Shipments"),
                new SubmoduleDefinition("Carriers", "Carriers")
            }),

            new("Operations", "Routing", "Routing", new []
            {
                new SubmoduleDefinition("Routes", "Routes"),
                new SubmoduleDefinition("Optimization", "Optimization")
            }),

            new("Operations", "Scheduling", "Scheduling", new []
            {
                new SubmoduleDefinition("Calendar", "Calendar"),
                new SubmoduleDefinition("Assignments", "Assignments")
            }),

            new("Operations", "Projects", "Projects", new []
            {
                new SubmoduleDefinition("Phases", "Phases"),
                new SubmoduleDefinition("Milestones", "Milestones")
            }),

            new("Operations", "Tasks", "Tasks", new []
            {
                new SubmoduleDefinition("Boards", "Boards"),
                new SubmoduleDefinition("Assignments", "Assignments")
            }),

            new("Operations", "Jobs", "Work Orders", new []
            {
                new SubmoduleDefinition("Dispatch", "Dispatch"),
                new SubmoduleDefinition("Completion", "Completion")
            }),

            new("Operations", "Services", "Services", new []
            {
                new SubmoduleDefinition("Catalog", "Catalog"),
                new SubmoduleDefinition("Pricing", "Pricing")
            }),

            new("Operations", "Workflows", "Workflows", new []
            {
                new SubmoduleDefinition("Automation", "Automation"),
                new SubmoduleDefinition("Triggers", "Triggers")
            }),

            new("Operations", "Replenishment", "Replenishment", new []
            {
                new SubmoduleDefinition("Rules", "Rules"),
                new SubmoduleDefinition("Forecasts", "Forecasts")
            }),

            new("Operations", "Forecasting", "Forecasting", new []
            {
                new SubmoduleDefinition("Demand", "Demand"),
                new SubmoduleDefinition("Supply", "Supply")
            }),

            new("Operations", "QualityControl", "Quality Control", new []
            {
                new SubmoduleDefinition("Inspections", "Inspections"),
                new SubmoduleDefinition("NonConformance", "Non-Conformance")
            }),

            new("Operations", "Compliance", "Compliance", new []
            {
                new SubmoduleDefinition("Policies", "Policies"),
                new SubmoduleDefinition("Audits", "Audits")
            }),

            new("Operations", "AssetManagement", "Asset Management", new []
            {
                new SubmoduleDefinition("Tracking", "Tracking"),
                new SubmoduleDefinition("Depreciation", "Depreciation")
            }),

            // ============================================================
            // HR & PEOPLE
            // ============================================================
            new("HR", "HR", "Human Resources", new []
            {
                new SubmoduleDefinition("Records", "Employee Records"),
                new SubmoduleDefinition("Benefits", "Benefits")
            }),

            new("HR", "Staff", "Staff Management", new []
            {
                new SubmoduleDefinition("Profiles", "Profiles"),
                new SubmoduleDefinition("Roles", "Roles")
            }),

            new("HR", "Recruiting", "Recruiting", new []
            {
                new SubmoduleDefinition("Applicants", "Applicants"),
                new SubmoduleDefinition("Interviews", "Interviews")
            }),

            new("HR", "Onboarding", "Onboarding", new []
            {
                new SubmoduleDefinition("Checklists", "Checklists"),
                new SubmoduleDefinition("Training", "Training")
            }),

            new("HR", "Performance", "Performance", new []
            {
                new SubmoduleDefinition("Reviews", "Reviews"),
                new SubmoduleDefinition("Goals", "Goals")
            }),

            new("HR", "Training", "Training", new []
            {
                new SubmoduleDefinition("Courses", "Courses"),
                new SubmoduleDefinition("Certifications", "Certifications")
            }),

            new("HR", "Timekeeping", "Timekeeping", new []
            {
                new SubmoduleDefinition("Timesheets", "Timesheets"),
                new SubmoduleDefinition("Approvals", "Approvals")
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