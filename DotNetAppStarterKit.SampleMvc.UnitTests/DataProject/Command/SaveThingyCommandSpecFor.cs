// /*
// Copyright (c) 2015 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using DotNetAppStarterKit.Core.EventBroker;
using DotNetAppStarterKit.Core.Mapping;
using DotNetAppStarterKit.SampleMvc.DataProject.Command;
using DotNetAppStarterKit.SampleMvc.DataProject.Command.CommandDto;
using DotNetAppStarterKit.SampleMvc.DataProject.Context;
using DotNetAppStarterKit.SampleMvc.DataProject.Entity;
using DotNetAppStarterKit.SampleMvc.UnitTests.Extensions;
using DotNetAppStarterKit.Testing.NUnitNSubstituteAutofixture;
using NSubstitute;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;

namespace DotNetAppStarterKit.SampleMvc.UnitTests.DataProject.Command
{
    public abstract class SaveThingyCommandSpecFor : AutoSpecFor<SaveThingyCommand>
    {
        protected IEventBroker EventBroker { get; }
        protected IDummyDataContext Context;
        protected IMapper<ThingyCommandDto, Thingy> Mapper;

        protected SaveThingyCommandSpecFor()
        {
            Fixture.Customize(new AutoNSubstituteCustomization());
            EventBroker = Substitute.For<IEventBroker>();

            Fixture.Register<IDummyDataContext>(() => Substitute.ForPartsOf<FakeDummyDataContext>());
            Context = Fixture.Create<IDummyDataContext>();
            Fixture.Inject(Context);

            Mapper = Fixture.Create<IMapper<ThingyCommandDto, Thingy>>();
            Fixture.Inject(Mapper);

            DomainEvents.SetEventBroker(EventBroker);
        }
    }
}