using SimpleHttp;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http;
public class BotHttpListener
{
    private CancellationTokenSource _cts = new();

    public string httpListenerPrefix { get; }
    public byte maxConnectionCount { get; }
    public bool isListening { get; private set; }

    public BotHttpListener(string _httpListenerPrefix, byte _maxConnectionCount)
    {
        httpListenerPrefix = _httpListenerPrefix;
        maxConnectionCount = _maxConnectionCount;
    }

    public bool StartListening()
    {
        if (isListening)
        {
            return false;
        }

        _cts = new CancellationTokenSource();
        HttpServer.ListenAsync(httpListenerPrefix, _cts.Token, HandleHttpRequest, maxHttpConnectionCount: maxConnectionCount);
        Program.logger.Info($"Start http listening on {httpListenerPrefix} (max connections: {maxConnectionCount})");
        isListening = true;
        return true;
    }

    public void StopListening()
    {
        _cts.Cancel();
        isListening = false;
        Program.logger.Info($"Http listening stopped");
    }

    public async Task HandleHttpRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        // TODO
        response.AsText($"Hello friend!\n\nListening on {httpListenerPrefix} (max connections: {maxConnectionCount})");
    }

}
