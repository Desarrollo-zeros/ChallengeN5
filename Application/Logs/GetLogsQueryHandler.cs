using AutoMapper;
using Domain.Interface.ElasticsearchServices;
using Domain.Interface.KafkaServices;
using Domain.Interface.Permissions;
using Domain.Permissions;
using Infrastructure.Audilog;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Logs.Querys
{
    public class GetLogsQueryHandler : IRequestHandler<GetLogsQuery, IEnumerable<LogDto>>, IGetLogsQueryHandler
    {

        private readonly IKafkaConsumer _kafkaConsumer;
        public GetLogsQueryHandler(IKafkaConsumer kafkaConsumer)
        {
            _kafkaConsumer = kafkaConsumer;

        }

        public async Task<IEnumerable<LogDto>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
        {
            return await _kafkaConsumer.ConsumeAsync<LogDto>("log");
        }
    }
}
