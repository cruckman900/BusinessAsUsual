# Project TODO & Backlog

## In Progress

### ✅ Tooltips Implementation (COMPLETED)
**Status**: Completed  
**Priority**: High  
**Description**: Implement tooltips across all BusinessAsUsual.Web and microservice module UIs  

**Completed Work**:
- ✅ Added tooltips to all buttons in HR.Web/Components/Pages/Benefits.razor
- ✅ Added tooltips to all buttons in HR.Web/Components/Pages/Onboarding.razor
- ✅ Added tooltips to all buttons in HR.Web/Components/Pages/Departments.razor
- ✅ Added tooltips to HR.Web/Components/Pages/Home.razor navigation
- ✅ Added tooltips to BusinessAsUsual.Web/Pages/Dashboard.razor module cards
- ✅ Added tooltips to BusinessAsUsual.Web sidebar navigation
- ✅ Created UI_STANDARDS.md with mandatory tooltip requirements
- ✅ HR.Web/Components/Pages/Employees.razor already had tooltips

**Standard**: All interactive elements (buttons, icon buttons, navigation) must have tooltips (see docs/UI_STANDARDS.md)

---

## High Priority

### Route Cleanup
**Status**: Completed  
**Priority**: High  
**Description**: Remove duplicate legacy shell HR routes that conflict with injected HR.Web module  

**Completed**:
- ✅ Removed `/hr/benefits` route from `BusinessAsUsual.Web/Modules/HR/Pages/BenefitsManagement.razor`
- ✅ Removed `/hr/onboarding` route from `BusinessAsUsual.Web/Modules/HR/Pages/NewHireOnboarding.razor`
- ✅ Removed `/hr` route from `BusinessAsUsual.Web/Modules/HR/Index.razor`

---

## Medium Priority

### HR Module - Additional Reports
**Status**: Proposed  
**Priority**: Medium  
**Description**: Build additional HR reporting and management pages  

Proposed reports (see docs/HR_REPORTS_COMPLETE.md):
- [ ] Compensation & Payroll Report
- [ ] Time Off & Leave Management
- [ ] Performance Reviews Dashboard
- [ ] Training & Development Tracker
- [ ] Turnover & Retention Analytics
- [ ] Diversity, Equity & Inclusion (DEI) Dashboard
- [ ] Headcount Planning & Forecasting
- [ ] Organization Chart Viewer
- [ ] New Hire Analytics

### Mobile API Contracts
**Status**: In Progress  
**Priority**: Medium  
**Description**: Expand mobile API contracts for all HR module features  

**Completed**:
- ✅ Created HR.Mobile.Contracts project
- ✅ Added mobile metadata to Module Registry
- ✅ Documented mobile UI construction in MICROSERVICEARCHITECTUREOVERVIEW.md

**Remaining**:
- [ ] Add mobile contracts for Benefits
- [ ] Add mobile contracts for Onboarding
- [ ] Add mobile contracts for Departments detail screens
- [ ] Implement mobile API endpoints in HR.API

---

## Low Priority / Future Enhancements

### Visual Theming
**Status**: Not Started  
**Priority**: Low  
**Description**: Implement company-specific theming and branding  
- [ ] Custom MudBlazor theme
- [ ] Logo/branding customization
- [ ] Dark mode support

### Analytics & Telemetry
**Status**: Not Started  
**Priority**: Low  
**Description**: Add usage analytics and performance monitoring  
- [ ] Application Insights integration
- [ ] User activity tracking
- [ ] Performance metrics dashboard

### Automated Testing
**Status**: Not Started  
**Priority**: Medium  
**Description**: Expand test coverage  
- [ ] Unit tests for HR services
- [ ] Integration tests for HR.API
- [ ] UI component tests (bUnit)
- [ ] End-to-end tests (Playwright)

### Documentation
**Status**: Ongoing  
**Priority**: Medium  
**Description**: Keep documentation current  
- ✅ Created UI_STANDARDS.md
- ✅ Created HR_REPORTS_COMPLETE.md
- ✅ Updated MICROSERVICEARCHITECTUREOVERVIEW.md
- [ ] Create API documentation (Swagger/OpenAPI)
- [ ] Add inline code comments where needed
- [ ] Video walkthroughs for key features

---

## Completed

### ✅ Microservice UI Injection Architecture
- Created Module Registry Service (MRS)
- Implemented HR service self-registration
- Dynamic module discovery in shell
- Iframe embedding for module UIs
- Shell-to-iframe navigation via postMessage
- Module navigation items displayed in sidebar

### ✅ HR.Web Microservice Module
- Created HR.Web Blazor Web App
- Converted to MudBlazor UI components
- Created IframeLayout for module pages
- Built Home, Employees, Departments, Onboarding, Benefits pages
- Fixed CSP/CORS for iframe embedding

### ✅ HR Reports (Initial Set)
- Dashboard / Home page
- Employee directory with MudDataGrid
- Departments overview with cards
- Onboarding pipeline tracking
- Benefits administration report

---

## Notes

- **Tooltip Standard**: As of this session, tooltips are a MANDATORY UI requirement. All new pages and components must include tooltips on all interactive elements.
- **MudBlazor First**: Always prefer MudBlazor components over custom HTML/CSS
- **Consistent Patterns**: Follow the examples in existing pages (Employees.razor, Benefits.razor) for new pages
- **Documentation**: Update relevant docs when adding new features or changing architecture

---

## How to Use This File

1. **Adding new tasks**: Add to appropriate priority section with status "Not Started"
2. **Starting work**: Move to "In Progress" and update status
3. **Completing work**: Move to "Completed" section with completion date
4. **Tracking progress**: Use checkboxes [ ] for sub-tasks

---

Last Updated: 2024-01-XX
