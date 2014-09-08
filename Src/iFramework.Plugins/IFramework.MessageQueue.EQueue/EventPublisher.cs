﻿using IFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IFramework.Infrastructure;
using IFramework.Message.Impl;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using IFramework.Infrastructure.Logging;
using IFramework.Message;
using IFramework.MessageQueue.MessageFormat;
using EQueueClientsProducers = EQueue.Clients.Producers;

namespace IFramework.MessageQueue.EQueue
{
    public class EventPublisher : IEventPublisher
    {
        protected ILogger _Logger;
        protected BlockingCollection<IMessageContext> MessageQueue { get; set; }
        protected EQueueClientsProducers.Producer Producer { get; set; }
        protected string Id { get; set; }
        protected string Topic { get; set; }

        public EventPublisher(string id, string topic, EQueueClientsProducers.ProducerSetting producerSetting)
        {
            Topic = topic;
            Id = id;
            _Logger = IoCFactory.Resolve<ILoggerFactory>().Create(this.GetType());
            MessageQueue = new BlockingCollection<IMessageContext>();
            try
            {

                Producer = new EQueueClientsProducers.Producer(id, producerSetting);
                Producer.Start();
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.GetBaseException().Message, ex);
            }
        }

        public IEnumerable<IMessageContext> Publish(params IDomainEvent[] events)
        {
            List<IMessageContext> messageContexts = new List<IMessageContext>();
            events.ForEach(@event =>
            {
                var messageContext = new MessageContext(@event);
                messageContexts.Add(messageContext);
            });
            var messageBody = messageContexts.GetMessageBytes();
            var sendResult = Producer.Send(new global::EQueue.Protocols.Message(Topic, messageBody), string.Empty);
            if (sendResult.SendStatus == EQueueClientsProducers.SendStatus.Success)
            {
                _Logger.DebugFormat("publish {0} events success, body {1}", 
                                     messageContexts.Count, messageContexts.ToJson());
            }
            else
            {
                _Logger.ErrorFormat("Send {0} event failed {1}", 
                                     messageContexts.Count, sendResult.SendStatus.ToString());
            }
            return messageContexts;
        }
    }
}
