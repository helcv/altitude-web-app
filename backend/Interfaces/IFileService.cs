using backend.DTOs;
using CSharpFunctionalExtensions;

namespace backend.Interfaces
{
    public interface IFileService
    {
        Task<Result<string, MessageDto>> UploadFileAsync(IFormFile file);
        void DeleteFile(string fileUrl);
    }
}
