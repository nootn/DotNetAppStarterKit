// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotNetAppStarterKit.SampleMvc.Controllers;
using DotNetAppStarterKit.SampleMvc.DataProject.Query.QueryDto;
using DotNetAppStarterKit.SampleMvc.Models;
using DotNetAppStarterKit.Testing.NUnitNSubstituteAutofixture;
using FluentAssertions;
using NSubstitute;
using Ploeh.AutoFixture;

namespace DotNetAppStarterKit.SampleMvc.UnitTests.Controllers.ThingyControllerTests
{
    public class IndexGetWithInvalidIdAndNoCacheAndData : ControllerSpecFor<ThingyController>
    {
        private Guid _invalidId;

        protected override ThingyController Given()
        {
            _invalidId = Fixture.Create<Guid>();

            var controller = Fixture.Create<ThingyController>();
            controller.GetThingyQuery.Execute(_invalidId).Returns(default(ThingyQueryDto));
            controller.GetThingyQuery.ExecuteAsync(_invalidId).Returns(Task.FromResult(default(ThingyQueryDto)));
            controller.GetThingyQuery.ExecuteCached(Guid.Empty).ReturnsForAnyArgs(default(ThingyQueryDto));
            return controller;
        }

        protected override void When()
        {
            Subject.Index(_invalidId).ContinueWith(_ => { Result = (ViewResult) _.Result; }).Wait();
        }


        [Then]
        public void ShouldRenderCorrectView()
        {
            Result.ViewName.Should().Be("");
        }

        [Then]
        public void ShouldHaveNewThingy()
        {
            ((ThingyModel) Result.Model).Id.Should().Be(Guid.Empty);
        }

        [Then]
        public void ShouldHaveTriedToGetCachedResults()
        {
            Subject.GetThingyQuery.Received().ExecuteCached(_invalidId);
        }

        [Then]
        public void ShouldHaveTriedToGetAsyncResults()
        {
            Subject.GetThingyQuery.Received().ExecuteAsync(_invalidId);
        }

        [Then]
        public void ShouldNotHaveTriedToGetSyncResults()
        {
            Subject.GetThingyQuery.DidNotReceiveWithAnyArgs().Execute(Guid.Empty);
        }
    }
}