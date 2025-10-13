using BusinessAsUsual.Admin.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Components
{
    public class LegacyDataGridViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(IEnumerable<object> model, string gridId, string rowActions = "renderCompanyActions")
        {
            ViewData["GridId"] = gridId;
            ViewData["RowActions"] = rowActions;
            return Task.FromResult<IViewComponentResult>(View(model)); // Renders Views/Shared/Components/LegacyDataGrid/Default.cshtml
        }
    }
}