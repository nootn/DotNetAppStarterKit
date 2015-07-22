// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using System.Threading.Tasks;
using System.Web.Mvc;
using DotNetAppStarterKit.Core.Mapping;
using DotNetAppStarterKit.SampleMvc.Controllers;
using DotNetAppStarterKit.SampleMvc.DataProject.Query.QueryDto;
using DotNetAppStarterKit.SampleMvc.Models;
using DotNetAppStarterKit.SampleMvc.Models.Mappers;
using DotNetAppStarterKit.Testing.NUnitNSubstituteAutofixture;
using FluentAssertions;
using NSubstitute;
using Ploeh.AutoFixture;

namespace DotNetAppStarterKit.SampleMvc.UnitTests.Controllers.ThingyControllerTests
{
    public class IndexPostWithValidModel : ControllerSpecFor<ThingyController>
    {
        private ThingyModel _validModel;

        protected override ThingyController Given()
        {
            Fixture.Register(() => new ThingyQueryDtoToThingyModelMapper() as IMapper<ThingyQueryDto, ThingyModel>);

            _validModel = Fixture.Create<ThingyModel>();

            var controller = Fixture.Create<ThingyController>();
            controller.SaveThingyCommand.ExecuteAsync(null).ReturnsForAnyArgs(Task.FromResult(true));

            return controller;
        }

        protected override void When()
        {
            Subject.Index(_validModel).ContinueWith(_ => { Result = _.Result; }).Wait();
        }


        [Then]
        public void ShouldRedirectToCorrectRoute()
        {
            var res = MVC.Home.Index().GetT4MVCResult();
            ((RedirectToRouteResult)Result).RouteValues.ShouldBeEquivalentTo(res.RouteValueDictionary);
        }

        [Then]
        public void ShouldHaveTriedToSaveAsync()
        {
            Subject.SaveThingyCommand.ReceivedWithAnyArgs(1).ExecuteAsync(null);
        }

        [Then]
        public void ShouldNotHaveTriedToSaveSync()
        {
            Subject.SaveThingyCommand.DidNotReceiveWithAnyArgs().Execute(null);
        }
    }
}