using MarkOne.Scripts.Bot;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace MarkOne.Scripts.GameCore.Http;
public class TelegramUpdatesHttpSevrice : IHttpService
{
    private const string SECRET_TOKEN_HEADER = "X-Telegram-Bot-Api-Secret-Token";

    private readonly string _secretToken;
    private readonly DateTime _strartUtcTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    private readonly TelegramBotUpdateHandler _updateHandler = new();


    public TelegramUpdatesHttpSevrice(string secretToken)
    {
        _secretToken = secretToken;
    }

    public Task HandleHttpRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var token = request.Headers[SECRET_TOKEN_HEADER];
        if (token is null || !token.Equals(_secretToken))
        {
            Program.logger.Error("TelegramUpdatesHttpSevrice: Incorrect secret token!");
            response.StatusCode = 404;
            response.Close();
            return Task.CompletedTask;
        }

        var streamReader = new StreamReader(request.InputStream);
        var updateJson = streamReader.ReadToEnd();
        // Program.logger.Debug("updateJson:\n" + updateJson);
        var update = DeserializeUpdate(updateJson);
        response.Close();
        if (update is not null)
        {
            _updateHandler.HandleSimpleUpdate(update);
        }
        return Task.CompletedTask;
    }

    // Легковесная десериализация, которая читает из json только то, что нам реально нужно
    // Message updates: в 3-4 раза быстрее, чем JsonConvert.DeserializeObject<Update>
    // CallbackQuery updates: в 5-6 раз быстрее, чем JsonConvert.DeserializeObject<Update>
    private SimpleUpdate? DeserializeUpdate(string updateJson)
    {
        var reader = new JsonTextReader(new StringReader(updateJson));
        var update = new SimpleUpdate();

        try
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var key = reader.Value.ToString();
                    switch (key)
                    {
                        case "update_id":
                            update.id = reader.ReadAsInt32() ?? 0;
                            break;
                        case "message":
                            update.updateType = UpdateType.Message;
                            update.message = ReadMessage(reader);
                            break;
                        case "callback_query":
                            update.updateType = UpdateType.CallbackQuery;
                            update.callbackQuery = ReadCallbackQuery(reader);
                            break;
                    }
                }
            }
            return update;
        }
        catch (Exception ex)
        {
            Program.logger.ErrorFormat("Catched exception on deserialize update...\n{0}", ex);
            return null;
        }
    }

    private SimpleMessage ReadMessage(JsonTextReader reader)
    {
        var message = new SimpleMessage();
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.PropertyName)
            {
                var key = reader.Value.ToString();
                switch (key)
                {
                    case "message_id":
                        message.id = reader.ReadAsInt32() ?? 0;
                        break;
                    case "from":
                        message.from = ReadUser(reader);
                        break;
                    case "date":
                        var timestamp = reader.ReadAsDouble() ?? 0;
                        message.date = _strartUtcTime.AddSeconds(timestamp).ToLocalTime();
                        break;
                    case "text":
                        message.text = reader.ReadAsString();
                        break;
                    case "document":
                        message.document = ReadDocument(reader);
                        break;
                }
            }
        }
        return message;
    }

    private SimpleCallbackQuery ReadCallbackQuery(JsonTextReader reader)
    {
        var callbackQuery = new SimpleCallbackQuery();
        bool isFromReaded = false;
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.PropertyName)
            {
                var key = reader.Value.ToString();
                switch (key)
                {
                    case "id":
                        if (string.IsNullOrEmpty(callbackQuery.id))
                        {
                            callbackQuery.id = reader.ReadAsString() ?? string.Empty;
                        }
                        break;
                    case "from":
                        if (!isFromReaded)
                        {
                            callbackQuery.from = ReadUser(reader);
                            isFromReaded = true;
                        }                        
                        break;
                    case "data":
                        if (string.IsNullOrEmpty(callbackQuery.data))
                        {
                            callbackQuery.data = reader.ReadAsString();
                        }
                        break;
                }
            }
        }
        return callbackQuery;
    }

    private SimpleUser ReadUser(JsonTextReader reader)
    {
        var user = new SimpleUser();
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return user;
            }
            if (reader.TokenType == JsonToken.PropertyName)
            {
                var key = reader.Value.ToString();
                switch (key)
                {
                    case "id":
                        user.id = long.Parse(reader.ReadAsString());
                        break;
                    case "first_name":
                        user.firstName = reader.ReadAsString() ?? "Unknown";
                        break;
                    case "last_name":
                        user.lastName = reader.ReadAsString();
                        break;
                    case "username":
                        user.username = reader.ReadAsString();
                        break;
                }
            }
        }
        return user;
    }

    private SimpleDocument ReadDocument(JsonTextReader reader)
    {
        var document = new SimpleDocument();
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return document;
            }
            if (reader.TokenType == JsonToken.PropertyName)
            {
                var key = reader.Value.ToString();
                switch (key)
                {
                    case "file_size":
                        document.fileSize = long.Parse(reader.ReadAsString() ?? "-1");
                        break;
                    case "file_name":
                        document.fileName = reader.ReadAsString();
                        break;
                    case "file_id":
                        document.fileId = reader.ReadAsString() ?? string.Empty;
                        break;
                    case "file_unique_id":
                        document.fileUniqueId = reader.ReadAsString() ?? string.Empty;
                        break;
                    case "mime_type":
                        document.mimeType = reader.ReadAsString();
                        break;
                }
            }
        }
        return document;
    }


}
