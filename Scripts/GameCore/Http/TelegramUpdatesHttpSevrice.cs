using MarkOne.Scripts.Bot;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MarkOne.Scripts.GameCore.Http;
public class TelegramUpdatesHttpSevrice : IHttpService
{
    private const string SECRET_TOKEN_HEADER = "X-Telegram-Bot-Api-Secret-Token";

    private readonly string _secretToken;
    private readonly DateTime _strartUtcTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public TelegramUpdatesHttpSevrice(string secretToken)
    {
        _secretToken = secretToken;
    }

    public Task HandleHttpRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        Program.logger.Debug("TelegramUpdatesHttpSevrice catched request!");
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
        var update = JsonConvert.DeserializeObject<Update>(updateJson);
        if (update is not null)
        {
            Program.logger.Debug("\t Type: " + update.Type);
            if (update.Message?.Text is not null)
            {
                Program.logger.Debug("\t Message.Text: " + update.Message.Text);
            }
            if (update.CallbackQuery is not null)
            {
                Program.logger.Debug("\t CallbackQuery.Data: " + update.CallbackQuery.Data);
            }
        }

        Program.logger.Debug("updateJson:\n" + updateJson);
        var simpleUpdate = DeserializeUpdate(updateJson);
        
        if (simpleUpdate is not null)
        {
            Program.logger.Debug("mine deserialization:");
            Program.logger.Debug("\t updateType: " + simpleUpdate.updateType);
            if (simpleUpdate.message is not null)
            {
                Program.logger.Debug("\t message.id: " + simpleUpdate.message.id);
                Program.logger.Debug("\t message.date: " + simpleUpdate.message.date);
                Program.logger.Debug("\t message.text: " + simpleUpdate.message.text);
                if (simpleUpdate.message.from is not null)
                {
                    Program.logger.Debug("\t message.from.id: " + simpleUpdate.message.from.id);
                    Program.logger.Debug("\t message.from.firstName: " + simpleUpdate.message.from.firstName);
                    Program.logger.Debug("\t message.from.lastName: " + simpleUpdate.message.from.lastName);
                    Program.logger.Debug("\t message.from.username: " + simpleUpdate.message.from.username);
                }
            }
        }




        response.Close();
        return Task.CompletedTask;
    }

    // Легковесная десериализация, которая читает из json только то, что нам реально нужно
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

    private SimpleMeesage ReadMessage(JsonTextReader reader)
    {
        var message = new SimpleMeesage();
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
                }
            }
        }
        return message;
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


}
