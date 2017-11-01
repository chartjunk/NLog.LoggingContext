using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Layouts;

namespace NLog.LoggingContext
{
    [LayoutRenderer("logging-context-gdc")]
    public class LoggingContextGdcLayoutRenderer : LayoutRenderers.GdcLayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var contextItem = Item + ":" + Layouts.ContextIdLayout.Render(logEvent);
            string value = GlobalDiagnosticsContext.Get(contextItem);
            builder.Append(value);
        }
    }
}