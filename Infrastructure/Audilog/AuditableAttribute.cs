using Azure;
using Domain.Interface.KafkaServices;
using Infrastructure.Middleware;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Audilog
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuditableAttribute : ActionFilterAttribute
    {
        private readonly IKafkaProducer _kafkaProducer;
        private LogDto _logDto;
        private string _operation;
        private readonly ILogger<AuditableAttribute> _logger;
        public AuditableAttribute(string operation, IKafkaProducer kafkaProducer, ILogger<AuditableAttribute> logger)
        {
            _operation  = operation;
            _kafkaProducer = kafkaProducer;
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logDto = new LogDto
            {
                Operation = _operation
            };
            _logger.LogInformation("Creando LogDto");
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _kafkaProducer.ProduceAsync("log",_logDto, Guid.NewGuid().ToString());
            _logger.LogInformation("Guardando LogDto");
            base.OnActionExecuted(context);
        }


    }
}
