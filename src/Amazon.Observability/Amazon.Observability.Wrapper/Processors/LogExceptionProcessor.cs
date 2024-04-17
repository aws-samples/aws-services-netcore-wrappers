using OpenTelemetry;
using OpenTelemetry.Logs;
using System.Linq;

namespace Amazon.Observability.Processors
{
    internal class LogExceptionProcessor : BaseProcessor<LogRecord>
    {
        private readonly string name;

        public LogExceptionProcessor(string name = "LogExceptionProcessor")
        {
            this.name = name;
        }

        public override void OnEnd(LogRecord record)
        {
            if (record.Exception != null)
            {
                record.Attributes = record!.Attributes!.Append(new KeyValuePair<string, object?>("exception.stacktrace", record!.Exception!.StackTrace)).ToList();
                record.Attributes = record!.Attributes!.Append(new KeyValuePair<string, object?>("exception.innerexception", record!.Exception!.InnerException!)).ToList();

            }
        }
    }

}
