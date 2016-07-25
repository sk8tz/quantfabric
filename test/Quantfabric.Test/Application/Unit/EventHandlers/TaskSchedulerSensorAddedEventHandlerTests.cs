﻿using System;
using Newtonsoft.Json.Linq;
using Aggregator.Domain.Events;
using Aggregator.Domain.Write;
using Aggregator.Task;
using Aggregator.Task.EventHandlers;
using Aggregator.Test.Helpers.Mocks;
using Aggregator.Test.Mocks;
using Xunit;

namespace Aggregator.Test.Unit.EventHandlers
{
    
    public class TaskSchedulerSensorAddedEventHandlerTests
    {
        [Fact]
        public void HandleSensorCreatedEventThrown()
        {
            var sensorId = Guid.NewGuid();
            var aggregateId = Guid.NewGuid();
            var @event = new SensorCreated<RequestMock>(sensorId, TimeSpan.Zero, new JObject());
            @event.AggregateId = aggregateId;
            var scheduler = new Scheduler();
            var taskFactory = new SampleRequestTaskFactoryMock();

            var eventHandler = new LiveSensorAddedEventHandler<RequestMock>(
                scheduler, 
                taskFactory);

            eventHandler.Handle(@event);

            taskFactory.AssertMinimumNumberOfInvocations(1);
        }
    }
}