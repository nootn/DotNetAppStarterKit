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
using DotNetAppStarterKit.Core.Caching;
using DotNetAppStarterKit.Core.Query;
using DotNetAppStarterKit.SampleMvc.DataProject.Context;
using DotNetAppStarterKit.SampleMvc.DataProject.Query.Interface;
using DotNetAppStarterKit.SampleMvc.DataProject.Query.Mappers;
using DotNetAppStarterKit.SampleMvc.DataProject.Query.QueryDto;

namespace DotNetAppStarterKit.SampleMvc.DataProject.Query
{
    public class GetThingyQuery : CachedQueryBase<Guid, ThingyQueryDto>, IGetThingyQuery
    {
        private readonly ICacheProvider<ThingyQueryDto> _cacheProvider;
        private readonly IDummyDataContext _context;
        private readonly ThingyToThingyQueryDtoMapper _mapper;

        public GetThingyQuery(IDummyDataContext context, ThingyToThingyQueryDtoMapper mapper,
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

        public override ThingyQueryDto Execute(Guid model)
        {
            var entity = _context.Thingys.SingleOrDefault(_ => _.Id == model);
            if (entity != null)
            {
                var item = _mapper.Map(entity, new ThingyQueryDto());
                _cacheProvider.AddCachedItem(item, item.Id.ToString(), 10);
                return item;
            }
            return null;
        }
    }
}