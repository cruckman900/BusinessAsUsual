# CRM Feature Roadmap 🚀

## Current Status
- ✅ **Leads Management** - Full CRUD with CustomDataGrid, status/source filtering
- ✅ **Opportunities Management** - Full CRUD with CustomDataGrid, stage filtering
- ✅ **Customers Management** - Full CRUD with CustomDataGrid, status filtering
- ✅ **Dashboard** - Polished home page with module cards and quick stats
- ✅ **Settings** - Basic CRM configuration page
- ✅ **Reports** - Full analytics dashboard with 5 interactive charts (COMPLETE!)
- ✅ **Activities** - Timeline view with filtering and CRUD operations (COMPLETE!)
- ✅ **Sales Pipeline Kanban** - Drag-and-drop board across opportunity stages (COMPLETE!)
- ✅ **Email Templates & Communication** - Template library with merge fields and send-from-pipeline (COMPLETE!)

---

## Phase 1: Reports & Analytics 📊
**Status**: ✅ COMPLETE!
**Goal**: Build comprehensive reporting dashboard with interactive charts

### 1.1 Setup & Infrastructure ✅
- [x] Add ApexCharts.Blazor package to CRM.Web (recommended - beautiful, interactive, well-documented)
- [x] Add ApexCharts.Blazor package to BusinessAsUsual.Web (shell app)
- [x] Register ApexCharts service in shell app
- [x] Create report service interfaces and implementations
- [x] Add report DTOs for aggregated data
- [x] Build KPI dashboard with summary cards
- [x] Register IReportService in shell app DI
- [x] Complete ApexCharts integration with proper lambda bindings
- [x] Convert data tables to interactive charts

### 1.2 Reports Completed ✅
- [x] Summary KPI Cards (Pipeline Value, Win Rate, Active Pipeline, Lead Conversion)
- [x] **Sales Pipeline Bar Chart** - Interactive chart showing opportunities by stage with counts
- [x] **Lead Source Donut Chart** - Visual distribution of leads by source
- [x] **Win/Loss Bar Chart** - Closed won vs closed lost revenue comparison
- [x] **Revenue Forecast Line Chart** - 6-month forecast vs actual (dual-series)
- [x] **Top 10 Customers Bar Chart** - Sorted by lifetime value
- [x] Conversion Funnel Metrics (visual cards)
- [x] Date range filter (30/60/90/180/365 days, All Time)
- [x] Auto-refresh on date range change
- [x] Manual refresh button
- [x] Parallel data loading for performance
- [x] Fixed namespace collisions (MudBlazor vs ApexCharts)

### 1.3 Chart Features ✅
- [x] Interactive ApexCharts with hover tooltips
- [x] Proper lambda expression bindings (XValue/YValue)
- [x] Chart data transformation layer (ChartDataPoint, ForecastDataPoint)
- [x] Consistent styling and height across all charts
- [x] Empty state handling with info alerts
- [x] Chart titles and captions with summary data

---

## Phase 2: Activities & Timeline 📅
**Status**: ✅ COMPLETE! (Core features implemented)
**Goal**: Track interactions with leads, opportunities, and customers

### 2.1 Activity Management ✅
- [x] Activity types (Call, Email, Meeting, Task, Note) - Enum-based in domain
- [x] IActivityService interface with full CRUD operations
- [x] MockActivityService with 13 realistic sample activities
- [x] Link activities to leads/opportunities/customers - All relation fields implemented
- [x] Schedule future activities - DueDate field with upcoming/overdue tracking
- [x] Mark activities as complete - CompleteActivityAsync method
- [x] Activity priority levels (High/Medium/Low)
- [x] Activity duration tracking
- [x] Outcome and next steps fields

### 2.2 Activity List & Timeline View ✅
- [x] Beautiful MudTimeline component showing chronological activity feed
- [x] Filters by Status (All/Upcoming/Overdue/Completed/Pending)
- [x] Filters by Type (Call/Meeting/Email/Task)
- [x] Filters by Priority (High/Medium/Low)
- [x] Quick stats cards (Overdue, This Week, Pending, Completed)
- [x] Activity icons and color coding by status
- [x] Outcome and next steps display for completed activities
- [x] Complete/Edit/Delete actions per activity
- [x] Registered in both CRM.Web and shell app DI

### 2.3 Features Still TODO
- [ ] Add/Edit activity modal dialogs
- [ ] Calendar view (month/week/day)
- [ ] Quick add activity widget from dashboard
- [ ] Activity timeline on Lead/Opportunity/Customer detail pages
- [ ] Activity reminder system
- [ ] Browser notifications

---

## Phase 3: Sales Pipeline Kanban 🎯
**Status**: ✅ COMPLETE! (Core board implemented)
**Goal**: Visual drag-and-drop opportunity management

### 3.1 Kanban Board ✅
- [x] Column for each opportunity stage (Prospecting → Closed Lost)
- [x] Drag-and-drop between stages (MudDropContainer/MudDropZone)
- [x] Opportunity cards with key info
- [x] Color coding by stage and win probability
- [x] Optimistic UI update with revert-on-failure and snackbar feedback
- [x] Persists stage change via `UpdateOpportunityAsync`

### 3.2 Board Features ✅
- [x] Column totals (count and value)
- [x] Search opportunities (filters cards across all columns)
- [x] Responsive layout (horizontal scroll on small screens)
- [x] "Add Opportunity" and "Open Board" entry points (from CRM Dashboard)
- [x] Filter by owner
- [x] Filter by expected close date (overdue / next 30 / next 90 days)
- [ ] Collapse/expand columns

