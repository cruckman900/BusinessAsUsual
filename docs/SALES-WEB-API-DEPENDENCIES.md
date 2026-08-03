# Sales Web Pickers - API Dependencies

## Required Running Services

For the **ProductPicker** and **CustomerPicker** to work in Sales.Web, you need these APIs running:

### 1. Inventory API (for Products)
**Port:** `http://localhost:5142`  
**Endpoint:** `GET /api/inventory/products`  
**Project:** `services/Inventory/Inventory.API/Inventory.API.csproj`

**To Run:**
```powershell
cd "D:\DotNet Projects\BusinessAsUsual\services\Inventory\Inventory.API"
dotnet run
```

### 2. CRM API (for Customers)
**Port:** `http://localhost:5004`  
**Endpoint:** `GET /api/customers`  
**Project:** `services/CRM/CRM.API/CRM.API.csproj`

**To Run:**
```powershell
cd "D:\DotNet Projects\BusinessAsUsual\services\CRM\CRM.API"
dotnet run
```

### 3. Sales Web (the UI)
**Port:** (configured in launchSettings)  
**Project:** `services/Sales/Sales.Web/Sales.Web.csproj`

**To Run:**
```powershell
cd "D:\DotNet Projects\BusinessAsUsual\services\Sales\Sales.Web"
dotnet run
```

## Configuration

### Sales.Web/Program.cs
Now configured with these named HttpClients:

```csharp
// Inventory API - defaults to http://localhost:5142
builder.Services.AddHttpClient("InventoryApi", client =>
{
	client.BaseAddress = new Uri(inventoryApiUrl);
});

// CRM API - defaults to http://localhost:5004
builder.Services.AddHttpClient("CrmApi", client =>
{
	client.BaseAddress = new Uri(crmApiUrl);
});
```

### Override Defaults (Optional)
Add to `Sales.Web/appsettings.Development.json`:

```json
{
  "InventoryApi": {
	"Url": "http://localhost:5142"
  },
  "CrmApi": {
	"Url": "http://localhost:5004"
  }
}
```

## How to Test All Three Together

### Option 1: Visual Studio Multiple Startup Projects
1. Right-click Solution → **Set Startup Projects**
2. Select **Multiple startup projects**
3. Set these to **Start**:
   - `Inventory.API`
   - `CRM.API`
   - `Sales.Web`
4. Click **Start**

### Option 2: Run in Separate Terminals
Open 3 PowerShell terminals:

**Terminal 1 - Inventory API:**
```powershell
cd "D:\DotNet Projects\BusinessAsUsual\services\Inventory\Inventory.API"
dotnet run
```

**Terminal 2 - CRM API:**
```powershell
cd "D:\DotNet Projects\BusinessAsUsual\services\CRM\CRM.API"
dotnet run
```

**Terminal 3 - Sales Web:**
```powershell
cd "D:\DotNet Projects\BusinessAsUsual\services\Sales\Sales.Web"
dotnet run
```

### Option 3: Use the Main Web Shell (BusinessAsUsual.Web)
The main BusinessAsUsual.Web app already runs all APIs in-process via module registration. Just run:

```powershell
cd "D:\DotNet Projects\BusinessAsUsual\frontend\BusinessAsUsual.Web"
dotnet run
```

Then navigate to the Sales module in the shell.

## Verification

### Check if APIs are running:

**Inventory API:**
```
http://localhost:5142/api/inventory/products
```
Should return JSON array of products.

**CRM API:**
```
http://localhost:5004/api/customers
```
Should return JSON array of customers.

### Expected Product Response (Inventory)
```json
[
  {
	"id": "guid-here",
	"name": "Dell Latitude 5420 Laptop",
	"sku": "LAP-DELL-5420",
	"description": "14-inch business laptop...",
	"price": 1299.99,
	"category": "Laptops"
  },
  ...
]
```

### Expected Customer Response (CRM)
```json
[
  {
	"id": "guid-here",
	"name": "Acme Corporation",
	"email": "contact@acme.com",
	"phone": "(555) 123-4567",
	"company": "Acme Corp",
	"industry": "Manufacturing"
  },
  ...
]
```

## Troubleshooting

### Pickers show "Loading..." forever
- **Cause:** APIs not running or wrong ports
- **Fix:** Verify APIs are running at correct ports
- **Check:** Open browser to API URLs above

### Pickers show "No products/customers available"
- **Cause:** APIs running but returning empty arrays
- **Fix:** Check if seed data was initialized
- **Solution:** Stop API, delete database (if using InMemory), restart

### CORS errors in browser console
- **Cause:** Sales.Web can't access API due to CORS
- **Fix:** Check that APIs have CORS configured for Sales.Web origin
- **Note:** Default is "AllowAll" in development mode

### Different port numbers?
If your APIs run on different ports:
1. Check `launchSettings.json` in each API project
2. Update `Sales.Web/Program.cs` default URLs
3. Or add overrides to `appsettings.Development.json`

## Summary

✅ **Updated:** Sales.Web/Program.cs with InventoryApi and CrmApi HttpClients  
✅ **Fixed:** ProductPicker endpoint to `api/inventory/products`  
✅ **Correct:** CustomerPicker endpoint is `api/customers`  
✅ **Next:** Run Inventory.API + CRM.API + Sales.Web together  

Once all three are running, the autocomplete pickers will fetch real data!
