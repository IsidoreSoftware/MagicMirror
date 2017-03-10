using FakeItEasy;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Isidore.MagicMirror.DAL.MongoDb.Tests
{
    public static class MongoTestsHelpers
    {
        public static void ReturnsCollection<T>(this IMongoCollection<T> collection,
            IEnumerable<T> enumerable)
        {
            var cursor = A.Fake<IAsyncCursor<T>>();
            var enumerator = enumerable.GetEnumerator();
            A
                .CallTo(() => cursor.MoveNextAsync(A<CancellationToken>.Ignored))
                .ReturnsLazily(() =>
                {
                    return Task.FromResult(enumerator.MoveNext());
                });

            A
                .CallTo(() => cursor.Current)
                .ReturnsLazily(() =>
                {
                    return new List<T>() { enumerator.Current };
                });

            A
                .CallTo(() => collection.FindAsync<T>(A<FilterDefinition<T>>.Ignored, A<FindOptions<T, T>>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(cursor));
        }

        public static IAsyncCursor<T> GetCursor<T>(IEnumerable<T> enumerable)
        {
            var cursor = A.Fake<IAsyncCursor<T>>();
            var enumerator = enumerable.GetEnumerator();
            A
                .CallTo(() => cursor.MoveNextAsync(A<CancellationToken>.Ignored))
                .ReturnsLazily(() =>
                {
                    return Task.FromResult(enumerator.MoveNext());
                });
            A
                 .CallTo(() => cursor.MoveNext(A<CancellationToken>.Ignored))
                 .ReturnsLazily(() =>
                 {
                     return enumerator.MoveNext();
                 });

            A
                .CallTo(() => cursor.Current)
                .ReturnsLazily(() =>
                {
                    return new List<T>() { enumerator.Current };
                });
            

            return cursor;
        }
    }
}
