using BusinessAsUsual.Admin.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Components
{
    /// <summary>
    /// component to render a legacy data grid with customizable row actions.
    /// </summary>
    public class LegacyDataGridViewComponent : ViewComponent
    {
        /// <summary>
        /// invokes the LegacyDataGridViewComponent to render a data grid.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="gridId"></param>
        /// <param name="rowActions"></param>
        /// <returns></returns>
        public Task<IViewComponentResult> InvokeAsync(IEnumerable<object> model, string gridId, string rowActions = "renderCompanyActions")
        {
            ViewData["GridId"] = gridId;
            ViewData["RowActions"] = rowActions;
            return Task.FromResult<IViewComponentResult>(View(model)); // Renders Views/Shared/Components/LegacyDataGrid/Default.cshtml
        }
    }
}