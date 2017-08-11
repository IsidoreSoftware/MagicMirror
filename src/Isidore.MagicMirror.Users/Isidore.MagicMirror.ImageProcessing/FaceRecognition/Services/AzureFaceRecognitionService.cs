﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Models;
using Isidore.MagicMirror.ImageProcessing.Helpers;
using Isidore.MagicMirror.Users.Contract;
using Isidore.MagicMirror.Users.Models;
using Microsoft.Extensions.Logging;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using MongoDB.Driver.Core.Operations;
using NLog;

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
                _localFaceGroup = _faceServiceClient.GetPersonGroupAsync(currentUserGroup.Id.ToString()).Result;
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
            }
            catch (FaceAPIException e)
            {
                _logger.LogError($"Can't identify a user. Reason: {e.Message}", e);
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

            var probableResult = GetMostLikelyPerson(identificationResult);

            var user = await _userService.GetByGuid(probableResult.PersonId);

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
                    if (!user.Key.UserGuid.HasValue)
                    {
                       var result = await _faceServiceClient.CreatePersonAsync(_localFaceGroup.PersonGroupId,
                            $"{user.Key.FirstName} {user.Key.LastName}");
                        user.Key.UserGuid = result.PersonId;
                        _userService.Update(user.Key.Id, user.Key);
                    }
                    await _faceServiceClient.AddPersonFaceAsync(_localFaceGroup.PersonGroupId,
                        user.Key.UserGuid.Value, stream);
                    await _faceServiceClient.TrainPersonGroupAsync(_localFaceGroup.PersonGroupId);
                }

            }
            catch (Exception e)
            {
                _logger.LogError($"Erren when learning new faces: {e.Message}", e);
                throw;
            }
        }
    }
}