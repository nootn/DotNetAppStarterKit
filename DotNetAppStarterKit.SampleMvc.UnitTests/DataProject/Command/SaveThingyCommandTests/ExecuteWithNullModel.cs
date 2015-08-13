// /*
// Copyright (c) 2015 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using System;
using DotNetAppStarterKit.SampleMvc.DataProject.Command;
using DotNetAppStarterKit.SampleMvc.DataProject.Command.CommandDto;
using DotNetAppStarterKit.Testing.NUnitNSubstituteAutofixture;
using FluentAssertions;
using Ploeh.AutoFixture;

namespace DotNetAppStarterKit.SampleMvc.UnitTests.DataProject.Command.SaveThingyCommandTests
{
    internal class ExecuteWithNullModel : SaveThingyCommandSpecFor
    {
        private Exception _expectedException;
        private ThingyCommandDto _model;

        protected override SaveThingyCommand Given()
        {
            _model = null;

            var cmd = Fixture.Create<SaveThingyCommand>();
            return cmd;
        }

        protected override async void When()
        {
            try
            {
                await Subject.ExecuteAsync(_model);
            }
            catch (Exception ex)
            {
                _expectedException = ex;
            }
        }

        [Then]
        public void ShouldThrowExceptionIndicatingNullModel()
        {
            _expectedException.Message.Should().Contain("model");
        }

        [Then]
        public void ShouldThrowArgumentNullException()
        {
            _expectedException.Should().BeOfType<ArgumentNullException>();
        }
    }
}