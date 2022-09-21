using System.Threading.Tasks;
using Amazon.Runtime.Internal;

public interface ISESController
{
    Task<bool> SendRawEmailAsync(IAmazonWebServiceRequest request);
}
