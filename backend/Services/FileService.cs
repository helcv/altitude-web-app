using backend.DTOs;
using backend.Interfaces;
using CSharpFunctionalExtensions;

namespace backend.Services
{
    public class FileService : IFileService
    {
        private readonly string _uploadFolderPath;

        public FileService(IConfiguration config)
        {
            _uploadFolderPath = config["UploadFolderPath"];
            if (!Directory.Exists(_uploadFolderPath))
            {
                Directory.CreateDirectory(_uploadFolderPath);
            }
        }

        public void DeleteFile(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return;

            string fileName = Path.GetFileName(fileUrl);
            string filePath = Path.Combine(_uploadFolderPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task<Result<string, MessageDto>> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Result.Failure<string, MessageDto>(new MessageDto { Message = "Invalid file." });
            }

            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(_uploadFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Result.Success<string, MessageDto>($"/photos/{fileName}");
        }
    }
}
