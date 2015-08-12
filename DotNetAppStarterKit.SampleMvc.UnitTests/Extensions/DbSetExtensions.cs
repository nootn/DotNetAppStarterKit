using System.Collections.Generic;
using System.Data.Entity;

namespace DotNetAppStarterKit.SampleMvc.UnitTests.Extensions
{
    public static class DbSetExtensions
    {
        public static void AddRange<T>(this IDbSet<T> dbSet, IEnumerable<T> items) where T : class
        {
            foreach (T item in items)
                dbSet.Add(item);
        }
    }
}