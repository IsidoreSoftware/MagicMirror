using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Exceptions;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using Isidore.MagicMirror.ImageProcessing.Helpers;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Microsoft.Extensions.Logging;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services
{
    public class AzureFaceRecognitionService : IFaceRecognitionService<Stream, User>
    {
        private readonly IFaceServiceClient _faceServiceClient;
        private readonly IUserService _userService;
        private readonly PersonGroup _localFaceGroup;
        private readonly ILogger<AzureFaceRecognitionService> _logger;

        public AzureFaceRecognitionService(
            IFaceServiceClient faceServiceClient,
            IUserService userService,
            ILoggerFactory loggerFactory,
            IUserGroupService userGroupService)
        {
            _faceServiceClient = faceServiceClient;
            _logger = new Logger<AzureFaceRecognitionService>(loggerFactory);
            _userService = userService;
            var currentUserGroup = userGroupService.GetCurrentUserGroup().Result;

            try
            {
                _localFaceGroup = _faceServiceClient.GetPersonGroupAsync(currentUserGroup.Id).Result;
            }
            catch (Exception e)
            {
                _faceServiceClient.CreatePersonGroupAsync(currentUserGroup.Id.ToString(), currentUserGroup.GroupName);
                _logger.LogError($"Can't get the local face group. {e.Message}", e);
                throw;
            }
        }

        public async Task<RecognitionResult<User>> RecognizeAsync(Stream image)
        {
            Face[] detectedFaces;
            try
            {
                detectedFaces = await _faceServiceClient.DetectAsync(image);
            }
            catch (FaceAPIException e)
            {
                _logger.LogError($"Can't identify a user. Reason: {e.Message}", e);
                return null;
            }

            var biggestFace = GetTheBiggestFace(detectedFaces);
            IdentifyResult[] identificationResult;
            try
            {
                identificationResult = await _faceServiceClient.IdentifyAsync(
                    _localFaceGroup.PersonGroupId,
                    new[] { biggestFace.FaceId });

                if (identificationResult[0]?.Candidates?.Length == 0)
                {
                    _logger.LogInformation("The face can't be assigned to any of known persons.");
                    return NoUserFound(biggestFace);
                }
            }
            catch (FaceAPIException e)
            {
                _logger.LogError($"Can't identify a user. Reason: {e.Message}", e);
                return NoUserFound(biggestFace);
            }

            var probableResult = GetMostLikelyPerson(identificationResult);
            var userGuid = probableResult.PersonId.ToString("N");
            var filter = new UserFilter
            {
                UserGuid = userGuid
            };

            var user = (await _userService.GetFilteredAsync(filter)).SingleOrDefault();

            if (user==null)
                throw new RecognizedNotExistingUserException(userGuid);

            _logger.LogTrace($"User (Guid:{user.UserGuid}) recognized. Confidence = {probableResult.Confidence}");
            return new RecognitionResult<User>
            {
                Area = new Area
                {
                    Height = biggestFace.FaceRectangle.Height,
                    Width = biggestFace.FaceRectangle.Width,
                    Top = biggestFace.FaceRectangle.Top,
                    Left = biggestFace.FaceRectangle.Left,
                },
                Distance = probableResult.Confidence,
                RecognizedItem = user
            };
        }

        private static RecognitionResult<User> NoUserFound(Face biggestFace)
        {
            return new RecognitionResult<User>
            {
                Area = new Area
                {
                    Height = biggestFace.FaceRectangle.Height,
                    Width = biggestFace.FaceRectangle.Width,
                    Top = biggestFace.FaceRectangle.Top,
                    Left = biggestFace.FaceRectangle.Left,
                }
            };
        }

        private Candidate GetMostLikelyPerson(IdentifyResult[] identificationResult)
        {
            return identificationResult.First().Candidates.MaxBy(x => x.Confidence);
        }

        private static Face GetTheBiggestFace(Face[] detectedFaces)
        {
            return detectedFaces.MaxBy(x => x.FaceRectangle.Height + x.FaceRectangle.Width);
        }

        public RecognitionResult<User> Recognize(Stream image)
        {
            return RecognizeAsync(image).Result;
        }

        public async Task LearnMore(IDictionary<User, IEnumerable<Stream>> imagesWithLabels)
        {
            try
            {
                foreach (var user in imagesWithLabels)
                foreach (var stream in user.Value)
                {
                    await _faceServiceClient.AddPersonFaceAsync(_localFaceGroup.PersonGroupId,
                        new Guid(user.Key.UserGuid), stream);
                    await _faceServiceClient.TrainPersonGroupAsync(_localFaceGroup.PersonGroupId);
                }

            }
            catch (FaceAPIException e)
            {
                _logger.LogWarning($"Error when learning face. Code:{e.ErrorCode}; Message:{e.ErrorMessage}; Status: {e.HttpStatus}",e);
                throw new FaceNotFoundException();
            }
            catch (Exception e)
            {
                _logger.LogError($"Erren when learning new faces: {e.Message}", e);
                throw;
            }
        }
    }
}
