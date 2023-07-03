using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http;
public interface IHttpService
{
    Task HandleHttpRequest(HttpListenerRequest request, HttpListenerResponse response);
}
