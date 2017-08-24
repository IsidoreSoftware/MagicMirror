using System;
using System.Threading.Tasks;
using FakeItEasy;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.Users.Services;
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

        public AzureUserServiceTests()
        {
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
            var azureUserService = new AzureUserService(_faceServiceClient, _userGroupService);
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
            var azureUserService = new AzureUserService(_faceServiceClient, _userGroupService);
            var user = new User{
                Id="abcd-e2324"
            };

            // Act
            await azureUserService.InsertAsync(user);

            // Assert

            A.CallTo(
                    () => _faceServiceClient.CreatePersonAsync(A<string>.That.IsEqualTo(DefaulGroupId),
                        A<string>.Ignored, A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task when_user_is_searched_its_guid_is_used_instead_of_normal_id()
        {
            // Arrange
            var azureUserService = new AzureUserService(_faceServiceClient, _userGroupService);
            var user = new User{Id = "test"};

            // Act
            await azureUserService.InsertAsync(user);
            await azureUserService.GetByIdAsync(user.Id);

            // Assert
            A.CallTo(
                    () => _faceServiceClient.GetPersonAsync(
                        A<string>.Ignored,A<Guid>.That.Matches(x=>x.Equals(Guid.Parse(user.UserGuid)))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        
        [Fact]
        public async Task user_must_have_id_specified_when_inserting_to_services()
        {
            // Arrange
            var azureUserService = new AzureUserService(_faceServiceClient, _userGroupService);
            var user = new User{
                Id = null
            };

            // Act
            await Assert.ThrowsAsync<ArgumentException>(() => azureUserService.InsertAsync(user));            
        }
    }
}
