using System;
using System.Collections.Generic;
using System.Threading;

namespace OOP_LR4._1
{
    class Program
    {
        static void Main(string[] args)
        {

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
    public class TemperatureChangedEventArgs : EventArgs
    {
        public double Temperature { get; set; }

        public TemperatureChangedEventArgs(double temperature)
        {
            Temperature = temperature;
        }
    }
}
