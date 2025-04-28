using Microsoft.AspNetCore.Http;

namespace Grad_Api.Models.Quiz
{
    public class FileUploadRequest
    {
        public IFormFile File { get; set; }
    }
} 