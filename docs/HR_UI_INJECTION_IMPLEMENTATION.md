# HR Module UI Injection - Implementation Guide

## Architecture Overview

We've implemented a **true micro-frontend architecture** where the HR module's UI is completely separate from the main web shell and loaded dynamically.

```
┌─────────────────────────────────────────────────────┐
│  Main Web Shell (BusinessAsUsual.Web)              │
│  Port: 5269                                         │
│  ┌───────────────────────────────────────────────┐ │
│  │  Navigation (from Module Registry)            │ │
│  │  - HR link points to: /hr → iframe loader     │ │
│  └───────────────────────────────────────────────┘ │
│  ┌───────────────────────────────────────────────┐ │
│  │  <iframe src="http://localhost:5002/hr">      │ │
│  │    ┌────────────────────────────────────────┐ │ │
│  │    │ HR.Web (Blazor Server)                 │ │ │
│  │    │ - Employee List                        │ │ │
│  │    │ - Employee Detail                      │ │ │
│  │    │ - Department Management                │ │ │
│  │    └────────────────────────────────────────┘ │ │
│  │  </iframe>                                     │ │
│  └───────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
		  │                              │
		  ▼                              ▼
┌──────────────────┐           ┌──────────────────┐
│ Module Registry  │           │ HR.API           │
│ Port: 5100       │           │ Port: 5041       │
│ - Module metadata│           │ - REST API       │
│ - UI entry points│           │ - Mobile contracts│
└──────────────────┘           └──────────────────┘
```

## What We Built

### 1. HR.Web Project ✅
**Location**: `services/HR/HR.Web`  
**Type**: Blazor Server Application  
**Port**: 5002  

**Features**:
- Standalone Blazor application
- Can run independently
- Configured for iframe embedding (CORS)
- Uses in-memory database for development
- Includes sample data

**Pages Created**:
- `/hr` - HR Module home dashboard
- `/hr/employees` - Employee list with search/filter
- Future: `/hr/departments`, `/hr/onboarding`, `/hr/benefits`

### 2. Module Registration Updated ✅
HR.API now registers with:
```json
{
  "uiEntryPoint": "http://localhost:5002/hr"
}
```

When the main web shell discovers this module, it knows where to load the UI from.

### 3. CORS Configuration ✅
HR.Web is configured to allow embedding from the main web shell:
```csharp
policy.WithOrigins(
	"http://localhost:5269",  // Main web shell
	"https://localhost:7229"
)
.AllowCredentials();
```

## How It Works

### Step 1: Module Discovery
1. User opens main web app (`http://localhost:5269`)
2. Web shell calls Module Registry (`http://localhost:5100/api/modules/ui`)
3. Registry returns HR module metadata including `uiEntryPoint`

### Step 2: Navigation Rendering
Main web shell generates navigation:
```html
<nav>
  <a href="/hr">Human Resources</a>
</nav>
```

### Step 3: UI Injection (Next Step - See Below)
When user clicks "Human Resources":
- Web shell route `/hr` renders an iframe loader component
- Iframe loads `http://localhost:5002/hr`
- HR.Web serves its Blazor UI
- User interacts with HR module inside the iframe

## What Still Needs to Be Done

### Main Web Shell Iframe Loader
The main web app (`BusinessAsUsual.Web`) needs a component to load the iframe:

```razor
@* File: frontend/BusinessAsUsual.Web/Components/Pages/ModuleLoader.razor *@
@page "/hr"
@page "/hr/{*path}"

<div class="module-container">
	<iframe src="http://localhost:5002/hr/@Path" 
			class="module-iframe"
			frameborder="0">
	</iframe>
</div>

<style>
	.module-container {
		width: 100%;
		height: calc(100vh - 60px); /* Adjust for your header */
	}

	.module-iframe {
		width: 100%;
		height: 100%;
		border: none;
	}
</style>

@code {
	[Parameter]
	public string? Path { get; set; }
}
```

**OR** for a more dynamic approach:

```razor
@* File: frontend/BusinessAsUsual.Web/Components/Pages/ModuleLoader.razor *@
@page "/{moduleId}/{*path}"
@inject IModuleDiscoveryService ModuleDiscovery

<div class="module-container">
	@if (!string.IsNullOrEmpty(moduleUrl))
	{
		<iframe src="@moduleUrl" 
				class="module-iframe"
				frameborder="0">
		</iframe>
	}
	else
	{
		<div class="alert alert-warning">
			Module not found or not available.
		</div>
	}
</div>

@code {
	[Parameter]
	public string ModuleId { get; set; } = "";

	[Parameter]
	public string? Path { get; set; }

	private string? moduleUrl;

	protected override async Task OnInitializedAsync()
	{
		var modules = await ModuleDiscovery.GetModulesWithUiAsync();
		var module = modules.FirstOrDefault(m => m.ModuleId == ModuleId);

		if (module != null && !string.IsNullOrEmpty(module.UiEntryPoint))
		{
			moduleUrl = Path != null 
				? $"{module.UiEntryPoint}/{Path}"
				: module.UiEntryPoint;
		}
	}
}
```

