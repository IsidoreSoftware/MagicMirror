using System;
using System.Threading.Tasks;
using FakeItEasy;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.Users.Services;
using Microsoft.Extensions.Logging;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Xunit;

namespace Isidore.MagicMirror.ImageProcessing.Tests.Azure
{
    public class AzureUserServiceTests
    {
        private const string DefaulGroupId = "test_group_identifier";
        private readonly IFaceServiceClient _faceServiceClient;
        private readonly IUserGroupService _userGroupService;
        private readonly ILogger<AzureUserService> _logger;

        public AzureUserServiceTests()
        {
            _logger = A.Fake<ILogger<AzureUserService>>();
            _faceServiceClient = A.Fake<IFaceServiceClient>();
            _userGroupService = A.Fake<IUserGroupService>();
            A.CallTo(() => _userGroupService.GetCurrentUserGroup()).Returns(new UserGroup
            {
                GroupName = "TestGroup",
                Id = DefaulGroupId
            });
        }

        [Fact]
        public async Task inserting_user_sets_up_its_guid()
        {
            // Arrange
            var testGuid = Guid.NewGuid();
            var azureUserService = new AzureUserService(_faceServiceClient, _userGroupService, _logger);
            A.CallTo(
                    () => _faceServiceClient.CreatePersonAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(new CreatePersonResult { PersonId = testGuid });
            var user = new User { Id = "abcde" };

            //Act
            await azureUserService.InsertAsync(user);

            Assert.Equal(testGuid.ToString(), user.UserGuid);
        }

        [Fact]
        public async Task inserting_user_using_default_group()
        {
            // Arrange
            var azureUserService = new AzureUserService(_faceServiceClient, _userGroupService, _logger);
            var user = new User();

            // Act
            await azureUserService.InsertAsync(user);

            // Assert

            A.CallTo(() => _faceServiceClient.CreatePersonInPersonGroupAsync(A<string>.That.IsEqualTo(DefaulGroupId),
                        A<string>.Ignored, A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task when_user_is_searched_its_guid_is_used_instead_of_normal_id()
        {
            // Arrange
            var azureUserService = new AzureUserService(_faceServiceClient, _userGroupService, _logger);
            var user = new User { Id = "test" };

            // Act
            await azureUserService.InsertAsync(user);
            await azureUserService.GetByIdAsync(user.Id);

            // Assert
            A.CallTo(
                    () => _faceServiceClient.GetPersonAsync(
                        A<string>.Ignored, A<Guid>.That.Matches(x => x.Equals(Guid.Parse(user.UserGuid)))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
