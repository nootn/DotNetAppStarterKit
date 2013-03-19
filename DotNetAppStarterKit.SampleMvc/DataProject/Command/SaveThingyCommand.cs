// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com)
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
using DotNetAppStarterKit.Core.Command;
using DotNetAppStarterKit.Core.Event;
using DotNetAppStarterKit.Core.Logging;
using DotNetAppStarterKit.SampleMvc.DataProject.Command.CommandDto;
using DotNetAppStarterKit.SampleMvc.DataProject.Command.Interface;
using DotNetAppStarterKit.SampleMvc.DataProject.Command.Mappers;
using DotNetAppStarterKit.SampleMvc.DataProject.Context;
using DotNetAppStarterKit.SampleMvc.DataProject.Event;

namespace DotNetAppStarterKit.SampleMvc.DataProject.Command
{
    public class SaveThingyCommand : CommandBase<ThingyCommandDto>, ISaveThingyCommand
    {
        private readonly IDummyDataContext _context;
        private readonly ILogWithCallerInfo<SaveThingyCommand> _log;
        private readonly ThingyCommandDtoToThingyMapper _mapper;
        private readonly IEventPublisher<ThingyChangedEvent> _publisherThingyChanged;

        public SaveThingyCommand(IDummyDataContext context, ThingyCommandDtoToThingyMapper mapper,
                                 IEventPublisher<ThingyChangedEvent> publisherThingyChanged,
                                 ILogWithCallerInfo<SaveThingyCommand> log)
        {
            _context = context;
            _mapper = mapper;
            _publisherThingyChanged = publisherThingyChanged;
            _log = log;
        }

        public override void Execute(ThingyCommandDto model)
        {
            Enums.ChangeAction action;
            var item = model.Id == Guid.Empty
                           ? null
                           : _context.Thingys.SingleOrDefault(_ => _.Id == model.Id);
            if (item == null)
            {
                //we need to set the id - set it on the model and it will get mapped onto the entity later
                model.Id = Guid.NewGuid();

                //create a new entity and mark it to be added
                item = _context.Thingys.Create();
                _context.Thingys.Add(item);

                action = Enums.ChangeAction.Added;
                _log.InformationWithCallerInfo(
                    () => string.Format("[ThreadId: {1}] Did not exist, will add with new id '{0}'", model.Id,
                                        Thread.CurrentThread.ManagedThreadId));
            }
            else
            {
                action = Enums.ChangeAction.Updated;
                _log.InformationWithCallerInfo(
                    () =>
                    string.Format(
                        "[ThreadId: {1}] Did exist, will possibly update existing id '{0}' if it has changed", model.Id,
                        Thread.CurrentThread.ManagedThreadId));
            }
            _mapper.Map(model, item);

            var res = _context.SaveChanges();

            if (res > 0)
            {
                _log.InformationWithCallerInfo(() => string.Format("[ThreadId: {1}] Changes were saved on '{0}'", model.Id,
                                                     Thread.CurrentThread.ManagedThreadId));
            }
            else
            {
                action = Enums.ChangeAction.None;
                _log.InformationWithCallerInfo(() => string.Format("[ThreadId: {1}] No changes were saved on '{0}'", model.Id,
                                                     Thread.CurrentThread.ManagedThreadId));
            }

            //Notify others that something has happened
            _publisherThingyChanged.Publish(new ThingyChangedEvent {Action = action, ThingyId = model.Id});
        }
    }
}