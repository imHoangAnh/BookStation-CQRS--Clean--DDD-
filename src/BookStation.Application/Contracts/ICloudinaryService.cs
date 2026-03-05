using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;


namespace BookStation.Application.Contracts
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task DeleteImageAsync(string publicId);
    }
}

