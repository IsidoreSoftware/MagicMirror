using Isidore.MagicMirror.Widgets.Contract;
using Isidore.MagicMirror.Widgets.Services;
using System;
using Xunit;

namespace Isidore.MagicMirror.Widgets.Tests
{
    public class WidgetsServiceTests
    {
        IWidgetService service;

        public WidgetsServiceTests()
        {
            service= new WidgetService(null);
        }

        [Fact]
        public void Test1()
        {
        }
    }
}