## Running the Complete System

### Required Services (4 total)

1. **Module Registry** - `services/ModuleRegistry/ModuleRegistry.API` (Port 5100)
2. **HR API** - `services/HR/HR.API` (Port 5041)
3. **HR Web UI** - `services/HR/HR.Web` (Port 5002) **← NEW!**
4. **Main Web Shell** - `frontend/BusinessAsUsual.Web` (Port 5269)

### Visual Studio Setup

1. Right-click solution → Properties → Startup Project
2. Select "Multiple startup projects"
3. Set these to **Start**:
   - `ModuleRegistry.API`
   - `HR.API`
   - `HR.Web` **← ADD THIS**
   - `BusinessAsUsual.Web`

### Startup Sequence

The services will start in this order:

```
1. ModuleRegistry.API starts
   ✓ In-memory database ready
   ✓ Now listening on: http://localhost:5100

2. HR.API starts
   ✓ In-memory database ready
   ✓ Successfully registered with Module Registry
   ✓ Now listening on: http://localhost:5041

3. HR.Web starts
   ✓ In-memory database ready
   ✓ Seeded sample HR data
   ✓ HR Web UI ready on http://localhost:5002

4. BusinessAsUsual.Web starts
   ✓ Discovered 1 module from Module Registry
   ✓ Now listening on: http://localhost:5269
```

## Testing the UI Injection

### Test 1: HR.Web Standalone
Visit `http://localhost:5002/hr` directly:
- Should see HR home dashboard
- Should see sample employees when clicking "Employees"

### Test 2: Main Web Shell
Visit `http://localhost:5269`:
- Should see "Human Resources" in navigation
- Click it → should load HR UI in iframe (once iframe loader is added)

### Test 3: Module Registry
```powershell
Invoke-WebRequest -Uri http://localhost:5100/api/modules/ui -UseBasicParsing
```

Should show:
```json
{
  "moduleId": "hr",
  "displayName": "Human Resources",
  "uiEntryPoint": "http://localhost:5002/hr",
  "supportsMobile": true
}
```

## Benefits of This Architecture

### ✅ True Independence
- HR team can develop/deploy independently
- Different Blazor/framework versions possible
- Isolated failures (HR crash won't crash main shell)

### ✅ Dynamic Discovery
- Module appears/disappears based on registry
- Display name, icon, permissions all dynamic
- Can enable/disable modules without redeploying main app

### ✅ Consistent with Mobile
- Same registry drives both web and mobile
- Mobile apps already consuming contracts
- One source of truth for all clients

### ✅ Scalable
- Add Finance, CRM, Inventory modules the same way
- Each module can scale independently
- Can deploy modules to different servers

## Challenges & Solutions

### Challenge 1: iframe Communication
**Problem**: Main shell needs to communicate with iframe (e.g., for navigation, auth)

**Solution**: Use `postMessage` API or consider:
- Module Federation (Webpack 5)
- Single-SPA framework
- Micro-frontend router libraries

### Challenge 2: Shared Styles
**Problem**: Each module may have different look/feel

**Solution**: 
- Create shared design system package
- Reference it in both main shell and modules
- Or embrace different styles as feature (module branding)

### Challenge 3: Authentication
**Problem**: User needs to be authenticated in both shell and iframe

**Solution**:
- Pass JWT token via URL or postMessage
- Shared authentication cookie domain
- SSO pattern with central auth service

## Next Steps

1. **Add iframe loader to main web shell** (see code samples above)
2. **Test the complete flow** end-to-end
3. **Add more HR pages** (Departments, Employee Detail, Edit)
4. **Implement auth token passing** if needed
5. **Add Finance module** following the same pattern

## Related Files

- `services/HR/HR.Web/Program.cs` - Service configuration
- `services/HR/HR.Web/Components/Pages/Home.razor` - HR dashboard
- `services/HR/HR.Web/Components/Pages/Employees.razor` - Employee list
- `services/HR/HR.Application/Services/ModuleRegistrationService.cs` - Registration with UI entry point
- `services/HR/HR.Web/Properties/launchSettings.json` - Port 5002 configuration

## Success Criteria

- [ ] All 4 services start without errors
- [ ] HR.Web accessible at http://localhost:5002/hr
- [ ] Main web shell shows HR in navigation
- [ ] Clicking HR loads iframe with HR.Web content
- [ ] Employee list shows sample data
- [ ] Mobile contracts still working at http://localhost:5041/api/hr/mobile/ui-spec

🎉 **You now have a true micro-frontend architecture with UI injection!**
