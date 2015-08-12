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
using DotNetAppStarterKit.SampleMvc.DataProject.Context;
using DotNetAppStarterKit.SampleMvc.DataProject.Entity;

namespace DotNetAppStarterKit.SampleMvc.UnitTests.Extensions
{
    public class FakeDummyDataContext : IDummyDataContext
    {

        public FakeDummyDataContext()
        {
            var context = this;

            context.Thingys = new FakeDbSet<Thingy>();
        }

        public virtual TEntity CreateAndAddEntity<TEntity>() where TEntity : class
        {
            var t = Activator.CreateInstance<TEntity>();
            var dbSet = Set<TEntity>();

            return dbSet.Add(t);
        }

        public IDbSet<Thingy> Thingys { get; private set; }

        /// <summary>
        /// Returns Zero by default.  If you want to return something other than Zero with NSubstitute, E.g. to return 1, use:
        ///     Context.When(call => call.SaveChanges()).DoNotCallBase();
        ///     Context.SaveChanges().ReturnsForAnyArgs(1);
        /// </summary>
        /// <returns></returns>
        public virtual int SaveChanges()
        {
            return 0;
        }

        public virtual IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            var property = GetType().GetProperties().Single(pi => pi.PropertyType == typeof (IDbSet<TEntity>));
            return property.GetValue(this) as IDbSet<TEntity>;
        }
    }
}