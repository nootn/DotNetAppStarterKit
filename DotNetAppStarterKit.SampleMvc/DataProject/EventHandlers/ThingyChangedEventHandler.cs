﻿// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using System.Collections.Generic;
using DotNetAppStarterKit.Core.Caching;
using DotNetAppStarterKit.Core.EventBroker;
using DotNetAppStarterKit.SampleMvc.DataProject.Event;
using DotNetAppStarterKit.SampleMvc.DataProject.Query.QueryDto;

namespace DotNetAppStarterKit.SampleMvc.DataProject.EventHandlers
{
    public class ThingyChangedEventHandler : IHandle<ThingyChangedEvent>
    {
        private readonly ICacheProvider<IEnumerable<ThingyQueryDto>> _thingysCacheProvider;
        private readonly ICacheProvider<ThingyQueryDto> _thingyCacheProvider;

        public ThingyChangedEventHandler(ICacheProvider<IEnumerable<ThingyQueryDto>> thingysCacheProvider,
                                            ICacheProvider<ThingyQueryDto> thingyCacheProvider)
        {
            _thingysCacheProvider = thingysCacheProvider;
            _thingyCacheProvider = thingyCacheProvider;
        }

        public void Handle(ThingyChangedEvent data)
        {
            _thingyCacheProvider.RemoveCachedItem(data.ThingyId.ToString());
            _thingysCacheProvider.RemoveCachedItem("");
        }
    }
}