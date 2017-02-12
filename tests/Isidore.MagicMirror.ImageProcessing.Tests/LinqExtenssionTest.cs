using System.Collections.Generic;
using System.Linq;
using Xunit;
using Isidore.MagicMirror.ImageProcessing.Helpers;

namespace Isidore.MagicMirror.ImageProcessing.Tests
{
    public class LinqExtenssionTest
    {
        [Fact]
        public void max_by_test()
        {
            var max = new KeyValuePair<int, int>(4, 5);
            var min = new KeyValuePair<int, int>(1, 1);
            var collection = new KeyValuePair<int, int>[] {
                new KeyValuePair<int,int>(1,6),
                new KeyValuePair<int,int>(6,1),
            }.ToList();
            collection.Add(max);
            collection.Add(min);

            var res = collection.MaxBy(x => x.Key + x.Value);

            Assert.Equal(max, res);
        }
    }
}