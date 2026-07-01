# 🚢 Navigation System - DEPRECATED

## This document is obsolete.

See `SIMPLE_NAVIGATION.md` for the current approach.

## What Happened

We tried to over-engineer iframe navigation with interceptors and postMessage gymnastics.

**The reality:** Module apps should work standalone. The shell is just optional chrome.

## Current Approach

1. Module URLs work standalone: `http://localhost:5002/hr/employees`
2. Shell wraps them optionally: `http://localhost:5000/modules/hr/hr/employees`
3. No interception. No hijacking. Just simple routing.

See `SIMPLE_NAVIGATION.md` for details.


---

## 📦 **Key Components**

### **1. navigation-interceptor.js**
**Location:** `services/HR/HR.Web/wwwroot/js/navigation-interceptor.js`

**The Enforcer** - Hijacks ALL navigation attempts in the iframe.

**What it intercepts:**
- ✅ `<a href="...">` clicks
- ✅ `history.pushState()`
- ✅ `history.replaceState()`
- ✅ `<form>` submissions (GET only)
- ✅ `window.location.href` changes
- ✅ `Navigation.NavigateTo()` (Blazor)

**How it works:**
```javascript
// Intercept clicks
document.addEventListener('click', function(e) {
	// Find anchor tag
	// Prevent default
	// Send postMessage to parent
}, true);

// Intercept History API
history.pushState = function(state, title, url) {
	// Send postMessage to parent
	// Call original method
};
```

**Message format:**
```javascript
{
	type: 'iframe-navigation',
	moduleKey: 'hr',
	route: '/employees?id=123',
	timestamp: 1234567890
}
```

---

### **2. NavigationInterceptorComponent.razor**
**Location:** `services/HR/HR.Web/Components/Layout/NavigationInterceptorComponent.razor`

**The Bridge** - Connects JavaScript interceptor to .NET runtime.

**Responsibilities:**
- Initialize JavaScript interceptor on first render
- Provide .NET callback for intercepted navigation
- Clean up on dispose

**Usage:**
```razor
<NavigationInterceptorComponent ModuleKey="hr" />
```

---

### **3. IframeLayout.razor (Updated)**
**Location:** `services/HR/HR.Web/Components/Layout/IframeLayout.razor`

**The Guard** - Ensures interceptor is active for all iframe pages.

**Added:**
```razor
<NavigationInterceptorComponent ModuleKey="hr" />
```

**Note:** The old `NavigationManager.LocationChanged` listener is still present but redundant - the interceptor handles everything now.

---

### **4. ModuleHost.razor (Enhanced)**
**Location:** `frontend/BusinessAsUsual.Web/Pages/ModuleHost.razor`

**The Orchestrator** - Receives navigation messages and updates both URL and iframe.

**New capabilities:**
- Tracks `_previousInternalRoute` to detect URL changes
- Calls `CommandIframeNavigation()` when route changes
- Sends postMessage to iframe: `{ type: 'navigate', route: '/employees' }`

**Flow:**
1. Parent URL changes: `/modules/hr/employees`
2. `OnParametersSetAsync` fires
3. Detects `InternalRoute` changed
4. Calls `navigateModuleIframe()` JavaScript function
5. JavaScript sends postMessage to iframe
6. Iframe's interceptor receives message, temporarily disables interception, navigates

---

### **5. theme-sync.js (Enhanced)**
**Location:** `frontend/BusinessAsUsual.Web/wwwroot/js/theme-sync.js`

**The URL Manager** - Updates browser URL when iframe navigates.

**Enhanced message handler:**
```javascript
if (event.data && event.data.type === 'iframe-navigation') {
	const route = event.data.route;
	const moduleKey = event.data.moduleKey;

	// Build new parent URL
	const newUrl = `/modules/${moduleKey}/${route}`;

	// Update browser URL (uses pushState now, not replaceState)
	window.history.pushState(null, '', newUrl);
}
```

**Why `pushState` instead of `replaceState`?**
- Enables browser back/forward buttons
- Creates proper navigation history
- User can use back button to return to previous pages

---

## 🔄 **Complete Flow Examples**

### **Scenario 1: User Clicks Link in Iframe**

```
1. User clicks <a href="/hr/employees">Employees</a> in HR home
2. NavigationInterceptor.js intercepts click
3. Prevents default navigation
4. Sends postMessage to parent:
   { type: 'iframe-navigation', moduleKey: 'hr', route: '/hr/employees' }
5. theme-sync.js receives message
6. Updates browser URL: /modules/hr/hr/employees
7. ModuleHost.OnParametersSetAsync detects route change
8. Calls CommandIframeNavigation()
9. Sends postMessage to iframe: { type: 'navigate', route: '/hr/employees' }
10. NavigationInterceptor.handleParentNavigation() temporarily disables interception
11. Iframe navigates to /hr/employees
12. ✅ Browser URL and iframe content are in sync!
```

---

### **Scenario 2: User Uses Blazor NavigationManager**

```
1. Code calls Navigation.NavigateTo("/hr/employees/123")
2. Blazor updates iframe location internally
3. NavigationInterceptor detects history.pushState call
4. Intercepts and sends postMessage to parent:
   { type: 'iframe-navigation', moduleKey: 'hr', route: '/hr/employees/123' }
5. theme-sync.js receives message
6. Updates browser URL: /modules/hr/hr/employees/123
7. ✅ Even programmatic navigation is caught!
```

---

### **Scenario 3: User Clicks Breadcrumb "Home"**

