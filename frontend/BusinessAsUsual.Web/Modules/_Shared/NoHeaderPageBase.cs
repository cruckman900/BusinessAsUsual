using Microsoft.AspNetCore.Components;

namespace BusinessAsUsual.Web.Modules._Shared
{
    public abstract class NoHeaderPageBase : ComponentBase
    {
        [Inject] public PageHeaderService HeaderService { get; set; } = default!;

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                HeaderService.SetHeader(null);
            }
        }
    }
}
