using System;
using System.Collections.Generic;
using System.Threading;

namespace OOP_LR4._1
{
    class Program
    {
        static void Main(string[] args)
        {
            var publisher = new Publisher();

            var subscriber1 = new Subscriber(1);
            var subscriber2 = new Subscriber(2);
            var subscriber3 = new Subscriber(3);

        }
    }
    public class EventBus
    {
        private Dictionary<string, List<Action>> _eventHandlers;
        private Dictionary<string, DateTime> _lastEventTimes;
        private int _throttleTime;

        public EventBus(int throttleTime)
        {
            _eventHandlers = new Dictionary<string, List<Action>>();
            _lastEventTimes = new Dictionary<string, DateTime>();
            _throttleTime = throttleTime;
        }

        public void Register(string eventName, Action eventHandler)
        {
            if (!_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName] = new List<Action>();
            }

            _eventHandlers[eventName].Add(eventHandler);
        }

        public void Unregister(string eventName, Action eventHandler)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName].Remove(eventHandler);
            }
        }

        public void Emit(string eventName)
        {
            if (!_eventHandlers.ContainsKey(eventName))
            {
                return;
            }

            var now = DateTime.Now;
            if (_lastEventTimes.ContainsKey(eventName) && now.Subtract(_lastEventTimes[eventName]).TotalMilliseconds < _throttleTime)
            {
                return;
            }

            _lastEventTimes[eventName] = now;

            foreach (var handler in _eventHandlers[eventName])
            {
                ThreadPool.QueueUserWorkItem(_ => handler());
            }
        }
    }
    public class Publisher
    {
        private Dictionary<Action<object>, int> subscribers = new Dictionary<Action<object>, int>();

        public void Subscribe(Action<object> subscriber, int priority)
        {
            subscribers[subscriber] = priority;
        }

        public void Unsubscribe(Action<object> subscriber)
        {
            subscribers.Remove(subscriber);
        }

        public void Publish(object data)
        {
            var sortedSubscribers = subscribers.OrderBy(x => x.Value);
            foreach (var subscriber in sortedSubscribers)
            {
                subscriber.Key(data);
            }
        }
    }
    public class Subscriber
    {
        private int priority;

        public Subscriber(int priority)
        {
            this.priority = priority;
        }

        public void HandleEvent(object data)
        {
            Console.WriteLine($"Received event with data: {data} and priority: {priority}");
        }
    }

}
