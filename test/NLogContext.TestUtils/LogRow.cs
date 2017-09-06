using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLogContext.TestUtils
{
    public class LogRow
    {
        public long? Id { get; set; }
        public string dateTime { get; set; }
        public string ContextId { get; set; }
        public string ContextName { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public string Exception { get; set; }
        public string ParentContextId { get; set; }
        public string TopmostParentContextId { get; set; }

        /// <summary>
        /// TODO: refactor this to another class that inherits LogRow
        /// </summary>
        public string TheUsername { get; set; }
    }
}
