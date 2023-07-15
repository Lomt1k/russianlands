using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Services.Payments;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.Payments;
internal class LavaPaymentsHttpWebhookService : IHttpService
{
    private static readonly PaymentManager paymentManager = ServiceLocator.Get<PaymentManager>();

    public async Task HandleHttpRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        try
        {
            using var streamReader = new StreamReader(request.InputStream);
            var json = await streamReader.ReadToEndAsync().FastAwait();
            var paymentInfo = JsonConvert.DeserializeObject<LavaPaymentInfo>(json);
            if (paymentInfo is null || !paymentInfo.IsSuccess())
            {
                response.Close();
                return;
            }

            var db = BotController.dataBase.db;
            var paymentData = await db.Table<PaymentData>().Where(x => x.orderId == paymentInfo.order_id).FirstOrDefaultAsync().FastAwait();
            if (paymentData is null)
            {
                Program.logger.Error($"Not found PaymentData with orderId: '{paymentInfo.order_id}' (but payment is success). Call the administrator!");
                response.Close();
                return;
            }
            var signature = request.QueryString["signature"];
            var requiredSignature = paymentData.signature;
            if (signature is null || !signature.Equals(requiredSignature))
            {
                response.Close();
                return;
            }

            await paymentManager.HandleSuccessPayment(paymentData).FastAwait();
        }
        catch (Exception ex)
        {
            Program.logger.Error("Catched exception on lava webhook:\n" + ex);
        }
        
        response.Close();
    }
}
