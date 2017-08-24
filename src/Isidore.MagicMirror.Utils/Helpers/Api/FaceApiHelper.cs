using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Isidore.MagicMirror.Utils.Helpers.Api
{
    public class FaceApiHelper : IApiHelper
    {
        public HttpResponse Get(string relativePath, HttpHeaders additionalHeaders = null)
        {
            throw new NotImplementedException();
        }
    }
}
