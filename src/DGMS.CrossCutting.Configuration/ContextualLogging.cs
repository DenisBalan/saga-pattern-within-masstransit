using Automatonymous;
using DGMS.CrossCutting.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGMS.CrossCutting.Configuration
{
    public class ContextualLogging: ILogger<ContextualLogging>
    {
        private readonly ILogger<ContextualLogging> logger;

        public ContextualLogging(ILogger<ContextualLogging> logger)
        {
            this.logger = logger;
        }
        public void LogIncomingMessage<TInstance, TData>(BehaviorContext<TInstance, TData> message)
        {
            var correlationId = message.Data switch
            {
                CorrelatedBy<Guid> correlatedBy => correlatedBy.CorrelationId.ToString(),
                Fault fault => fault.FaultedMessageId?.ToString(),
                _ => "unknown"
            };

            string json = JsonConvert.SerializeObject(message.Data, Formatting.Indented);
            var parentSagaId = (message.Instance.GetType().GetProperty(nameof(ICreateMultipleDocumentsSagaRequest.ParentSagaCorrelationId)).GetValue(message.Instance) as Guid?)?.ToString();

            correlationId = string.IsNullOrEmpty(parentSagaId) ? correlationId : $"{parentSagaId} -> {correlationId}";

            logger.LogDebug($"[{{CorrelationId}}] Received {{MessageType}}: {{Message}}",
                    correlationId, typeof(TData).Name, json);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
