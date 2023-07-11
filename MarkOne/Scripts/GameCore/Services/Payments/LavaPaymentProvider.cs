using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Shop;
using Newtonsoft.Json;
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

    public LavaPaymentProvider(PaymentsSettings _paymentsSettings)
    {
        settings = _paymentsSettings;
    }

    public async Task<bool> CreatePayment(GameSession session, ShopItemBase shopItem, PaymentData paymentData)
    {
        var jsonBuilder = new StringBuilder();
        using (var textWriter = new StringWriter(jsonBuilder))
        {
            using var jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("sum");
            jsonWriter.WriteValue(paymentData.rubbles);
            jsonWriter.WritePropertyName("shopId");
            jsonWriter.WriteValue(settings.shopId);
            jsonWriter.WritePropertyName("orderId");
            jsonWriter.WriteValue(paymentData.paymentId.ToString());
            jsonWriter.WritePropertyName("expire");
            jsonWriter.WriteValue(settings.expireTimeInMinutes);
            jsonWriter.WritePropertyName("customFields");
            jsonWriter.WriteValue(paymentData.vendorCode);
            jsonWriter.WritePropertyName("comment");
            jsonWriter.WriteValue(shopItem.GetTitle(session).RemoveHtmlTags());
            jsonWriter.WriteEndObject();
        }
        var jsonData = jsonBuilder.ToString();
        Program.logger.Debug("Payment request data:\n" + jsonData);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.lava.ru/business/invoice/create");
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Signature", GetSignature(jsonData));
        request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await BotController.httpClient.SendAsync(request).FastAwait();
        Program.logger.Debug($"Response code: {response.StatusCode}");
        var content = await response.Content.ReadAsStringAsync().FastAwait();
        Program.logger.Debug($"Response content: {content}");

        return response.IsSuccessStatusCode;
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
