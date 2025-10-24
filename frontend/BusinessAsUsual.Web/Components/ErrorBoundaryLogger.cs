using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BusinessAsUsual.Web.Components
{
    /// <summary>
    /// Exposes Error Boundary error messages to display more than just a generic 'An error has occurred" message
    /// </summary>
    public class ErrorBoundaryLogger : ErrorBoundary
    {
        private RenderFragment? _childContent;

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

        /// <summary>
        /// Builds a new tree of elements to render the error message
        /// </summary>
        /// <param name="builder"></param>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (CurrentException is not null)
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "text-danger");

                builder.OpenElement(2, "h1");
                builder.AddContent(3, "Error Boundary Logger");
                builder.CloseElement();

                builder.OpenElement(4, "h4");
                builder.AddContent(5, "🔥 Unhandled Exception");
                builder.CloseElement();

                builder.OpenElement(6, "p");
                builder.AddContent(7, CurrentException.Message);
                builder.CloseElement();

                builder.OpenElement(8, "pre");
                builder.AddContent(9, CurrentException.StackTrace);
                builder.CloseElement();

                builder.CloseElement(); // div
            }
            else
            {
                builder.AddContent(10, _childContent);
            }
        }
    }
}