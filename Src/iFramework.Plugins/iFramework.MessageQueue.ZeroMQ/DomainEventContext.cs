﻿using IFramework.Message.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFramework.Event.Impl
{
    public class DomainEventContext : MessageContext
    {
        public DomainEventContext() : base()
        {
            
        }
        public DomainEventContext(object message) : base(message)
        {
          
        }
    }
}
