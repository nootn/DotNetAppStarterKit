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
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using DotNetAppStarterKit.Core.Caching;

namespace DotNetAppStarterKit.Web.Caching
{
    /// <summary>
    /// Handles caching of items using the type name as part of the key for uniqueness.
    /// Supports "Offline" mode when there is no HTTPContext by storing in a static variable - handy for testing if you don'y want to have to mock it out.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebCacheProvider<T> : ICacheProvider<T> where T : class
    {
        private static readonly Dictionary<string, KeyValuePair<T, DateTime>> InMemoryItems =
            new Dictionary<string, KeyValuePair<T, DateTime>>();

        private static readonly T ThisLock = default(T);

        public void RemoveCachedItem(string key)
        {
            var itemKey = GetCacheKey(key);
            if (HttpContext.Current != null && HttpContext.Current.Cache != null)
            {
                HttpContext.Current.Cache.Remove(itemKey);
            }
            else
            {
                lock (ThisLock)
                {
                    if (InMemoryItems.ContainsKey(itemKey))
                    {
                        InMemoryItems.Remove(itemKey);
                    }
                }
            }
        }

        public T GetCachedItem(string key)
        {
            var itemKey = GetCacheKey(key);
            if (HttpContext.Current != null && HttpContext.Current.Cache != null)
            {
                var item = HttpContext.Current.Cache[itemKey];
                return item == null ? default(T) : (T) item;
            }
            else
            {
                lock (ThisLock)
                {
                    if (InMemoryItems.ContainsKey(itemKey))
                    {
                        var cachedItem = InMemoryItems[itemKey];
                        if (DateTime.Now > cachedItem.Value)
                        {
                            InMemoryItems.Remove(itemKey);
                            return default(T);
                        }

                        return cachedItem.Key;
                    }
                    return default(T);
                }
            }
        }

        public void AddCachedItem(T item, string key, int? timeoutInMinutes)
        {
            if (HttpContext.Current != null && HttpContext.Current.Cache != null)
            {
                HttpContext.Current.Cache.Add(GetCacheKey(key), item, null,
                                              timeoutInMinutes == null
                                                  ? Cache.NoAbsoluteExpiration
                                                  : DateTime.Now.AddMinutes(timeoutInMinutes.Value), TimeSpan.Zero,
                                              CacheItemPriority.Normal, null);
            }
            else
            {
                lock (ThisLock)
                {
                    var itemKey = GetCacheKey(key);
                    var cachceRec = new KeyValuePair<T, DateTime>(item,
                                                                  timeoutInMinutes == null
                                                                      ? DateTime.MaxValue
                                                                      : DateTime.Now.AddMinutes(timeoutInMinutes.Value));
                    if (InMemoryItems.ContainsKey(itemKey))
                    {
                        InMemoryItems[itemKey] = cachceRec;
                    }
                    else
                    {
                        InMemoryItems.Add(itemKey, cachceRec);
                    }
                }
            }
        }

        private static string GetCacheKey(string key)
        {
            return string.Concat(typeof (T).ToString(), "_", key);
        }
    }
}