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
using System.Linq;

namespace DotNetAppStarterKit.Mapping
{
    public static class MappingSetup
    {
        public static void AssertConfigurationIsValidInAllMappers()
        {
            var exceptions = new List<Exception>();
            foreach (var currAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var mapperTypes = from x in currAssembly.GetTypes()
                                  let y = x.BaseType
                                  where !x.IsAbstract && !x.IsInterface &&
                                        y != null && y.IsGenericType &&
                                        y.GetGenericTypeDefinition() == typeof (MapperBase<,>)
                                  select x;

                foreach (var currType in mapperTypes)
                {
                    dynamic inst = Activator.CreateInstance(currType);
                    try
                    {
                        inst.EnsureMapExists();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(
                    "One or more classes that inherit MapperBase<,> have failed configuration assertion.", exceptions);
            }
        }
    }
}