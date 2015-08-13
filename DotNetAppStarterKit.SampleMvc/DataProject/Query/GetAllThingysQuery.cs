// /*
// Copyright (c) 2015 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DotNetAppStarterKit.Core.Caching;
using DotNetAppStarterKit.Core.EventBroker;
using DotNetAppStarterKit.Core.Mapping;
using DotNetAppStarterKit.Core.Query;
using DotNetAppStarterKit.SampleMvc.DataProject.Context;
using DotNetAppStarterKit.SampleMvc.DataProject.Entity;
using DotNetAppStarterKit.SampleMvc.DataProject.Event;
using DotNetAppStarterKit.SampleMvc.DataProject.Query.Interface;
using DotNetAppStarterKit.SampleMvc.DataProject.Query.QueryDto;

namespace DotNetAppStarterKit.SampleMvc.DataProject.Query
{
    public class GetAllThingysQuery : CachedAsyncQueryBase<IEnumerable<ThingyQueryDto>>, IGetAllThingysQuery
    {
        private readonly ICacheProvider<IEnumerable<ThingyQueryDto>> _cacheProvider;
        private readonly IDummyDataContext _context;
        private readonly IMapper<Thingy, ThingyQueryDto> _mapper;

        public GetAllThingysQuery(IDummyDataContext context, IMapper<Thingy, ThingyQueryDto> mapper,
            ICacheProvider<IEnumerable<ThingyQueryDto>> cacheProvider)
        {
            _context = context;
            _mapper = mapper;
            _cacheProvider = cacheProvider;
        }

        public override IEnumerable<ThingyQueryDto> ExecuteCached()
        {
            return _cacheProvider.GetCachedItem("");
        }

        public override async Task<IEnumerable<ThingyQueryDto>> ExecuteAsync()
        {
            var items = await
                (from t in _context.Thingys
                    orderby t.Name
                    select t).ToListAsync();

            var res = items.Select(item => _mapper.Map(item, new ThingyQueryDto()));

            DomainEvents.Raise(new ThingysRetrievedEvent(res));

            return res;
        }
    }
}