using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.KafkaServices
{
    public interface IKafkaProducer
    {
        Task ProduceAsync<T>(string topic, T value, string key = null);
    }
}
