using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static MarkOne.Scripts.Bot.BotConfig;

namespace MarkOne.Scripts.GameCore.Services.Payments;
internal class LavaPaymentProvider : IPaymentProvider
{
    public PaymentProviderType providerType => PaymentProviderType.LAVA_RU;
    private readonly PaymentsSettings settings;

    private string defaultWebHookPath { get; }

    public LavaPaymentProvider(PaymentsSettings _paymentsSettings)
    {
        settings = _paymentsSettings;
        defaultWebHookPath = BotController.config.httpListenerSettings.externalHttpPrefix.TrimEnd('/') + _paymentsSettings.webhookPath;
    }

    public async Task<CreatedPaymentInfo?> CreatePayment(GameSession session, PaymentData paymentData)
    {
        try
        {
            var hookSignature = GetSignature(new Random().Next(1_000_000_000).ToString(), settings.secondaryKey);
            var hookUrl = $"{defaultWebHookPath}?signature={hookSignature}";

            var jsonBuilder = new StringBuilder();
            using (var textWriter = new StringWriter(jsonBuilder))
            {
                using var jsonWriter = new JsonTextWriter(textWriter);
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("sum");
                //jsonWriter.WriteValue(paymentData.rubbles);
                jsonWriter.WriteValue(10); // debug only!
                jsonWriter.WritePropertyName("shopId");
                jsonWriter.WriteValue(settings.shopId);
                jsonWriter.WritePropertyName("orderId");
                jsonWriter.WriteValue(paymentData.orderId);
                jsonWriter.WritePropertyName("expire");
                jsonWriter.WriteValue(settings.expireTimeInMinutes);
                jsonWriter.WritePropertyName("customFields");
                jsonWriter.WriteValue(paymentData.vendorCode);
                jsonWriter.WritePropertyName("comment");
                jsonWriter.WriteValue(paymentData.comment);
                jsonWriter.WritePropertyName("hookUrl");
                jsonWriter.WriteValue(hookUrl);
                jsonWriter.WriteEndObject();
            }
            var jsonData = jsonBuilder.ToString();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.lava.ru/business/invoice/create");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Signature", GetSignature(jsonData, settings.secretKey));
            request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await BotController.httpClient.SendAsync(request).FastAwait();
            var jsonStream = await response.Content.ReadAsStreamAsync().FastAwait();
            using var streamReader = new StreamReader(jsonStream);
            using var jsonReader = new JsonTextReader(streamReader);

            while (jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonToken.PropertyName)
                {
                    var key = jsonReader.Value.ToString();
                    switch (key)
                    {
                        case "url":
                            var paymentUrl = jsonReader.ReadAsString() ?? string.Empty;
                            return new CreatedPaymentInfo(paymentUrl, hookSignature);
                    }
                }
            }

            var jsonStr = response.Content.ReadAsStringAsync().FastAwait();
            Program.logger.Error($"Error on try to create lava payment...\nStatus: {response.StatusCode}\n JSON from LAVA.RU:\n{jsonStr}");
        }
        catch (Exception ex)
        {
            Program.logger.Error($"Catched exception on try to create lava payment:\n{ex}");
        }
        return null;
    }

    public static string GetSignature(string serializeData, string secretKey)
    {
        var encoding = new UTF8Encoding();
        var data = encoding.GetBytes(serializeData);
        var key = encoding.GetBytes(secretKey);
        var hash = new HMACSHA256(key);
        var hashmessage = hash.ComputeHash(data);

        var signatureBuilder = new StringBuilder();
        for (int i = 0; i < hashmessage.Length; i++)
        {
            signatureBuilder.Append(hashmessage[i].ToString("X2").ToLower());
        }
        return signatureBuilder.ToString();
    }

}
