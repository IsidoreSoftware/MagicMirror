using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Isidore.MagicMirror.Utils.Helpers.Api
{
    public interface IApiHelper
    {
        HttpResponse Get(string relativePath, HttpHeaders additionalHeaders = null);
    }
}