﻿using IFramework.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFramework.Message.Impl;

namespace IFramework4._5Tests
{
    public class MessageContext : IMessageContext
    {
        public string CorrelationID
        {
            get; set;
        }

        public long Offset { get; set; }

        public IDictionary<string, object> Headers
        {
            get; set;
        }

        public string Key
        {
            get; set;
        }

        public object Message
        {
            get; set;
        }

        public string MessageID
        {
            get; set;
        }

        public object Reply
        {
            get; set;
        }

        public string ReplyToEndPoint
        {
            get; set;
        }

        public DateTime SentTime
        {
            get; set;
        }

        public List<IMessageContext> ToBeSentMessageContexts
        {
            get; set;
        }

        public string Topic
        {
            get; set;
        }

        public SagaInfo SagaInfo
        {
            get;set;
        }
    }
}
