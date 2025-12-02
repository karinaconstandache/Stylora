using System.Threading.Tasks;

namespace Stylora.Domain.Interfaces
{
    // The contract: "I promise I can take two images and return a result image."
    public interface IVirtualTryOnService
    {
        Task<byte[]> ExecuteTryOnAsync(byte[] personImage, byte[] garmentImage);
    }
}