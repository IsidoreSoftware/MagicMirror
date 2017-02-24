using FakeItEasy;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Isidore.MagicMirror.ImageProcessing.Tests
{
    public static class TestMocker
    {
        public static IFileProvider MockFileProvider(string file)
        {
           IFileProvider fileProvider = A.Fake<IFileProvider>();
            IFileInfo fi = A.Fake<IFileInfo>();
            var realFile = new FileInfo(file).FullName;
            A.CallTo(() => fi.PhysicalPath)
                .Returns(realFile);
            A.CallTo(() => fi.Exists)
                .Returns(true);
            A.CallTo(() => fileProvider.GetFileInfo(A<string>.Ignored))
                .Returns(fi);
            return fileProvider;
        }
    }
}