### 3.3 Opportunity Cards ✅
- [x] Customer name
- [x] Deal value
- [x] Probability indicator (chip + linear progress)
- [x] Days in stage
- [x] Expected close date
- [x] Click card to open opportunity detail
- [x] Owner avatar (initials + tooltip + color)
- [x] Quick actions menu (View / Edit / Send Email / Delete)

### 3.4 Features Still TODO
- [ ] Collapse/expand columns
- [ ] Mobile Kanban parity in Android app

---

## Phase 4: Email Templates & Communication ✉️
**Status**: ✅ CORE COMPLETE!
**Goal**: Streamline customer communication

### 4.1 Template Management ✅
- [x] Email template library (`/crm/email-templates`)
- [x] Template categories (Follow-up, Proposal, Scheduling, Closing)
- [x] Merge fields for personalization ({{ContactName}}, {{CompanyName}}, {{OpportunityName}}, {{Amount}}, {{SenderName}})
- [x] Seeded template library (5 starter templates)
- [x] Create / edit / delete templates (dialog editor with merge-field insertion)
- [ ] HTML email editor (rich text)
- [x] Merge-field resolution preview before send

### 4.2 Email Sending ✅
- [x] Send email from opportunity (Pipeline card quick action)
- [x] Select template or write custom
- [x] Merge-field resolution against the linked record
- [x] Save sent emails to activity log (logged as Email activity)
- [ ] Attach files
- [ ] CC/BCC support

### 4.3 Email Tracking (Basic)
- [ ] Log sent emails
- [ ] Track open status (pixel-based)
- [ ] Track link clicks
- [ ] Email engagement metrics

### 4.4 Campaign Management (Advanced)
- [ ] Create email campaigns
- [ ] Bulk send to segments
- [ ] Campaign performance dashboard
- [ ] A/B testing (future)

---

## Phase 5: Notes & Collaboration 📝
**Status**: TODO
**Goal**: Team collaboration and knowledge sharing

### 5.1 Notes System
- [ ] Rich text notes editor
- [ ] Attach notes to leads/opportunities/customers
- [ ] Note categories/tags
- [ ] Pinned important notes
- [ ] Note search

### 5.2 File Attachments
- [ ] Upload files to records
- [ ] File preview for common types
- [ ] File versioning
- [ ] Download/delete files
- [ ] Storage management

### 5.3 Team Collaboration
- [ ] @mention team members in notes
- [ ] Notification on mention
- [ ] Comment threads
- [ ] Share records with team
- [ ] Collaborative editing

### 5.4 Activity Feed
- [ ] Record change history
- [ ] Who changed what and when
- [ ] Activity feed on detail pages
- [ ] Filter feed by action type

---

## Phase 6: Advanced Features 🔥
**Status**: FUTURE
**Goal**: Enterprise-level CRM capabilities

### 6.1 Sales Forecasting
- [ ] AI-powered win probability
- [ ] Revenue forecasting engine
- [ ] What-if scenario analysis
- [ ] Goal tracking and quotas

### 6.2 Territory Management
- [ ] Sales territories definition
- [ ] Territory assignment rules
- [ ] Territory performance dashboards
- [ ] Opportunity routing

### 6.3 Product Catalog
- [ ] Product and service catalog
- [ ] Pricing tiers
- [ ] Quote generation
- [ ] Product-based reporting

### 6.4 Custom Fields
- [ ] Add custom fields to entities
- [ ] Field type options (text, number, date, dropdown)
- [ ] Custom field validation
- [ ] Include in reports and searches

### 6.5 Workflow Automation
- [ ] Trigger-based workflows
- [ ] Automated email sequences
- [ ] Stage transition rules
- [ ] Task auto-creation

### 6.6 Mobile App
- [ ] Responsive web views
- [ ] Mobile-optimized layouts
- [ ] Offline capability
- [ ] Push notifications

---

## Technical Considerations

### Charting Library: ApexCharts.Blazor ⭐
**Why ApexCharts:**
- ✅ Native Blazor support
- ✅ Beautiful, modern charts
- ✅ Interactive (zoom, pan, drill-down)
- ✅ Responsive design
- ✅ Extensive chart types
- ✅ Active development & good docs
- ✅ Free and open source

**Alternatives:**
- Plotly.NET - More scientific, overkill for CRM
- ChartJS.Blazor - Simpler, less interactive
- Radzen Charts - Good but tied to Radzen ecosystem

### Data Strategy
- Use existing mock services for now
- Add aggregation methods to services
- Create report-specific DTOs
- Cache computed metrics (future)

### Performance
- Lazy load charts on scroll
- Debounce filter changes
- Paginate large datasets
- Add loading indicators

### UI/UX
- Follow MudBlazor design language
- Consistent color scheme (orange primary)
- Responsive grid layouts
- Print-friendly report views

---

## Next Steps
1. ✅ Create this roadmap document
2. ✅ Install ApexCharts.Blazor package
3. ✅ Create report service and DTOs
4. ✅ Build KPI dashboard with cards
5. ✅ Add data tables for all reports
6. ✅ Add date range filtering
7. ✅ Parallel data loading
8. ✅ Validate and test reports page
9. ✅ Research ApexCharts Blazor API for correct chart bindings
10. ✅ Convert tables to interactive charts
11. ✅ Fix namespace collisions and build successfully
12. ✅ Phase 2 (Activities & Timeline) core complete
13. ✅ Phase 3 (Sales Pipeline Kanban) core complete
14. ✅ Phase 3 polish complete (owner avatars, quick actions, owner/close-date filters)
15. ✅ Phase 4 (Email Templates & Communication) core complete
16. 🎯 **Next: Phase 5 (Notes & Collaboration)**

---

## Notes
- Prioritize visual impact and usability
- Get feedback after each phase
- Keep it simple, iterate and improve
- Document as we go
- Have fun! 🎉
