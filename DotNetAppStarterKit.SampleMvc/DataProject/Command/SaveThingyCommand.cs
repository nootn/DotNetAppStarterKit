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
using DotNetAppStarterKit.Core.Command;
using DotNetAppStarterKit.Core.Event;
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
        private readonly ThingyCommandDtoToThingyMapper _mapper;
        private readonly IEventPublisher<ThingyChangedEvent> _publisherThingyChanged;

        public SaveThingyCommand(IDummyDataContext context, ThingyCommandDtoToThingyMapper mapper,
                                 IEventPublisher<ThingyChangedEvent> publisherThingyChanged)
        {
            _context = context;
            _mapper = mapper;
            _publisherThingyChanged = publisherThingyChanged;
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
            }
            else
            {
                action = Enums.ChangeAction.Updated;
            }
            _mapper.Map(model, item);

            _context.SaveChanges();

            //Notify others that something has happened
            _publisherThingyChanged.Publish(new ThingyChangedEvent {Action = action, ThingyId = model.Id});
        }
    }
}