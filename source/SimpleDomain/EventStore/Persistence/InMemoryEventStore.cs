﻿//-------------------------------------------------------------------------------
// <copyright file="InMemoryEventStore.cs" company="frokonet.ch">
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
    using System.Collections.Generic;
    
    /// <summary>
    /// The InMemory event store
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        /// <summary>
        /// Gets the event descriptor list configuration key
        /// </summary>
        public const string EventDescriptors = "EventDescriptors";

        /// <summary>
        /// Gets the snapshot descriptor list configuration key
        /// </summary>
        public const string SnapshotDescriptors = "SnapshotDescriptors";

        private readonly IHaveEventStoreConfiguration configuration;

        /// <summary>
        /// Creates a new instance of <see cref="InMemoryEventStore"/>
        /// </summary>
        /// <param name="configuration">Dependency injection for <see cref="IHaveEventStoreConfiguration"/></param>
        public InMemoryEventStore(IHaveEventStoreConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <inheritdoc />
        public IEventStream OpenStream<T>(Guid aggregateId) where T : IEventSourcedAggregateRoot
        {
            return new InMemoryEventStream<T>(
                aggregateId,
                this.configuration.DispatchEvents,
                this.configuration.Get<List<EventDescriptor>>(EventDescriptors),
                this.configuration.Get<List<SnapshotDescriptor>>(SnapshotDescriptors));
        }
    }
}