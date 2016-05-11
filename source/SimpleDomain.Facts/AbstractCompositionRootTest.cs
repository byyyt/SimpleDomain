﻿//-------------------------------------------------------------------------------
// <copyright file="AbstractCompositionRootTest.cs" company="frokonet.ch">
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

namespace SimpleDomain
{
    using System;

    using FakeItEasy;

    using FluentAssertions;

    using SimpleDomain.Bus;
    using SimpleDomain.EventStore;
    using SimpleDomain.EventStore.Persistence;

    using Xunit;

    public class AbstractCompositionRootTest
    {
        [Fact]
        public void CanBuildBusEventStoreAndRepositoryBasedOnDerivedMethods()
        {
            var testee = new CompositionRoot();

            testee.Bus.Should().BeAssignableTo<SimpleJitney>();
            testee.EventStore.Should().BeAssignableTo<InMemoryEventStore>();
            testee.Repository.Should().NotBeNull();
        }

        [Fact]
        public void CanRegisterBoundedContexts()
        {
            var firstBoundedContext = A.Fake<IBoundedContext>();
            var secondBoundedContext = A.Fake<IBoundedContext>();
            var testee = new CompositionRoot();

            testee.Register(firstBoundedContext);
            testee.Register(secondBoundedContext);

            A.CallTo(() => firstBoundedContext.Configure(A<Jitney>.Ignored, A<IEventSourcedRepository>.Ignored)).MustHaveHappened();
            A.CallTo(() => secondBoundedContext.Configure(A<Jitney>.Ignored, A<IEventSourcedRepository>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void ThrowsException_WhenTryingToRegisterNullAsBoundedContext()
        {
            var testee = new CompositionRoot();

            Action action = () => testee.Register(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        private class CompositionRoot : AbstractCompositionRoot
        {
            protected override void ConfigureBus(IConfigureThisJitney configuration)
            {
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
}