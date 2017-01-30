using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CrossQuery;

using Xunit;

namespace CrossQueryFull.Test
{
    /// <summary>
    ///  class in the service itself
    /// </summary>
    public class TestClass
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public int CrossReferenceClassId { get; set; }

        public CrossReferenceClass CrossReferenceClass { get; set; }

        public IEnumerable<CrossReferenceClass2> CrossReferenceClass2es { get; set; }
    }

    /// <summary>
    /// class in another service
    /// </summary>
    public class CrossReferenceClass
    {
        public int Id { get; set; }

        public string Caption { get; set; }

    }

    /// <summary>
    /// class in another service
    /// </summary>
    public class CrossReferenceClass2
    {
        public int Id { get; set; }

        public string Caption { get; set; }

        public int TestClassId { get; set; }
    }

    public class Class1
    {
        [Fact]
        public void Test()
        {
            var queryProvider = new StringQueryProvider();
            var query = new Query<TestClass>(queryProvider);

            IQueryable<TestClass> testClassQuery = new Query<TestClass>(queryProvider);
            IQueryable<CrossReferenceClass> crossReferenceClassQuery = null;
            IQueryable<CrossReferenceClass2> crossReferenceClass2Query = null;

            var results = testClassQuery.Where(x => x.Number == 1).IncludeCrossSingle(x => x.CrossReferenceClassId, (CrossReferenceClass x) => x.Id).ToList();
            //var crossResults =
            //    crossReferenceClassQuery.Where(x => results.Select(r => r.CrossReferenceClassId).Contains(x.Id));
            //results.ForEach(
            //    x =>
            //    {
            //        x.CrossReferenceClass = crossResults.FirstOrDefault(c => c.Id == x.CrossReferenceClassId);
            //    });

            var crossResults2 = crossReferenceClass2Query.Where(x => results.Select(r => r.Id).Contains(x.TestClassId));
            results.ForEach(
                x =>
                {
                    x.CrossReferenceClass2es = crossResults2.Where(c => c.TestClassId == x.Id);
                });




            query.Where(x => x.Number == 1).ToList();
        }
    }

    public static class CrossReferenceExtensions
    {
        //public static IQueryable<T> IncludeCrossSingle<T, TForeignKey, TCross, TCrossKey>(
        //    this IQueryable<T> query,
        //    Expression<Func<T, TForeignKey>> foreignKey,
        //    Expression<Func<TCross, TCrossKey>> crossKey)
        //{

        //}

        public static IQueryable<T> IncludeCrossSingle<T, TCross, TKey>(
            this IQueryable<T> query,
            IQueryable<TCross> crossQuery,
            Expression<Func<T, TKey>> foreignKey,
            Expression<Func<TCross, TKey>> crossKey)
        {
            Expression extendedExpression;
            var queryExpression = query.Expression as MethodCallExpression;

            var methodInfo = (MethodInfo)MethodInfo.GetCurrentMethod();
            var genericMethodInfo = methodInfo.MakeGenericMethod(typeof(T), typeof(TCross), typeof(TKey));

            extendedExpression = Expression.Call(null, genericMethodInfo, queryExpression, foreignKey, crossKey);

            return new Query<T>(query.Provider, extendedExpression);
        }

        public static IQueryable<T> IncludeCrossMulti<T, TCross, TKey>(
               this IQueryable<T> query,
               IQueryable<TCross> crossQuery,
               Expression<Func<T, TKey>> key,
               Expression<Func<TCross, TKey>> crossForeignKey)
        {
            Expression extendedExpression;
            var queryExpression = query.Expression as MethodCallExpression;

            var methodInfo = (MethodInfo)MethodInfo.GetCurrentMethod();
            var genericMethodInfo = methodInfo.MakeGenericMethod(typeof(T), typeof(TCross), typeof(TKey));

            extendedExpression = Expression.Call(null, genericMethodInfo, queryExpression, key, crossForeignKey);

            return new Query<T>(query.Provider, extendedExpression);
        }

        public static void OnIncludeCrossSingle<T, TCross, TKey>(
            IEnumerable<T> originResults,
            IQueryable<TCross> crossQuery,
            Expression<Func<T, TKey>> foreignKey,
            Expression<Func<TCross, TKey>> crossKey)
        {
            var foreignKeyGetter = foreignKey.Compile();
            var crossKeyGetter = crossKey.Compile();
            var originCrossIds = originResults.Select(foreignKeyGetter);
            
            var crossResults = crossQuery.Where(x => originCrossIds.Contains((TKey)crossKeyGetter(x))).ToArray();
            foreach (var result in originResults)
            {
                result.
            }
        }
    }
}
