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
    public class GetThingyQuery : CachedAsyncQueryBase<Guid, ThingyQueryDto>, IGetThingyQuery
    {
        private readonly ICacheProvider<ThingyQueryDto> _cacheProvider;
        private readonly IDummyDataContext _context;
        private readonly IMapper<Thingy, ThingyQueryDto> _mapper;

        public GetThingyQuery(IDummyDataContext context, IMapper<Thingy, ThingyQueryDto> mapper,
            ICacheProvider<ThingyQueryDto> cacheProvider)
        {
            _context = context;
            _mapper = mapper;
            _cacheProvider = cacheProvider;
        }

        public override ThingyQueryDto ExecuteCached(Guid model)
        {
            return _cacheProvider.GetCachedItem(model.ToString());
        }

        public override async Task<ThingyQueryDto> ExecuteAsync(Guid model)
        {
            var entity = await
                (from t in _context.Thingys
                    where t.Id == model
                    select t).SingleOrDefaultAsync();

            if (entity != null)
            {
                var item = _mapper.Map(entity, new ThingyQueryDto());
                DomainEvents.Raise(new ThingyRetrievedEvent(item));
                return item;
            }
            return null;
        }
    }
}