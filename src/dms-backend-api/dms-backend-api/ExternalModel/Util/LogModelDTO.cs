using Microsoft.Extensions.Logging;
using System;

namespace dms_backend_api.ExternalModel.Util
{
    public class LogModelDTO
    {
        public string LogTypeText { get; set; } = null!;

        private LogLevel logType;

        public LogLevel LogType
        {
            get { return logType; }
            set
            {
                Enum.TryParse(LogTypeText, true, out LogLevel parsedEnumValue);
                logType = parsedEnumValue;
            }
        }
        public string? LogMessage { get; set; }
        public object[]? LogParameters { get; set; }
    }
}
