using System.Threading.Tasks;
using Zap.Identity.Application.DTOs;

namespace Zap.Identity.Application.Interfaces;

public interface IResourceService
{
    Task<SetupMetadataDto> GetSetupMetadataAsync(string languageCode = "en");
}
