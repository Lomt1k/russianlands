using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.Payments;
internal class LavaPaymentsHttpWebhookService : IHttpService
{
    public async Task HandleHttpRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        using var streamReader = new StreamReader(request.InputStream);
        var json = await streamReader.ReadToEndAsync().FastAwait();
        var query = request.QueryString;
        Program.logger.Debug("lava webhook works...");
        foreach (var key in query.AllKeys)
        {
            Program.logger.Debug($"{key} : {query[key]}");
        }
        Program.logger.Debug($"webhook json:\n{json}");

        response.StatusCode = (int)HttpStatusCode.OK;
        response.Close();
    }
}
