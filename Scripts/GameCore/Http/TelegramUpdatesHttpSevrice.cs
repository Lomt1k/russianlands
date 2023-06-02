using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http;
public class TelegramUpdatesHttpSevrice : IHttpService
{
    public async Task HandleHttpRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        Program.logger.Debug("TelegramUpdatesHttpSevrice catched request!");
        Program.logger.Debug("\tRawUrl: " + request.RawUrl);
        Program.logger.Debug("\tHttpMethod: " + request.HttpMethod);
        Program.logger.Debug("\tRemoteEndPoint: " + request.RemoteEndPoint);
        Program.logger.Debug("\tIsLocal: " + request.IsLocal);
        Program.logger.Debug("\tContentType: " + request.ContentType);
        foreach (var header in request.Headers.AllKeys)
        {
            Program.logger.Debug($"\tHeader | {header} : {request.Headers[header]}");
        }
        if (request.AcceptTypes is not null)
        {
            foreach (var acceptType in request.AcceptTypes)
            {
                Program.logger.Debug($"\tAcceptType: {acceptType}");
            }
        }
        foreach (var query in request.QueryString.AllKeys)
        {
            Program.logger.Debug($"\tQuery | {query} : {request.QueryString[query]}");
        }
        if (request.HasEntityBody)
        {
            var streamReader = new StreamReader(request.InputStream);
            var updateJson = streamReader.ReadToEnd();
            Program.logger.Debug("\tContent: \n" + updateJson);
        }

        response.Close();
    }
}
