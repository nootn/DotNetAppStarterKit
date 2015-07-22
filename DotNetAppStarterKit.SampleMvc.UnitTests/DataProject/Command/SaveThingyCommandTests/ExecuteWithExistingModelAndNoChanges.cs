﻿// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using System;
using System.Collections.Generic;
using System.Linq;
using DotNetAppStarterKit.SampleMvc.DataProject;
using DotNetAppStarterKit.SampleMvc.DataProject.Command;
using DotNetAppStarterKit.SampleMvc.DataProject.Command.CommandDto;
using DotNetAppStarterKit.SampleMvc.DataProject.Entity;
using DotNetAppStarterKit.SampleMvc.DataProject.Event;
using DotNetAppStarterKit.Testing.NUnitNSubstituteAutofixture;
using FluentAssertions;
using NSubstitute;
using Ploeh.AutoFixture;

namespace DotNetAppStarterKit.SampleMvc.UnitTests.DataProject.Command.SaveThingyCommandTests
{
    internal class ExecuteWithExistingModelAndNoChanges : SaveThingyCommandSpecFor
    {
        private ThingyCommandDto _model;
        private Guid _originalModelId;
        private Thingy _existingEntity;

        protected override SaveThingyCommand Given()
        {
            _model = Fixture.Create<ThingyCommandDto>();
            _originalModelId = Fixture.Create<Guid>();
            _model.Id = _originalModelId;
            _existingEntity = Fixture.Create<Thingy>();
            _existingEntity.Id = _originalModelId;

            var cmd = Fixture.Create<SaveThingyCommand>();

            //IQueryable is not mocked well, so instead use a List
            var existingThingys = Fixture.Create<List<Thingy>>();
            existingThingys.Add(_existingEntity);
            cmd.Context.Thingys = existingThingys.AsQueryable();

            cmd.Context.SaveChanges().ReturnsForAnyArgs(0);

            return cmd;
        }

        protected override void When()
        {
            Subject.Execute(_model);
        }

        [Then]
        public void ShouldHaveExistingModelId()
        {
            _model.Id.Should().Be(_originalModelId);
        }

        [Then]
        public void ShouldHaveMappedModelToEntity()
        {
            Subject.Mapper.ReceivedWithAnyArgs(1).Map(null, null);
        }

        [Then]
        public void ShouldHaveCalledSavedChanges()
        {
            Subject.Context.ReceivedWithAnyArgs(1).SaveChanges();
        }

        [Then]
        public void ShouldHavePublishedEventIndicatingNoAction()
        {
            EventBroker.Received(1).Raise(Arg.Is<ThingyChangedEvent>(t => t.Action == Enums.ChangeAction.None));
        }
    }
}