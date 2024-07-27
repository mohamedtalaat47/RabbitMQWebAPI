using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQWebAPI.Services.RabbitMQServices
{
    public interface IRabbitMQService
    {
        void PublishMessage(string message, string queueName);
        void Dispose() ;
    }
}