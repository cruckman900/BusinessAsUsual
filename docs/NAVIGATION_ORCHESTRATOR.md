# Navigation Orchestrator Architecture - DEPRECATED

## This document is obsolete.

The "orchestrator" approach was over-engineered. We don't need to intercept and route every navigation event.

## New Approach

See `SIMPLE_NAVIGATION.md` for the current, simpler approach.

**Key insight:** Module apps should work standalone. The shell is just optional UI chrome that wraps them in an iframe when you want the sidebar/topbar.

No interception. No orchestration. Just simple URL routing.


```
User types or clicks: /modules/hr/employees?id=123
					  ↓
		 [Parent Shell - Master Router]
					  ↓
		 Parses: module=hr, route=employees?id=123
					  ↓
		 [MainLayout + ModuleHost Component]
					  ↓
		 Iframe loads: http://localhost:5002/employees?id=123
					  ↓
		 [Iframe navigates internally]
					  ↓
		 IframeLayout detects navigation → postMessage to parent
					  ↓
		 Parent updates browser URL via history.replaceState
```

## 📁 **Key Components**

### **1. ModuleRouteInterceptor.cs**
**Location:** `frontend/BusinessAsUsual.Web/Services/ModuleRouteInterceptor.cs`

**Purpose:** Master navigation service that parses module routes and orchestrates navigation.

**Key Methods:**
- `ParseModuleRoute(url)` - Parses URLs like `/modules/hr/employees` into `(moduleKey: "hr", internalRoute: "employees")`
- `BuildModuleUrl(moduleKey, internalRoute)` - Builds parent URLs: `"hr" + "/employees" → "/modules/hr/employees"`
- `NavigateToModule(moduleKey, internalRoute)` - Navigates the parent shell, which updates the iframe

**Registration:** Added to DI in `Program.cs` as scoped service

---

### **2. ModuleHost.razor**
**Location:** `frontend/BusinessAsUsual.Web/Pages/ModuleHost.razor`

**Route:** `@page "/modules/{ModuleKey}/{*InternalRoute}"`

**Purpose:** Hosts the iframe and translates parent URLs into iframe URLs.

**Parameters:**
- `ModuleKey` - The module identifier (e.g., "hr")
- `InternalRoute` - The catch-all internal route (e.g., "employees", "employees/123", "employees?id=123")

**Flow:**
1. Parent route: `/modules/hr/employees?id=123`
2. Parsed: `ModuleKey = "hr"`, `InternalRoute = "employees"`
3. Query preserved: `?id=123`
4. Iframe URL built: `http://localhost:5002/employees?id=123`

---

### **3. HR.Web App.razor - Standalone Detection**
**Location:** `services/HR/HR.Web/Components/App.razor`

**Purpose:** Detects when the iframe is accessed directly (refresh) and redirects to parent shell.

**Logic:**
```javascript
if (window.self !== window.top) {
	// Running in iframe - normal mode
} else {
	// Running standalone - REDIRECT
	const currentPath = window.location.pathname + window.location.search;
	const shellUrl = 'http://localhost:5000/modules/hr' + currentPath;
	window.location.replace(shellUrl);
}
```

**Example:**
- User refreshes while on `/hr/employees`
- Browser requests `http://localhost:5002/hr/employees`
- Script detects standalone mode
- Redirects to `http://localhost:5000/modules/hr/hr/employees`
- Parent shell loads with correct iframe

---

### **4. IframeLayout.razor - Navigation Sync**
**Location:** `services/HR/HR.Web/Components/Layout/IframeLayout.razor`

**Purpose:** Listens for iframe navigation changes and notifies parent shell to update browser URL.

**Event Flow:**
1. User navigates within iframe: `/hr/employees` → `/hr/employees/123`
2. `NavigationManager.LocationChanged` fires
3. `OnLocationChanged` method extracts route
4. Sends `postMessage` to parent: `{ type: 'iframe-navigation', route: '/hr/employees/123' }`
5. Parent shell receives message
6. Parent updates browser URL: `history.replaceState(null, '', '/modules/hr/hr/employees/123')`

---

### **5. theme-sync.js - Parent Message Handler**
**Location:** `frontend/BusinessAsUsual.Web/wwwroot/js/theme-sync.js`

**Purpose:** Listens for iframe navigation messages and updates parent URL.

**Key Code:**
```javascript
if (event.data && event.data.type === 'iframe-navigation') {
	const currentPath = window.location.pathname;
	const moduleMatch = currentPath.match(/^\/modules\/([^\/]+)/);

	if (moduleMatch) {
		const moduleKey = moduleMatch[1];
		const internalRoute = event.data.route.startsWith('/') 
			? event.data.route.substring(1) 
			: event.data.route;
		const newUrl = `/modules/${moduleKey}/${internalRoute}`;

		// Update browser URL WITHOUT reloading
		window.history.replaceState(null, '', newUrl);
	}
}
```

---

### **6. Sidebar.razor - Navigation via Parent URL**
**Location:** `frontend/BusinessAsUsual.Web/Components/Layout/Sections/Sidebar.razor`

**Purpose:** Sidebar navigation items navigate via parent URL (not iframe postMessage).

**Updated Logic:**
```csharp
private void NavigateInIframe(string route)
{
	if (CurrentModule != null)
	{
		var internalRoute = route.TrimStart('/');
		RouteInterceptor.NavigateToModule(CurrentModule.Key, internalRoute);
	}
}
```

**Flow:**
1. User clicks "Employees" in sidebar
2. Route: `/hr/employees`
3. `RouteInterceptor.NavigateToModule("hr", "hr/employees")`
4. Navigates to: `/modules/hr/hr/employees`
5. `ModuleHost` detects route change
6. Updates iframe URL: `http://localhost:5002/hr/employees`

