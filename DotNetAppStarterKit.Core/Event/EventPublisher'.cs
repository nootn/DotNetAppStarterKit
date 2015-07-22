// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using System;
using System.Linq;

namespace DotNetAppStarterKit.Core.Event
{
    [Obsolete("This class is obsolete, please use DomainEvents.")]
    public class EventPublisher<T> : IEventPublisher<T>
    {
        private readonly IEventSubscribersProvider<T> _subscriptionService;

        public EventPublisher(IEventSubscribersProvider<T> subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public void Publish(T data)
        {
            var subscriptions = _subscriptionService.GetSubscribersForEvent();
            subscriptions.ToList().ForEach(consumer => PublishToConsumer(consumer, data));
        }

        private static void PublishToConsumer(IEventSubscriber<T> consumer, T data)
        {
            consumer.Handle(data);
        }
    }
}