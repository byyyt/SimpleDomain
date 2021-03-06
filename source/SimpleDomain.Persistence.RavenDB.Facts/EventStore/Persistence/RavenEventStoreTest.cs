﻿//-------------------------------------------------------------------------------
// <copyright file="RavenEventStoreTest.cs" company="frokonet.ch">
//   Copyright (c) 2014-2016
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace SimpleDomain.EventStore.Persistence
{
    using System;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.EventStore.Configuration;
    using SimpleDomain.TestDoubles;

    using Xunit;

    public class RavenEventStoreTest : EmbeddedRavenDbTest
    {
        [Fact]
        public void CanOpenTypedEventStream()
        {
            var factory = new EventStoreFactory();
            var configuration = new ContainerLessEventStoreConfiguration(factory);
            configuration.UseRavenEventStore(this.DocumentStore);

            var bus = A.Fake<IDeliverMessages>();

            var testee = factory.Create(configuration, bus);

            var eventStream = testee.OpenStream<MyStaticEventSourcedAggregateRoot>(Guid.NewGuid());

            eventStream.Should().BeAssignableTo<RavenEventStream<MyStaticEventSourcedAggregateRoot>>();
        }
    }
}