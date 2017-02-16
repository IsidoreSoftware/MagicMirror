using Nancy;
using System.Collections.Generic;

namespace Isidore.MagicMirror.Infrastructure.Http.FileUploads
{
    public class FileUploadRequest
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public IList<string> Tags { get; set; }

        public HttpFile File { get; set; }
    }
}
