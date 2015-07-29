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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetAppStarterKit.Core.Command;
using DotNetAppStarterKit.Core.EventBroker;
using DotNetAppStarterKit.Core.Logging;
using DotNetAppStarterKit.Core.Mapping;
using DotNetAppStarterKit.SampleMvc.Application;
using DotNetAppStarterKit.SampleMvc.DataProject.Command.CommandDto;
using DotNetAppStarterKit.SampleMvc.DataProject.Command.Interface;
using DotNetAppStarterKit.SampleMvc.DataProject.Context;
using DotNetAppStarterKit.SampleMvc.DataProject.Entity;
using DotNetAppStarterKit.SampleMvc.DataProject.Event;

namespace DotNetAppStarterKit.SampleMvc.DataProject.Command
{
    public class SaveThingyCommand : CommandBase<ThingyCommandDto>, ISaveThingyCommand
    {
        public readonly IDummyDataContext Context;
        public readonly ILogWithCallerInfo<SaveThingyCommand> Log;
        public readonly IMapper<ThingyCommandDto, Thingy> Mapper;
        private readonly ILifetimeScopeAwareTaskFactory _taskFactory;

        public SaveThingyCommand(IDummyDataContext context, IMapper<ThingyCommandDto, Thingy> mapper,
            ILifetimeScopeAwareTaskFactory taskFactory,
            ILogWithCallerInfo<SaveThingyCommand> log)
        {
            Context = context;
            Mapper = mapper;
            _taskFactory = taskFactory;
            Log = log;
        }

        public override void Execute(ThingyCommandDto model)
        {
            if (model == null) throw new ArgumentNullException("model");
            Enums.ChangeAction action;
            var item = model.Id == Guid.Empty
                ? null
                : Context.Thingys.SingleOrDefault(_ => _.Id == model.Id);
            if (item == null)
            {
                //we need to set the id - set it on the model and it will get mapped onto the entity later
                model.Id = Guid.NewGuid();

                //create a new entity and mark it to be added
                item = Context.CreateAndAddThingy();

                action = Enums.ChangeAction.Added;
                Log.InformationWithCallerInfo(() => string.Format("[ThreadId: {1}] Did not exist, will add with new id '{0}'", model.Id, Thread.CurrentThread.ManagedThreadId));
            }
            else
            {
                action = Enums.ChangeAction.Updated;
                Log.InformationWithCallerInfo(() =>string.Format("[ThreadId: {1}] Did exist, will possibly update existing id '{0}' if it has changed", model.Id, Thread.CurrentThread.ManagedThreadId));
            }
            Mapper.Map(model, item);

            var res = Context.SaveChanges();

            if (res > 0)
            {
                Log.InformationWithCallerInfo(() => string.Format("[ThreadId: {1}] Changes were saved on '{0}'", model.Id, Thread.CurrentThread.ManagedThreadId));
            }
            else
            {
                action = Enums.ChangeAction.None;
                Log.InformationWithCallerInfo(() => string.Format("[ThreadId: {1}] No changes were saved on '{0}'", model.Id, Thread.CurrentThread.ManagedThreadId));
            }

            //Notify others that something has happened
            DomainEvents.Raise(new ThingyChangedEvent(model.Id, action));
        }

        public override Task ExecuteAsync(ThingyCommandDto model)
        {
            return _taskFactory.StartNew(() => Execute(model));
        }
    }
}