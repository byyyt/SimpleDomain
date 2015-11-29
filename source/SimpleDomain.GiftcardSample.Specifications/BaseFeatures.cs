﻿//-------------------------------------------------------------------------------
// <copyright file="BaseFeatures.cs" company="frokonet.ch">
//   Copyright (c) 2014-2015
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

namespace GiftcardSample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using GiftcardSample.Domain;
    using GiftcardSample.ReadStore;
    using GiftcardSample.ReadStore.InMemory;

    using SimpleDomain;
    using SimpleDomain.Bus;
    using SimpleDomain.EventStore;

    public abstract class BaseFeatures
    {
        private readonly CompositionRoot compositionRoot;

        protected BaseFeatures()
        {
            var readStore = new InMemoryReadStore();

            this.compositionRoot = new CompositionRoot();
            this.compositionRoot.Register(new GiftcardContext(readStore));

            this.OverviewQuery = new InMemoryGiftcardOverviewQuery(readStore);
            this.TransactionQuery = new InMemoryGiftcardTransactionQuery(readStore);
        }

        protected Jitney Bus
        {
            get { return this.compositionRoot.Bus; }
        }

        protected IEventStore EventStore
        {
            get { return this.compositionRoot.EventStore; }
        }

        protected IGiftcardOverviewQuery OverviewQuery { get; private set; }

        protected IGiftcardTransactionQuery TransactionQuery { get; private set; }

        protected async Task PrepareEventsAsync(Guid cardId, params IEvent[] events)
        {
            var expectedVersion = events.Length - 1;
            var version = 0;

            var versionableEvents = events.Select(@event => new VersionableEvent(@event).With(version++));

            using (var eventStream = this.EventStore.OpenStream<Giftcard>(cardId))
            {
                await eventStream.SaveAsync(versionableEvents, expectedVersion, new Dictionary<string, object>());
            }
        }
    }
}