// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using AutoMapper;

namespace DotNetAppStarterKit.Mapping
{
    public abstract class MapperBase<TSource, TDestination> : IMapper<TSource, TDestination>
    {
        public void EnsureMapExists()
        {
            if (Mapper.FindTypeMapFor<TSource, TDestination>() == null)
            {
                var exp = Mapper.CreateMap<TSource, TDestination>();
                CustomizeMappingExpression(exp);

                var createdMap = Mapper.FindTypeMapFor<TSource, TDestination>();

                Mapper.AssertConfigurationIsValid(createdMap);
            }
        }

        public abstract void CustomizeMappingExpression(IMappingExpression<TSource, TDestination> expression);

        public abstract void SetValuesAfterAutomaticMapping(TSource sourceItem, ref TDestination mappedItem);

        public TDestination Map(TSource originalItem, TDestination newItem)
        {
            EnsureMapExists();
            var mappedItem = Mapper.Map(originalItem, newItem);
            SetValuesAfterAutomaticMapping(originalItem, ref mappedItem);
            return mappedItem;
        }
    }
}