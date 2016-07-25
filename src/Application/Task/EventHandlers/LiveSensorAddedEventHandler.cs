﻿using Aggregator.Domain.Events;
using Aggregator.Domain.Write;
using Aggregator.Framework.Contracts;
using Material.Infrastructure;
using SimpleCQRS.Framework.Contracts;

namespace Aggregator.Task.EventHandlers
{
    public class LiveSensorAddedEventHandler<TRequest> : 
        IEventHandler<SensorCreated<TRequest>>
        where TRequest : Request, new()
    {
        private readonly Scheduler _scheduler;
        private readonly ISampleRequestTaskFactory _factory;

        public LiveSensorAddedEventHandler(
            Scheduler scheduler, 
            ISampleRequestTaskFactory schedulerTaskFactory)
        {
            _scheduler = scheduler;
            _factory = schedulerTaskFactory;
        }

        public void Handle(SensorCreated<TRequest> @event)
        {
            var task = _factory.GetTask(
                @event.AggregateId,
                @event.AuthenticationToken, 
                typeof(TRequest),
                string.Empty);

            _scheduler.AddTask(
                @event.SensorId, 
                task,
                @event.PollingInterval, 
                true);
        }
    }
}