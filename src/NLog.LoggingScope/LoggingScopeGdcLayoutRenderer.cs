using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Layouts;

namespace NLog.LoggingScope
{
    [LayoutRenderer("logging-scope-gdc")]
    public class LoggingScopeGdcLayoutRenderer : LayoutRenderers.GdcLayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var contextItem = Item + ":" + Layouts.ScopeIdLayout.Render(logEvent);
            string value = GlobalDiagnosticsContext.Get(contextItem);
            builder.Append(value);
        }
    }
}