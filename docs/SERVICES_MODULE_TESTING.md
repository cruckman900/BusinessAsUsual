# Services Module - Testing & Validation Checklist

## ✅ Fixes Applied

### 1. DTO Field Mismatch Fixed
**Issue:** Home.razor expected `Price` and `DurationMinutes` fields that don't exist in API
**Fix:** Updated ServiceDto in Home.razor to match actual API DTO:
- Changed `Price` → `BasePrice`
- Removed `DurationMinutes` field
- Made `Description` nullable

### 2. Configuration Verified
- ✅ **Port Registry:** Services.API port 7286 documented in `docs/PORT_REGISTRY.md`
- ✅ **Shell HttpClient:** Registered in `frontend/BusinessAsUsual.Web/Program.cs` line 138
- ✅ **Module Discovery:** Services added to `ModuleDiscoveryService.cs` fallback list
- ✅ **Shell Assembly:** Services.Web included in `App.razor` AdditionalAssemblies
- ✅ **MainLayout Route:** Services route added to `MainLayout.razor.cs` UpdateModuleFromUri (line ~193)
- ✅ **API Route:** Lowercase explicit route `[Route("api/services")]`
- ✅ **HttpClient Usage:** All pages use `IHttpClientFactory` (not bare HttpClient)

### 3. Layout Integration
- ✅ Services pages will use **shell's MainLayout** when loaded through shell
- ✅ Dashboard follows Finance/Inventory pattern exactly
- ✅ Submodule cards match Finance structure (centered, MudCardContent + MudCardActions)

---

## 🧪 Testing Checklist

### Pre-Test Setup
1. **Start Required Projects** (Visual Studio → Solution Properties → Multiple Startup Projects):
   - ✅ `Services.API` (port 7286)
   - ✅ `BusinessAsUsual.Web` (shell, port 5269)
   - ✅ `ModuleRegistry.API` (optional, port 5100)

2. **Verify Services.API is Running**:
   - Open browser to `http://localhost:7286/api/services`
   - Should return JSON array (empty `[]` or with seed data)

### Test 1: Module Navigation
1. Navigate to `http://localhost:5269` (shell)
2. Click **Services** in the sidebar
3. ✅ Should navigate to `/services` (Services dashboard)
4. ✅ Should see shell's MainLayout (top bar, **SIDEBAR**, footer)
5. ✅ Sidebar should show Services navigation items
6. ✅ Should NOT see plain/missing layout or missing sidebar

### Test 2: Dashboard Insights
1. On Services dashboard (`/services`)
2. ✅ Should see **4 insight cards** at top:
   - Total Services
   - Active Services
   - Avg. Service Price
   - Total Revenue Potential
3. ✅ Cards should show **numbers** (not loading spinner)
4. ✅ If API is down, should show **error alert** (not crash)

### Test 3: Submodule Cards
1. On Services dashboard
2. ✅ Should see **section heading**: "Service Management"
3. ✅ Should see **3 submodule cards**:
   - Service Catalog (active, with metric chip)
   - Appointments (disabled, "Coming Soon")
   - Service Providers (disabled, "Coming Soon")
4. ✅ Cards should be **center-aligned** (icon top, then title, description, chip)
5. ✅ Active card should have **hover effect** (lift up)
6. ✅ Button text should have **arrow →**

### Test 4: Quick Actions Section
1. On Services dashboard, scroll down
2. ✅ Should see **2-column grid**: "Quick Actions" + "Alerts & Notifications"
3. ✅ Quick Actions should have:
   - Create New Service (active)
   - Schedule Appointment (disabled)
   - Add Service Provider (disabled)
4. ✅ Alerts should show dynamic message based on active services count

### Test 5: About Module Section
1. On Services dashboard, scroll to bottom
2. ✅ Should see **2-column grid**:
   - Left (8 cols): "About Services Module" with feature list
   - Right (4 cols): "Module Info" with metadata
3. ✅ Module Info should show:
   - Module ID: `services`
   - Version: `1.0.0`
   - API Port: `7286 (HTTP) / 7285 (HTTPS)`
   - Web UI Port: `5009`
   - Mobile Support: Yes (green chip)
   - Status: Active (green chip)

### Test 6: Service Catalog Navigation
1. Click **"Manage Services →"** button on Service Catalog card
2. ✅ Should navigate to `/services/list`
3. ✅ Should see CustomDataGrid with services
4. ✅ Grid should have: Name, Description, Price, Active, Actions columns
5. ✅ Should see Create Service button

### Test 7: CRUD Operations
1. Click **"Create Service"**
   - ✅ Navigate to `/services/create`
   - ✅ Form should render with all fields
   - ✅ Submit should work and show snackbar
2. Click **Edit** on a service
   - ✅ Navigate to `/services/edit/{id}`
   - ✅ Form should populate with existing data
