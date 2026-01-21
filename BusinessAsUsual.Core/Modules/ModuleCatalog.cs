namespace BusinessAsUsual.Core.Modules;

public static class ModuleCatalog
{
    public static IReadOnlyList<ModuleDefinition> AllModules { get; } =
        new List<ModuleDefinition>
        {
            new ModuleDefinition("Core", "Admin", "Admin", new []
            {
                new SubmoduleDefinition("ADM_CompSettings", "Company Settings"),
                new SubmoduleDefinition("ADM_UsrMgmt", "User Management"),
                new SubmoduleDefinition("ADM_RolesPerms", "Roles & Permissions"),
                new SubmoduleDefinition("ADM_TenantConfig", "Tenant Configuration"),
                new SubmoduleDefinition("ADM_Audit", "Audit Logs"),
                new SubmoduleDefinition("ADM_Brand", "Branding & Theme"),
                new SubmoduleDefinition("ADM_APIInt", "API Keys / Integration")
            }),

            new ModuleDefinition("Core", "HR", "Human Resources", new []
            {
                new SubmoduleDefinition("HR_Employees", "Employee Records"),
                new SubmoduleDefinition("HR_OnboardingDocs", "Onboarding Documents"),
                new SubmoduleDefinition("HR_PTO", "PTO / Leave Management"),
                new SubmoduleDefinition("HR_Payroll", "Payroll Integration"),
                new SubmoduleDefinition("HR_Training", "Certifications & Training"),
                new SubmoduleDefinition("HR_Performance", "Performance Reviews"),
                new SubmoduleDefinition("HR_BenefitsComp", "Benefits & Compensation"),
                new SubmoduleDefinition("HR_Compliance", "Compliance (I-9, W-4, OSHA, etc.")
            }),

            new ModuleDefinition("Core", "Finance", "Finance", new []
            {
                new SubmoduleDefinition("FIN_GL", "General Ledger"),
                new SubmoduleDefinition("FIN_AR", "Accounts Receivable"),
                new SubmoduleDefinition("FIN_AP", "Accounts Payable"),
                new SubmoduleDefinition("FIN_Budget", "Budgeting"),
                new SubmoduleDefinition("FIN_ExpenseMgmt", "Expense Management"),
                new SubmoduleDefinition("FIN_Reports", "Financial Reporting"),
                new SubmoduleDefinition("FIN_BankReconcile", "Bank Reconciliation"),
                new SubmoduleDefinition("FIN_TaxCfg", "Tax Configuration"),
                new SubmoduleDefinition("FIN_Currency", "Multi-Currency Support")
            }),

            new ModuleDefinition("Core", "CRM", "CRM", new []
            {
                new SubmoduleDefinition("CRM_Leads", "Leads"),
                new SubmoduleDefinition("CRM_Opps", "Opportunities"),
                new SubmoduleDefinition("CRM_Pipelines", "Pipelines"),
                new SubmoduleDefinition("CRM_AcctContacts", "Accounts & Contacts"),
                new SubmoduleDefinition("CRM_ActNote", "Activities & Notes"),
                new SubmoduleDefinition("CRM_EmailInt", "Email Integration"),
                new SubmoduleDefinition("CRM_SalesFcst", "Sales Forcasting"),
                new SubmoduleDefinition("CRM_CustSeg", "Customer Segmentation")
            }),

            new ModuleDefinition("Core", "Inventory", "Inventory", new []
            {
                new SubmoduleDefinition("INV_StockLvls", "Stock Levels"),
                new SubmoduleDefinition("INV_Warehouses", "Warehouses"),
                new SubmoduleDefinition("INV_POs", "Purchase Orders"),
                new SubmoduleDefinition("INV_Rec", "Receiving"),
                new SubmoduleDefinition("INV_Transfers", "Transfers"),
                new SubmoduleDefinition("INV_Adj", "Adjustments"),
                new SubmoduleDefinition("INV_LSTracking", "Lot/Serial Tracking"),
                new SubmoduleDefinition("INV_ReorderRules", "Reorder Rules"),
                new SubmoduleDefinition("INV_VendorMgmt", "Vendor Management")
            }),

            new ModuleDefinition("Core", "Timekeeping", "Timekeeping", new []
            {
                new SubmoduleDefinition("TK_Timesheets", "Timesheets"),
                new SubmoduleDefinition("TK_JobCosting", "Job Costing"),
                new SubmoduleDefinition("TK_LaborCats", "Labor Categories"),
                new SubmoduleDefinition("TK_OTRules", "Overtime Rules"),
                new SubmoduleDefinition("TK_Approvals", "Approvals"),
                new SubmoduleDefinition("TK_Export", "Payroll Export")
            }),

            new ModuleDefinition("Core", "A&C", "Audit & Compliance", new []
            {
                new SubmoduleDefinition("AC_ContractAudit", "Contract Audit"),
                new SubmoduleDefinition("AC_DocComp", "Document Compliance"),
                new SubmoduleDefinition("AC_Certs", "Certifications"),
                new SubmoduleDefinition("AC_PolicyAck", "Policy Acknowledgements"),
                new SubmoduleDefinition("AC_InternalCtrls", "Internal Controls"),
                new SubmoduleDefinition("AC_RiskReg", "Risk Register")
            }),

            new ModuleDefinition("Retail", "POS", "Point of Sale (POS)", new []
            {
                new SubmoduleDefinition("POS_RegMgmt", "Register Management"),
                new SubmoduleDefinition("POS_BarcodeScan", "Barcode Scanning"),
                new SubmoduleDefinition("POS_CashDrawer", "Cash Drawer"),
                new SubmoduleDefinition("POS_RtnsExch", "Returns & Exchanges"),
                new SubmoduleDefinition("POS_ReceiptPrint", "Receipt Printing")
            }),

            new ModuleDefinition("Retail", "Products", "Product Catalog", new []
            {
                new SubmoduleDefinition("PC_SKUs", "SKUs"),
                new SubmoduleDefinition("PC_Variants", "Variants"),
                new SubmoduleDefinition("PC_Categories", "Categories"),
                new SubmoduleDefinition("PC_PricingRules", "Pricing Rules"),
                new SubmoduleDefinition("PC_ImgDesc", "Images & Descriptions")
            }),

            new ModuleDefinition("Retail", "Promotions", "Promotions", new []
            {
                new SubmoduleDefinition("PRO_Discounts", "Discounts"),
                new SubmoduleDefinition("PRO_Coupons", "Coupons"),
                new SubmoduleDefinition("PRO_BOGO", "BOGO"),
                new SubmoduleDefinition("PRO_SchedPromo", "Scheduled Promotions")
            }),

            new ModuleDefinition("Retail", "Loyalty", "Loyalty", new []
            {
                new SubmoduleDefinition("LOY_Points", "Points"),
                new SubmoduleDefinition("LOY_Rewards", "Rewards"),
                new SubmoduleDefinition("LOY_CustTiers", "Customer Tiers"),
                new SubmoduleDefinition("LOY_RedeemRules", "Redemption Rules")
            }),

            new ModuleDefinition("Manufacturing", "Production", "Production", new []
            {
                new SubmoduleDefinition("MAN_WorkCenters", "Work Centers"),
                new SubmoduleDefinition("MAN_ProdSched", "Production Schedules"),
                new SubmoduleDefinition("MAN_MatReq", "Material Requirements"),
                new SubmoduleDefinition("MAN_CapPlan", "Capacity Planning"),
                new SubmoduleDefinition("MAN_ProdOrders", "Production Orders"),
                new SubmoduleDefinition("MAN_ShopFlExec", "Shop Floor Execution")
            }),

            new ModuleDefinition("Manufacturing", "WO", "Work Orders", new []
            {
                new SubmoduleDefinition("WO_WOCreation", "Work Order Creation"),
                new SubmoduleDefinition("WO_RoutingSteps", "Routing Steps"),
                new SubmoduleDefinition("WO_LaborTrack", "Labor Tracking"),
                new SubmoduleDefinition("WO_MatConsum", "Material Consumption"),
                new SubmoduleDefinition("WO_CompScrap", "Completion & Scrap")
            }),

            new ModuleDefinition("Manufacturing", "BOM", "Bill of Materials (BOM)", new []
            {
                new SubmoduleDefinition("BOM_MultiLovel", "Multi-Level BOM"),
                new SubmoduleDefinition("BOM_CompSubs", "Component Substitutions"),
                new SubmoduleDefinition("BOM_CostRollups", "Cost Rollups"),
                new SubmoduleDefinition("BOM_RevControl", "Revision Control")
            }), 

            new ModuleDefinition("Manufacturing", "Routing", "Routing", new []
            {
                new SubmoduleDefinition("RTE_OpSteps", "Operation Steps"),
                new SubmoduleDefinition("RTE_SetupRuntimes", "Setup & Run Times"),
                new SubmoduleDefinition("RTE_WCAssign", "Work Center Assignments"),
                new SubmoduleDefinition("RTE_EfficTrack", "Efficiency Tracking")
            }),

            new ModuleDefinition("Manufacturing", "MRP", "Material Requirements Planning", new []
            {
                new SubmoduleDefinition("MRP_DemandFC", "Demand Forecasting"),
                new SubmoduleDefinition("MRP_SupplyPlan", "Supply Planning"),
                new SubmoduleDefinition("MRP_ShortAnalysis", "Shortage Analysis"),
                new SubmoduleDefinition("MRP_SugPO", "Suggested Purchase Orders")
            }),

            new ModuleDefinition("Manufacturing", "QA", "Quality Assurance", new []
            {
                new SubmoduleDefinition("QA_Inpections", "Inspections"),
                new SubmoduleDefinition("QA_NonConfReports", "Non-Conformance Reports"),
                new SubmoduleDefinition("QA_CorrectiveActions", "Corrective Actions"),
                new SubmoduleDefinition("QA_SamplingPlans", "Sampling Plans")
            }),

            new ModuleDefinition("Mining / Drilling", "EquipMaint", "Equipment Maintenance", new []
            {
                new SubmoduleDefinition("EM_PrevMaint", "Preventive Maintenance"),
                new SubmoduleDefinition("EM_WO", "Work Orders"),
                new SubmoduleDefinition("EM_PartsInv", "Parts Inventory"),
                new SubmoduleDefinition("EM_MaintLogs", "Maintenance Logs")
            }),

            new ModuleDefinition("Mining / Drilling", "FieldOps", "Field Operations", new []
            {
                new SubmoduleDefinition("FO_Tickets", "Field Tickets"),
                new SubmoduleDefinition("FO_CrewAssn", "Crew Assignments"),
                new SubmoduleDefinition("FO_JobSites", "Job Sites"),
                new SubmoduleDefinition("FO_DAL", "Daily Activity Logs")
            }),

            new ModuleDefinition("Mining / Drilling", "SafetyCompliance", "Safety & Compliance", new []
            {
                new SubmoduleDefinition("SC_SafetyInc", "Safety Incidents"),
                new SubmoduleDefinition("SC_TrainingRecs", "Training Records"),
                new SubmoduleDefinition("SC_OSHALogs", "OSHA Logs"),
                new SubmoduleDefinition("SC_PPETracking", "PPE Tracking")
            }),

            new ModuleDefinition("Mining / Drilling", "ExtractionTracking", "Extraction Tracking", new []
            {
                new SubmoduleDefinition("ET_ProdLogs", "Production Logs"),
                new SubmoduleDefinition("ET_ResYield", "Resource Yield"),
                new SubmoduleDefinition("ET_SiteReporting", "Site Reporting"),
                new SubmoduleDefinition("ET_EnvMetrics", "Environmental Metrics")
            }),

            new ModuleDefinition("Logistics", "FleetMgmt", "Fleet Management", new []
            {
                new SubmoduleDefinition("FM_Vehicles", "Vehicles"),
                new SubmoduleDefinition("FM_Maint", "Maintenance"),
                new SubmoduleDefinition("FM_FuelLogs", "Fuel Logs"),
                new SubmoduleDefinition("FM_GPSTracking", "GPS Tracking")
            }),

            new ModuleDefinition("Logistics", "Dispatch", "Dispatch", new []
            {
                new SubmoduleDefinition("DIS_RoutePlanning", "Route Planning"),
                new SubmoduleDefinition("DIS_DriverAssn", "Driver Assignments"),
                new SubmoduleDefinition("DIS_LoadMgmt", "Load Management"),
                new SubmoduleDefinition("DIS_DeliveryWindows", "Delivery Windows")
            }),

            new ModuleDefinition("Logistics", "ShipmentTracking", "Shipment Tracking", new []
            {
                new SubmoduleDefinition("ST_TrkNumbers", "Tracking Numbers"),
                new SubmoduleDefinition("ST_StatusUpdates", "Status Updates"),
                new SubmoduleDefinition("ST_POD", "Proof of Delivery"),
                new SubmoduleDefinition("ST_CarrierInt", "Carrier Integrations")
            }),

            new ModuleDefinition("Logistics", "CarrierMgmt", "Carrier Management", new []
            {
                new SubmoduleDefinition("CM_Profiles", "Carrier Profiles"),
                new SubmoduleDefinition("CM_Rates", "Rates"),
                new SubmoduleDefinition("CM_Contracts", "Contracts"),
                new SubmoduleDefinition("CM_PerfMetrics", "Performance Metrics")
            }),

            new ModuleDefinition("Healthcare", "PatientMgmt", "Patient Management", new []
            {
                new SubmoduleDefinition("PM_PatientRecords", "Patient Records"),
                new SubmoduleDefinition("PM_Insurance", "Insurance"),
                new SubmoduleDefinition("PM_Demographics", "Demographics"),
                new SubmoduleDefinition("PM_MedHistory", "Medical History")
            }),

            new ModuleDefinition("Healthcare", "Appointments", "Appointments", new []
            {
                new SubmoduleDefinition("APPT_Scheduling", "Scheduling"),
                new SubmoduleDefinition("APPT_Reminders", "Reminders"),
                new SubmoduleDefinition("APPT_ProvAvail", "Provider Availability"),
                new SubmoduleDefinition("APPT_CheckIn", "Check-In")
            }),

            new ModuleDefinition("Healthcare", "ProviderScheduling", "Provider Scheduling", new []
            {
                new SubmoduleDefinition("PS_Rotations", "Rotations"),
                new SubmoduleDefinition("PS_ShiftAssign", "Shift Assignments"),
                new SubmoduleDefinition("PS_Credentialing", "Credentialing")
            }),

            new ModuleDefinition("Healthcare", "Compliance", "Compliance", new []
            {
                new SubmoduleDefinition("COMP_HIPPADocs", "HIPAA Documents"),
                new SubmoduleDefinition("COMP_AccessLogs", "Access Logs"),
                new SubmoduleDefinition("COMP_Training", "Training"),
                new SubmoduleDefinition("COMP_IncidentReporting", "Incident Reporting")
            }),

            new ModuleDefinition("Hospitality", "Reservations", "Reservations", new []
            {
                new SubmoduleDefinition("RES_BookEngine", "Booking Engine"),
                new SubmoduleDefinition("RES_AvailCal", "Availabiltiy Calendar"),
                new SubmoduleDefinition("RES_RatePlans", "Rate Plans"),
                new SubmoduleDefinition("RES_Deposits", "Deposits")
            }),

            new ModuleDefinition("Hospitality", "GuestServices", "Guest Services", new []
            {
                new SubmoduleDefinition("GS_Requests", "Requests"),
                new SubmoduleDefinition("GS_Housekeeping", "Housekeeping"),
                new SubmoduleDefinition("GS_RoomService", "Room Service"),
                new SubmoduleDefinition("GS_Amenities", "Amenities")
            }),

            new ModuleDefinition("Hospitality", "EventScheduling", "Event Scheduling", new []
            {
                new SubmoduleDefinition("ES_Venues", "Venues"),
                new SubmoduleDefinition("ES_Catering", "Catering"),
                new SubmoduleDefinition("ES_StaffAssign", "Staff Assignments"),
                new SubmoduleDefinition("ES_Billing", "Billing")
            }),

            new ModuleDefinition("Professional Services", "ProjectManagement", "Project Management", new []
            {
                new SubmoduleDefinition("PM_Tasks", "Tasks"),
                new SubmoduleDefinition("PM_Milestones", "Milestones"),
                new SubmoduleDefinition("PM_Gantt", "Gantt"),
                new SubmoduleDefinition("PM_Dependencies", "Dependencies")
            }),

            new ModuleDefinition("Professional Services", "BillingInvoicing", "Billing & Invoicing", new []
            {
                new SubmoduleDefinition("BI_TimeMats", "Time & Materials"),
                new SubmoduleDefinition("BI_FixedFee", "Fixed Fee"),
                new SubmoduleDefinition("BI_Retainers", "Retainers"),
                new SubmoduleDefinition("BI_InvoiceTemplates", "Invoice Templates")
            }),

            new ModuleDefinition("Professional Services", "ContractManagement", "Contract Management", new []
            {
                new SubmoduleDefinition("CM_ContractTempl", "Contract Templates"),
                new SubmoduleDefinition("CM_Renewals", "Renewals"),
                new SubmoduleDefinition("CM_Amendments", "Amendments"),
                new SubmoduleDefinition("CM_Approvals", "Approvals")
            }),

            new ModuleDefinition("Professional Services", "ResourceAllocation", "Resource Allocation", new []
            {
                new SubmoduleDefinition("RA_Staffing", "Staffing"),
                new SubmoduleDefinition("RA_Utilization", "Utilization"),
                new SubmoduleDefinition("RA_SkillsMatrix", "Skills Matrix"),
                new SubmoduleDefinition("RA_Forecasting", "Forecasting")
            }),

            new ModuleDefinition("Cross-Industry Power Modules", "AssetManagement", "Asset Management", new []
            {
                new SubmoduleDefinition("AM_AssetReg", "Asset Register"),
                new SubmoduleDefinition("AM_Depr", "Depreciation"),
                new SubmoduleDefinition("AM_Maint", "Maintenance"),
                new SubmoduleDefinition("AM_WarrantTrack", "Warranty Tracking")
            }),

            new ModuleDefinition("Cross-Industry Power Modules", "ServiceContracts", "Service Contract", new []
            {
                new SubmoduleDefinition("SC_SLAs", "SLAs"),
                new SubmoduleDefinition("SC_Renewals", "Renewals"),
                new SubmoduleDefinition("SC_ContractBilling", "Contract Billing"),
                new SubmoduleDefinition("SC_Entitlements", "Entitlements")
            }),

            new ModuleDefinition("Cross-Industry Power Modules", "DocumentManagement", "Document Management", new []
            {
                new SubmoduleDefinition("DM_FileStore", "File Storage"),
                new SubmoduleDefinition("DM_Versioning", "Versioning"),
                new SubmoduleDefinition("DM_Tagging", "Tagging"),
                new SubmoduleDefinition("DM_AccessCtrl", "Access Control")
            }),

            new ModuleDefinition("Cross-Industry Power Modules", "AnalyticsBI", "Analytics & BI", new []
            {
                new SubmoduleDefinition("ABI_Dashboards", "Dashboards"),
                new SubmoduleDefinition("ABI_KPIs", "KPIs"),
                new SubmoduleDefinition("API_Forecasting", "Forecasting"),
                new SubmoduleDefinition("ABI_DataWHConn", "Data Warehouse Connectors")
            }),

            new ModuleDefinition("Cross-Industry Power Modules", "WorkflowAutomation", "Workflow Automation", new []
            {
                new SubmoduleDefinition("WA_Triggers", "Triggers"),
                new SubmoduleDefinition("WA_Approvals", "Approvals"),
                new SubmoduleDefinition("WA_CustomFlows", "Custom Flows"),
                new SubmoduleDefinition("WA_Notification", "Notifications")
            }),

            new ModuleDefinition("Cross-Industry Power Modules", "Procurement", "Procurement", new []
            {
                new SubmoduleDefinition("WA_RFQs", "RFQs"),
                new SubmoduleDefinition("WA_VendorQuotes", "Vendor Quotes"),
                new SubmoduleDefinition("WA_PurchaseAppr", "Purchase Approvals"),
                new SubmoduleDefinition("WA_ContractedPricing", "Contracted Pricing")
            }),

            new ModuleDefinition("Cross-Industry Power Modules", "ECommerce", "E-Commerce", new []
            {
                new SubmoduleDefinition("EC_Storefront", "Storefront"),
                new SubmoduleDefinition("EC_Cart", "Cart"),
                new SubmoduleDefinition("EC_Checkout", "Checkout"),
                new SubmoduleDefinition("EC_OrderSync", "Order Sync")
            }),

            new ModuleDefinition("Cross-Industry Power Modules", "FieldService", "Field Service", new []
            {
                new SubmoduleDefinition("FS_Technicians", "Technicians"),
                new SubmoduleDefinition("FS_Dispatch", "Dispatch"),
                new SubmoduleDefinition("FS_ServiceTickets", "Service Tickets"),
                new SubmoduleDefinition("FS_PartsUsed", "Parts Used")
            })
        };
}