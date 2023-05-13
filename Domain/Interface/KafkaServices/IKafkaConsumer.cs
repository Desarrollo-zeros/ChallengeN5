using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.KafkaServices
{
    public interface IKafkaConsumer
    {
        Task ConsumeAsync<T>(string topic, Action<T> onMessageReceived, string groupId = null);

        Task<IEnumerable<T>> ConsumeAsync<T>(string topic, string groupId = null);
    }
}
