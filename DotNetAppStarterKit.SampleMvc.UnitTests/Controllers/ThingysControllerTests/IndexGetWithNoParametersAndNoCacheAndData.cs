// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotNetAppStarterKit.SampleMvc.Controllers;
using DotNetAppStarterKit.SampleMvc.DataProject.Query.QueryDto;
using DotNetAppStarterKit.SampleMvc.Models;
using DotNetAppStarterKit.Testing.NUnitNSubstituteAutofixture;
using FluentAssertions;
using NSubstitute;
using Ploeh.AutoFixture;

namespace DotNetAppStarterKit.SampleMvc.UnitTests.Controllers.ThingysControllerTests
{
    public class IndexGetWithNoParametersAndNoCacheAndData : ControllerSpecFor<ThingysController>
    {
        private List<ThingyQueryDto> _validResults;

        protected override ThingysController Given()
        {
            _validResults = Fixture.Create<List<ThingyQueryDto>>();

            var controller = Fixture.Create<ThingysController>();
            controller.GetAllThingysQuery.Execute().ReturnsForAnyArgs(_validResults);
            controller.GetAllThingysQuery.ExecuteAsync()
                .ReturnsForAnyArgs(Task.FromResult(_validResults.AsEnumerable()));
            controller.GetAllThingysQuery.ExecuteCached().ReturnsForAnyArgs(default(IEnumerable<ThingyQueryDto>));
            return controller;
        }

        protected override void When()
        {
            Subject.Index().ContinueWith(_ => { Result = _.Result; }).Wait();
        }

        [Then]
        public void ShouldRenderCorrectView()
        {
            ((ViewResult) Result).ViewName.Should().Be("");
        }

        [Then]
        public void ShouldHavePopulatedThingysCollection()
        {
            ((AllThingysModel) ((ViewResult) Result).Model).Thingys.Should().ContainInOrder(_validResults);
        }

        [Then]
        public void ShouldHaveTriedToGetCachedResults()
        {
            Subject.GetAllThingysQuery.ReceivedWithAnyArgs(1).ExecuteCached();
        }

        [Then]
        public void ShouldHaveTriedToGetAsyncResults()
        {
            Subject.GetAllThingysQuery.ReceivedWithAnyArgs(1).ExecuteAsync();
        }

        [Then]
        public void ShouldNotHaveTriedToGetSyncResults()
        {
            Subject.GetAllThingysQuery.DidNotReceiveWithAnyArgs().Execute();
        }
    }
}