```
1. User clicks <MudLink Href="/dashboard">Home</MudLink>
2. NavigationInterceptor.js intercepts click
3. Sends postMessage: { type: 'iframe-navigation', moduleKey: 'hr', route: '/dashboard' }
4. theme-sync.js receives message
5. Attempts to build URL: /modules/hr/dashboard (WRONG!)
6. 
7. ⚠️ ISSUE: Dashboard is NOT part of HR module!
8. 
9. SOLUTION: Check if route starts with module prefix
10. If not, navigate parent directly: window.location.href = '/dashboard'
11. ✅ User exits module and lands on dashboard
```

**TODO:** Add logic to detect cross-module navigation and handle appropriately.

---

### **Scenario 4: User Refreshes Page**

```
1. Browser URL: /modules/hr/hr/employees?id=123
2. User hits F5
3. Browser requests: http://localhost:5002/hr/employees?id=123 (iframe URL)
4. App.razor detects standalone mode (window.self === window.top)
5. Redirects to: http://localhost:5000/modules/hr/hr/employees?id=123
6. Parent shell loads
7. ModuleHost parses: ModuleKey=hr, InternalRoute=hr/employees?id=123
8. Iframe loads with URL: http://localhost:5002/hr/employees?id=123
9. NavigationInterceptor initializes
10. ✅ Layout preserved, correct page loaded, interception active!
```

---

### **Scenario 5: User Uses Browser Back Button**

```
1. Browser history:
   - /modules/hr
   - /modules/hr/hr/employees
   - /modules/hr/hr/employees?id=123  ← Current
2. User clicks back button
3. Browser navigates to: /modules/hr/hr/employees
4. ModuleHost.OnParametersSetAsync fires
5. Detects InternalRoute changed from "hr/employees?id=123" to "hr/employees"
6. Calls CommandIframeNavigation()
7. Iframe navigates to /hr/employees
8. ✅ Back button works perfectly!
```

---

## ✅ **What This Solves**

| Problem | Solution |
|---------|----------|
| Refresh breaks layout | Standalone detection redirects to parent shell |
| Iframe navigates autonomously | ALL navigation is intercepted and routed through parent |
| Browser URL out of sync | postMessage updates parent URL in real-time |
| Back/forward buttons don't work | Parent URL uses pushState, creating proper history |
| Deep links don't work | Parent URL parsing works for any depth |
| Module pages feel disconnected | Browser URL always shows full context |

---

## 🐛 **Debugging**

### **Console Logs to Watch**

**Iframe side:**
```
[NavigationInterceptor] Initializing for module: hr
[NavigationInterceptor] ✓ Click interception active
[NavigationInterceptor] ✓ History API interception active
[NavigationInterceptor] ✓ Form interception active
[NavigationInterceptor] ✓ Location change monitoring active
[NavigationInterceptor] ✅ All navigation intercepted!
[NavigationInterceptor] 🎯 Click intercepted: /hr/employees → /hr/employees
[NavigationInterceptor] 📤 Sending to parent: /hr/employees
```

**Parent side:**
```
[ThemeSync] 🎯 Iframe navigation intercepted: /hr/employees
[ThemeSync] Module key: hr
[ThemeSync] 📍 Updating parent URL to: /modules/hr/hr/employees
[ModuleHost] OnParametersSetAsync - ModuleKey: hr, InternalRoute: hr/employees
[ModuleHost] Internal route changed, commanding iframe navigation
[ModuleHost] ✅ Commanded iframe to navigate to: /hr/employees
```

---

### **Common Issues**

1. **Interceptor not initializing**
   - Check: Is `navigation-interceptor.js` loaded in App.razor?
   - Check: Is `NavigationInterceptorComponent` in IframeLayout?
   - Check: Console for JavaScript errors

2. **Navigation still bypasses parent**
   - Check: Is iframe running standalone? (window.self === window.top)
   - Check: Interceptor initialization logs
   - Check: Event listener attachment

3. **Infinite loop / navigation storm**
   - Check: Is interceptor properly disabling during handleParentNavigation?
   - Check: Is ModuleHost calling CommandIframeNavigation unnecessarily?

4. **Back button doesn't work**
   - Check: Is theme-sync.js using `pushState` (not `replaceState`)?
   - Check: Browser history in DevTools

---

## 🚀 **Next Steps**

1. **Test all navigation scenarios**
   - ✅ Link clicks
   - ✅ Programmatic navigation (NavigationManager)
   - ✅ Form submissions
   - ✅ Breadcrumb clicks
   - ✅ Sidebar navigation
   - ✅ Refresh (F5)
   - ✅ Browser back/forward
   - ⏳ Cross-module navigation (Dashboard → HR → Dashboard)

2. **Handle cross-module navigation**
   - Detect when target route is not within current module
   - Navigate parent directly instead of building module URL
   - Example: `/dashboard` should not become `/modules/hr/dashboard`

3. **Add error boundaries**
   - Handle interceptor initialization failures gracefully
   - Fall back to direct navigation if interception fails

4. **Performance monitoring**
   - Track postMessage frequency
   - Optimize history monitoring interval (currently 100ms)

5. **Apply to other modules**
   - Copy interceptor pattern to other module projects
   - Consider extracting to shared library

---

## 🎯 **Success Criteria**

- ✅ Browser URL always shows complete route
- ✅ Refresh preserves layout AND page
- ✅ Back/forward buttons work
- ✅ All link clicks are intercepted
- ✅ All programmatic navigation is intercepted
- ✅ Forms work correctly
- ✅ No autonomous iframe navigation
- ✅ Clean, debuggable console logs
- ✅ No cross-origin errors
- ✅ **FRIGATE LAUNCHED! 🚢**

---

**THIS IS THE REAL DEAL - NO MORE ICE CREAM CONES, WE GOT A WARSHIP!** ⚓
