using System.Threading.Tasks;

namespace OCRDemoApplication.Models
{
    public interface ITextractService
    {
        Task SaveFileToBucketAndGetResponse(string fileName, ApiResult resultPT);
    }
}
