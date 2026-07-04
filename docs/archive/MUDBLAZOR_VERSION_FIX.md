# MudBlazor Version Alignment Fix

## Issue
Runtime `MissingMethodException` when HR.Web (iframe) tried to access `BusinessAsUsual.Web.Themes.ThemeRegistry`:

```
MissingMethodException: Method not found: 
'Void MudBlazor.MudTheme.set_PaletteLight(MudBlazor.PaletteLight)'
```

## Root Cause
**Version mismatch** between projects:
- `BusinessAsUsual.Web` → MudBlazor **8.15.0**
- `HR.Web` → MudBlazor **9.3.0**

When `HR.Web` referenced `BusinessAsUsual.Web` for shared `ThemeRegistry`, it loaded themes compiled against MudBlazor 8.x but tried to use them with MudBlazor 9.x APIs, causing the method signature mismatch.

## Breaking Changes in MudBlazor 9.x
MudBlazor 9.0+ introduced several breaking changes:

### 1. Generic Type Parameters Required
Components like `MudList`, `MudListItem`, `MudChip`, `MudSelect` now require explicit `T` type parameter:

**Before (v8):**
```razor
<MudList Dense="true">
	<MudListItem Icon="@Icons.Material.Filled.Check">Item</MudListItem>
</MudList>
<MudChip Size="Size.Small">Tag</MudChip>
```

**After (v9):**
```razor
<MudList T="string" Dense="true">
	<MudListItem T="string" Icon="@Icons.Material.Filled.Check">Item</MudListItem>
</MudList>
<MudChip T="string" Size="Size.Small">Tag</MudChip>
```

### 2. MudTheme API Changes
The `MudTheme` class property setters were modified, breaking binary compatibility.

### 3. Other Component Changes
- `MudTable<T>` has new required parameters
- `MudAutocomplete<T>` search delegate signature changed
- Some color palette properties reorganized

## Solution Applied

### 1. Upgraded BusinessAsUsual.Web
**File:** `frontend/BusinessAsUsual.Web/BusinessAsUsual.Web.csproj`

**Changed:**
```xml
<PackageReference Include="MudBlazor" Version="8.15.0" />
```
**To:**
```xml
<PackageReference Include="MudBlazor" Version="9.3.0" />
```

### 2. Verified Existing Code Compatibility
Checked that components in `BusinessAsUsual.Web` already had `T` parameters added (likely from a previous partial update).

**Example from `About.razor`:**
```razor
<MudList T="string" Dense="true">
	<MudListItem><MudText>Item text</MudText></MudListItem>
</MudList>
```

### 3. Previously Fixed HR.Web Components
During the theme sync implementation, we already fixed HR.Web components:
- `services/HR/HR.Web/Components/Pages/Home.razor`
- `services/HR/HR.Web/Components/Pages/Employees.razor`

All `MudChip`, `MudList`, `MudListItem` components now have `T="string"`.

## Verification

### Build Status
✅ **All projects build successfully**

```powershell
dotnet restore frontend\BusinessAsUsual.Web\BusinessAsUsual.Web.csproj
dotnet build
```

Result: **Build successful** (17 projects)

### Runtime Test
To verify the fix:
1. Start `BusinessAsUsual.Web` on port 5269
2. Start `HR.Web` on port 5002
3. Navigate to HR module (loaded in iframe)
4. Verify theme loads without `MissingMethodException`

Expected: Theme colors from `ThemeRegistry` apply correctly in iframe.

## Projects Affected

| Project | Old Version | New Version | Status |
|---------|-------------|-------------|--------|
| BusinessAsUsual.Web | 8.15.0 | 9.3.0 | ✅ Updated |
| HR.Web | 9.3.0 | 9.3.0 | ✅ Already correct |
| BusinessAsUsual.Admin | N/A | N/A | ℹ️ Doesn't use MudBlazor |

## Migration Checklist for Future Modules

When creating new microservice UI projects:

- [ ] Use MudBlazor **9.3.0** (or latest 9.x)
- [ ] Add `T` type parameters to generic components:
  - `MudList<T>`
  - `MudListItem<T>`
  - `MudChip<T>`
  - `MudSelect<T>`
  - `MudAutocomplete<T>`
  - `MudTable<T>`
- [ ] Reference `BusinessAsUsual.Web` for shared `ThemeRegistry`
- [ ] Test theme sync in iframe mode
- [ ] Verify no `MissingMethodException` at runtime

## Related Documentation
- `docs/THEME_SYNC_IMPLEMENTATION.md` - Theme synchronization architecture
- `docs/HANDOVER_DOCUMENT.md` - Overall architecture guide
- [MudBlazor v9 Migration Guide](https://mudblazor.com/docs/migration/v9-migration)

## Lessons Learned

1. **Always align dependency versions** across projects that share code
2. **MudBlazor 9.x is a major version** with breaking API changes
3. **Binary compatibility breaks** at runtime, not compile time, when mixing versions
4. **Test iframe scenarios** with shared assemblies to catch version conflicts early
5. **Document breaking changes** in upgrade notes for future developers

## Future Considerations

- [ ] Pin MudBlazor version in a `Directory.Build.props` for consistency
- [ ] Add dependency version checks to CI/CD pipeline
- [ ] Create upgrade guide for next MudBlazor major version
- [ ] Consider consolidating shared UI components in a separate library
