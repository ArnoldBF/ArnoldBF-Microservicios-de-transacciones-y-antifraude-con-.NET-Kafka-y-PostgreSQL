using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiFraudService.config
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string ConsumeTopic { get; set; } = string.Empty;
        public string ProduceTopic { get; set; } = string.Empty;

    }
}