using Isidore.MagicMirror.API.Controllers;
using System;
using Xunit;

namespace Isidore.MagicMirror.API.Tests
{
    public class FaceControllerTests
    {
        [Fact]
        public void can_create_controller()
        {
            var ctrl = new FacesController();
        }
    }
}