---

## 🔄 **Complete Flow Examples**

### **Scenario 1: Normal Navigation**
```
1. User on Dashboard
2. Clicks "HR Module" → NavigateTo("/modules/hr")
3. ModuleHost renders with ModuleKey="hr", InternalRoute=null
4. Iframe loads: http://localhost:5002/
5. Browser URL: http://localhost:5000/modules/hr
```

### **Scenario 2: Sidebar Navigation**
```
1. User on HR home (/modules/hr)
2. Clicks "Employees" in sidebar
3. RouteInterceptor.NavigateToModule("hr", "hr/employees")
4. Parent navigates to: /modules/hr/hr/employees
5. ModuleHost detects parameter change
6. Updates iframe: http://localhost:5002/hr/employees
7. Browser URL: http://localhost:5000/modules/hr/hr/employees
```

### **Scenario 3: Iframe Internal Navigation**
```
1. User on Employees page (/modules/hr/hr/employees)
2. Clicks employee row → NavigationManager.NavigateTo("/hr/employees/123")
3. Iframe navigates to: http://localhost:5002/hr/employees/123
4. IframeLayout.OnLocationChanged fires
5. postMessage to parent: { type: 'iframe-navigation', route: '/hr/employees/123' }
6. Parent updates URL: /modules/hr/hr/employees/123
7. Browser URL stays in sync!
```

### **Scenario 4: Refresh/F5**
```
1. User on: http://localhost:5000/modules/hr/hr/employees?id=123
2. User hits F5
3. Browser requests: http://localhost:5002/hr/employees?id=123 (iframe URL)
4. HR.Web App.razor detects standalone: window.self === window.top
5. Redirects to: http://localhost:5000/modules/hr/hr/employees?id=123
6. Parent shell loads
7. ModuleHost parses: ModuleKey="hr", InternalRoute="hr/employees?id=123"
8. Iframe loads: http://localhost:5002/hr/employees?id=123
9. ✅ Layout preserved, correct page loaded!
```

### **Scenario 5: Breadcrumb "Home" Navigation**
```
1. User on Employees page inside HR iframe
2. Clicks "Home" breadcrumb
3. HR.Web sends: postMessage({ type: 'navigate-parent', route: '/dashboard' })
4. Parent shell theme-sync.js receives message
5. window.location.href = '/dashboard'
6. Full page reload to dashboard
7. ✅ User exits module cleanly
```

---

## 🎨 **URL Structure**

### **Parent Shell URLs (what user sees in browser)**
```
/dashboard                           → Dashboard page
/modules/{key}                       → Module home
/modules/{key}/{internalRoute}       → Module internal page
/modules/hr                          → HR home
/modules/hr/hr/employees             → HR employees list
/modules/hr/hr/employees?id=123      → HR employee detail
```

### **Iframe URLs (internal to module)**
```
http://localhost:5002/               → HR home
http://localhost:5002/hr/employees   → HR employees list
http://localhost:5002/hr/employees?id=123  → HR employee detail
```

### **Mapping Examples**
| Browser URL | Iframe URL |
|------------|-----------|
| `/modules/hr` | `http://localhost:5002/` |
| `/modules/hr/hr/employees` | `http://localhost:5002/hr/employees` |
| `/modules/hr/hr/employees?id=123` | `http://localhost:5002/hr/employees?id=123` |

---

## 🔧 **Configuration Notes**

### **Hardcoded Values to Replace**
- `http://localhost:5000` in `HR.Web/Components/App.razor` - should be configuration
- Module port numbers (`5002` for HR) - should come from Module Registry

### **Future Enhancements**
1. **Configuration Service** - Replace hardcoded shell URL with config
2. **Deep Linking** - Support direct navigation to deep module routes from external links
3. **Route Guards** - Add authentication/authorization checks at the parent level
4. **Analytics** - Track module navigation for usage metrics
5. **Lazy Loading** - Load module iframes only when needed, not on parent load

---

## 🐛 **Debugging**

### **Console Logs to Watch**
```
[ModuleHost] OnInitializedAsync - ModuleKey: hr, InternalRoute: hr/employees
[ModuleHost] Iframe URL: http://localhost:5002/hr/employees
[IframeLayout] Navigation changed to: /hr/employees
[ThemeSync] Iframe navigated to: /hr/employees
[ThemeSync] Updating parent URL to: /modules/hr/hr/employees
[Sidebar] Navigating to: /hr/employees
```

### **Common Issues**
1. **Browser URL not updating** - Check `theme-sync.js` message handler
2. **Iframe showing wrong page** - Check `ModuleHost.razor` URL building logic
3. **Refresh breaks layout** - Verify `App.razor` standalone detection
4. **Sidebar not highlighting** - Ensure `OnIframeNavigation` callback is registered

---

## ✅ **Success Criteria**

- ✅ Browser URL always shows complete route
- ✅ Refresh preserves both layout AND page
- ✅ Back/forward buttons work correctly
- ✅ Deep links work from external sources
- ✅ Module navigation updates parent URL in real-time
- ✅ No cross-origin security errors
- ✅ Clean separation of concerns (parent owns navigation, iframe renders content)

---

## 🚀 **Next Steps**

1. **Test all navigation scenarios**
2. **Replace hardcoded URLs with configuration**
3. **Add route guards/authentication**
4. **Implement for additional modules (beyond HR)**
5. **Add error boundaries for iframe load failures**
6. **Document for other developers**

**THIS IS THE PIONEER APPARATUS! 🎯**
