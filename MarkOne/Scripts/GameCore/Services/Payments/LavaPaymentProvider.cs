using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Shop;
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
    private static string botName => BotController.botname;

    public PaymentProviderType providerType => PaymentProviderType.LAVA_RU;
    private readonly PaymentsSettings settings;

    public LavaPaymentProvider(PaymentsSettings _paymentsSettings)
    {
        settings = _paymentsSettings;
    }

    public async Task<string?> CreatePayment(GameSession session, ShopItemBase shopItem, PaymentData paymentData)
    {
        try
        {
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
                jsonWriter.WriteValue($"date-{DateTime.UtcNow}-bot-{botName}-payment-{paymentData.paymentId}");
                jsonWriter.WritePropertyName("expire");
                jsonWriter.WriteValue(settings.expireTimeInMinutes);
                jsonWriter.WritePropertyName("customFields");
                jsonWriter.WriteValue(paymentData.vendorCode);
                jsonWriter.WritePropertyName("comment");
                jsonWriter.WriteValue(shopItem.GetTitle(session).RemoveHtmlTags());
                jsonWriter.WriteEndObject();
            }
            var jsonData = jsonBuilder.ToString();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.lava.ru/business/invoice/create");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Signature", GetSignature(jsonData));
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
                            var paymentUrl = jsonReader.ReadAsString();
                            return paymentUrl;
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

    private string GetSignature(string serializeData)
    {
        var encoding = new UTF8Encoding();
        var data = encoding.GetBytes(serializeData);
        var key = encoding.GetBytes(settings.secretKey);
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
