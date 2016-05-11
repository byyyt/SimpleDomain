﻿//-------------------------------------------------------------------------------
// <copyright file="CompositionRoot.cs" company="frokonet.ch">
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

namespace GiftcardSample
{
    using GiftcardSample.Commands;

    using SimpleDomain;
    using SimpleDomain.Bus;
    using SimpleDomain.EventStore;
    using SimpleDomain.EventStore.Persistence;
    
    public class CompositionRoot : AbstractCompositionRoot
    {
        protected override void ConfigureBus(IConfigureThisJitney configuration)
        {
            configuration.DefineLocalEndpointAddress("gc.sample");
            configuration.SetSubscriptionStore(new FileSubscriptionStore());
            configuration.MapContracts(typeof(CreateGiftcard).Assembly).ToMe();
            configuration.AddPipelineStep(new LogIncommingEnvelopeStep());
        }

        protected override Jitney CreateBus(IHaveJitneyConfiguration configuration)
        {
            return new SimpleJitney(configuration);
        }

        protected override void ConfigureEventStore(IConfigureThisEventStore configuration)
        {
            configuration.DefineAsyncEventDispatching(@event => this.Bus.PublishAsync(@event));
            configuration.PrepareInMemoryEventStore();
        }

        protected override IEventStore CreateEventStore(IHaveEventStoreConfiguration configuration)
        {
            return new InMemoryEventStore(configuration);
        }
    }
}