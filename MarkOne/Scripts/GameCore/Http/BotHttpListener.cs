﻿using SimpleHttp;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http;
public class BotHttpListener
{
    private CancellationTokenSource _cts = new();
    private readonly Dictionary<string, IHttpService> _httpServices = new();

    public string httpListenerPrefix { get; }
    public string externalHttpListenerPrefix { get; }
    public byte maxConnectionCount { get; }
    public bool onlyLocalRequests { get; }
    public bool isListening { get; private set; }

    public BotHttpListener(Bot.BotConfig.HttpListenerSettings settings)
    {
        httpListenerPrefix = settings.httpPrefix;
        externalHttpListenerPrefix = settings.externalHttpPrefix;
        maxConnectionCount = settings.maxConnections;
        onlyLocalRequests = settings.onlyLocalRequests;
    }

    public bool StartListening()
    {
        if (isListening)
        {
            return false;
        }

        _cts = new CancellationTokenSource();
        HttpServer.ListenAsync(httpListenerPrefix, _cts.Token, HandleHttpRequest, maxHttpConnectionCount: maxConnectionCount);
        Program.logger.Info($"Start http listening on {httpListenerPrefix}");
        isListening = true;
        return true;
    }

    public void StopListening()
    {
        _cts.Cancel();
        isListening = false;
        Program.logger.Info($"Http listening stopped");
    }

    public void RegisterHttpService(string localPath, IHttpService httpService)
    {
        // force replace old service if exists
        _httpServices[localPath] = httpService;
    }

    private async Task HandleHttpRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        try
        {
            response.ContentEncoding = Encoding.UTF8;
            if (onlyLocalRequests && !request.IsLocal)
            {
                response.StatusCode = 404;
                response.Close();
                return;
            }

            var localPath = request.Url?.LocalPath;
            if (localPath is not null)
            {
                if (_httpServices.TryGetValue(localPath, out var httpService))
                {
                    await httpService.HandleHttpRequest(request, response).FastAwait();
                    return;
                }
            }

            response.StatusCode = 404;
            response.Close();

            //Program.logger.WarnFormat("Unhandled HTTP Request!\n\tRawUrl: {0}\n\tRemoteEndPoint: {1}\n\tIsLocalRequest: {2}", request.RawUrl, request.RemoteEndPoint, request.IsLocal);
        }
        catch (System.Exception ex)
        {
            Program.logger.Error($"Catched exception on handle HTTP Request\n\tRawUrl: {request.RawUrl}\n{ex}");
        }
    }

}