3. Click **Details** on a service
   - ✅ Navigate to `/services/details/{id}`
   - ✅ Should show read-only view with breadcrumbs
4. Click **Delete** on a service
   - ✅ Navigate to `/services/delete/{id}`
   - ✅ Should show warning alert and confirmation

### Test 8: Loading States
1. **Slow/Failed API Test:**
   - Stop Services.API
   - Navigate to `/services`
   - ✅ Should show loading spinner briefly
   - ✅ After 2 seconds, should show error alert
   - ✅ Should NOT crash or show blank page

### Test 9: Responsive Design
1. Resize browser window
2. ✅ Insight cards should stack on mobile (1 column)
3. ✅ Submodule cards should adjust: 4 cols (lg) → 3 (md) → 2 (sm) → 1 (xs)
4. ✅ Quick Actions/Alerts should stack on mobile

### Test 10: Visual Consistency
Compare Services dashboard to Finance and Inventory:
- ✅ Breadcrumb format matches
- ✅ Page title format matches (icon + text)
- ✅ Card spacing matches
- ✅ Button styles match
- ✅ About section layout matches
- ✅ Module Info layout matches

---

## 🐛 Known Issues & Workarounds

### Issue: API Connection Refused
**Symptoms:** Dashboard shows error "Failed to load dashboard data: No connection could be made..."  
**Cause:** Services.API not running or wrong port  
**Fix:**
1. Verify Services.API is in startup projects
2. Check `docs/PORT_REGISTRY.md` for correct port
3. Ensure port 7286 isn't blocked or in use

### Issue: Module Not in Sidebar
**Symptoms:** Services doesn't appear in navigation  
**Cause:** ModuleDiscoveryService not updated OR shell not restarted  
**Fix:**
1. Check `ModuleDiscoveryService.cs` has Services in fallback list
2. Restart shell (BusinessAsUsual.Web)

### Issue: Layout Missing/Plain
**Symptoms:** Pages show content but no shell chrome (sidebar, header, footer)  
**Cause:** Module assembly not in App.razor AdditionalAssemblies  
**Fix:**
1. Check `frontend/BusinessAsUsual.Web/App.razor` line 30 has `typeof(global::Services.Web.Components.App).Assembly`
2. Rebuild shell project

### Issue: Sidebar Missing (but header/footer show)
**Symptoms:** Top bar and footer render, but sidebar is hidden  
**Cause:** Module route not in MainLayout.razor.cs hardcoded routes  
**Fix:**
1. Open `frontend/BusinessAsUsual.Web/Components/Layout/MainLayout.razor.cs`
2. Find `UpdateModuleFromUri` method (line ~154)
3. Add your module to the hardcoded routes (line ~192):
```csharp
else if (path.StartsWith("/services"))
    _currentModule = "Services";
```
4. Rebuild shell project
5. Restart shell application

**Why this happens:** The sidebar only renders when `_currentModule is not null` (MainLayout.razor line 21). The `UpdateModuleFromUri` method sets `_currentModule` based on the URL path. If your module's route isn't in the hardcoded list, `_currentModule` stays null and the sidebar is hidden.

### Issue: Insight Cards Show Wrong Data
**Symptoms:** Cards show 0 or incorrect numbers  
**Cause:** DTO field mismatch or seeded data missing  
**Fix:**
1. Verify Services.API has seed data (check `Program.cs` DataSeeder)
2. Verify DTO fields match between API and Web (BasePrice vs Price)
3. Check browser console for JSON deserialization errors

---

## 📊 Success Criteria

### All tests pass ✅
- Dashboard loads with insights, cards, actions, and about section
- Navigation works for all routes
- CRUD operations function correctly
- Error handling works (loading states, error alerts)
- Visual consistency matches Finance/Inventory modules
- Responsive design works on all screen sizes

### Build Status ✅
- `Services.API` builds without errors
- `Services.Web` builds without errors
- `BusinessAsUsual.Web` (shell) builds without errors

### No Console Errors
- Browser console shows no errors
- Visual Studio Output window shows no unhandled exceptions

---

## 🚀 Next Steps After Testing

1. **If all tests pass:**
   - Document any additional features needed
   - Plan Appointments and Providers submodules
   - Update ModifyModule skill with lessons learned

2. **If tests fail:**
   - Capture error messages
   - Check browser console and VS Output window
   - Verify against PORT_REGISTRY.md
   - Consult `.github/skills/ModifyModule/SKILL.md` Common Issues section

---

## 📖 Related Documentation

- `docs/PORT_REGISTRY.md` - Authoritative port assignments
- `.github/skills/CreateModule/SKILL.md` - Module creation guide
- `.github/skills/ModifyModule/SKILL.md` - Module modification patterns
- Services dashboard reference: `services/Finance/Finance.Web/Components/Pages/Dashboard.razor`
