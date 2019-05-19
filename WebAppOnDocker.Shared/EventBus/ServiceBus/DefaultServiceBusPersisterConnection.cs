using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace WebAppOnDocker.Shared.EventBus.ServiceBus
{
    public class DefaultServiceBusPersisterConnection : IServiceBusPersisterConnection
    {
        private bool _disposed;

        private readonly ILogger<DefaultServiceBusPersisterConnection> _logger;
        private readonly ServiceBusConnectionStringBuilder _serviceBusConnectionStringBuilder;
        private ITopicClient _topicClient;

        public DefaultServiceBusPersisterConnection(ILogger<DefaultServiceBusPersisterConnection> logger, 
                                                    ServiceBusConnectionStringBuilder serviceBusConnectionStringBuilder)
        {
            _logger = logger;
            _serviceBusConnectionStringBuilder = serviceBusConnectionStringBuilder;
            
            _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy.Default);
        }

        public ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder => _serviceBusConnectionStringBuilder;

        public ITopicClient CreateModel()
        {
            if (_topicClient.IsClosedOrClosing)
            {
                _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy.Default);
            }

            return _topicClient;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }
    }
}