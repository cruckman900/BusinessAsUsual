using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;

namespace BusinessAsUsual.Web.Components
{
    /// <summary>
    /// Catches unhandled render-tree exceptions and renders a friendly, on-brand error page
    /// instead of a raw stack trace. Tenant-resolution failures (<see cref="InvalidOperationException"/>
    /// thrown by <c>TenantContext</c> when the tenant hasn't been resolved for this
    /// circuit/session) get a targeted "sign in again" experience via
    /// <see cref="TenantContextError"/>; all other unhandled exceptions fall back to the
    /// generic <see cref="UnhandledError"/> page. Technical details (message/stack trace)
    /// are only shown when running in the Development environment.
    /// </summary>
    public class ErrorBoundaryLogger : ErrorBoundary
    {
        private RenderFragment? _childContent;

        [Inject] private IWebHostEnvironment? Environment { get; set; }

        /// <summary>
        /// Overrides the OnError task to bypass the silent error details state
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected override Task OnErrorAsync(Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"🔥 ErrorBoundaryLogger caught: {exception.Message}");
            return Task.CompletedTask; // Skip base to suppress default rendering
        }

        /// <summary>
        /// Overrides the SetParameters task to get values of ChildContent
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);
            parameters.TryGetValue(nameof(ChildContent), out _childContent);
            return base.SetParametersAsync(ParameterView.Empty);
        }

        private static bool IsTenantResolutionError(Exception ex) =>
            ex is InvalidOperationException
            && ex.Message.Contains("Tenant context has not been resolved", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Builds a new tree of elements to render the error message
        /// </summary>
        /// <param name="builder"></param>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (CurrentException is not null)
            {
                var showDetails = Environment?.IsDevelopment() ?? false;
                var detail = $"{CurrentException.GetType().Name}: {CurrentException.Message}\n{CurrentException.StackTrace}";

                if (IsTenantResolutionError(CurrentException))
                {
                    builder.OpenComponent<TenantContextError>(0);
                    builder.AddAttribute(1, nameof(TenantContextError.Detail), detail);
                    builder.AddAttribute(2, nameof(TenantContextError.ShowDetails), showDetails);
                    builder.CloseComponent();
                }
                else
                {
                    builder.OpenComponent<UnhandledError>(3);
                    builder.AddAttribute(4, nameof(UnhandledError.Detail), detail);
                    builder.AddAttribute(5, nameof(UnhandledError.ShowDetails), showDetails);
                    builder.CloseComponent();
                }
            }
            else
            {
                builder.AddContent(6, _childContent);
            }
        }
    }
}