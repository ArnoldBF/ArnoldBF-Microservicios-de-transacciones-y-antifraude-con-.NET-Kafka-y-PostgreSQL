using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IKafka
    {

        Task Kafka(string topic, object message);

    }
}