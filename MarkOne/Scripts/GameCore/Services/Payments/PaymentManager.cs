using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Dialogs.Town.Shop;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Services.DailyDataManagers;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Shop.Offers;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MarkOne.Scripts.Bot.BotConfig;

namespace MarkOne.Scripts.GameCore.Services.Payments;
public class PaymentManager : Service
{
    private readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();
    private readonly SessionExceptionHandler sessionExceptionHandler = ServiceLocator.Get<SessionExceptionHandler>();
    private readonly BattleManager battleManager = ServiceLocator.Get<BattleManager>();
    private readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();
    private readonly ServerDailyDataManager serverDailyDataManager = ServiceLocator.Get<ServerDailyDataManager>();

    private bool _isEnabled;
    private PaymentsSettings _settings;
    private IPaymentProvider? _paymentProvider;

    public override Task OnBotStarted()
    {
        _settings = BotController.config.paymentsSettings;
        _isEnabled = _settings.isEnabled;
        if (_isEnabled)
        {
            switch (_settings.paymentProvider)
            {
                case PaymentProviderType.LAVA_RU:
                    _paymentProvider = new LavaPaymentProvider(_settings);
                    break;
            }
        }
        return Task.CompletedTask;
    }

    public async Task<PaymentData?> TryGetOrCreatePayment(GameSession session, double rubblePrice, string vendorCode, string comment, DateTime? expireDate = null)
    {
        var db = BotController.dataBase.db;
        var mixExpireTime = DateTime.UtcNow.AddMinutes(_settings.minExpireTimeToUseOldOrder);
        var telegramId = session.actualUser.Id;

        var paymentData = db.Table<PaymentData>()
            .Where(x => x.telegramId == telegramId && x.vendorCode == vendorCode && x.expireDate >= mixExpireTime && x.status == PaymentStatus.NotPaid)
            .FirstOrDefault();

        if (paymentData is not null)
        {
            return paymentData;
        }
        return await TryCreatePayment(session, rubblePrice, vendorCode, comment).FastAwait();
    }

    private async Task<PaymentData?> TryCreatePayment(GameSession session, double rubblePrice, string vendorCode, string comment, DateTime? expireDate = null)
    {
        if (!_isEnabled || _paymentProvider is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var paymentData = new PaymentData
        {
            orderId = $"{now.Year}-{now.Month}-{now.Day} {now.Hour}:{now.Minute}:{now.Second} (ID {session.actualUser.Id})",
            telegramId = session.actualUser.Id,
            providerType = _paymentProvider.providerType,
            vendorCode = vendorCode,
            status = PaymentStatus.NotPaid,
            rubbles = rubblePrice,
            creationDate = now,
            expireDate = expireDate ?? now.AddMinutes(_settings.expireTimeInMinutes),
            comment = comment
        };

        var createdPaymentInfo = await _paymentProvider.CreatePayment(session, paymentData).FastAwait();
        if (createdPaymentInfo is null)
        {
            return null;
        }
        paymentData.url = createdPaymentInfo.url;
        paymentData.signature = createdPaymentInfo.signature;

        var db = BotController.dataBase.db;
        db.Insert(paymentData);   
        
        return paymentData;
    }

    public async Task HandleSuccessPayment(PaymentData paymentData)
    {
        Program.logger.Info($"PAYMENT | Success payment from {paymentData.providerType}:" +
            $"\n orderId: {paymentData.orderId}" +
            $"\n telegramId: {paymentData.telegramId}" +
            $"\n rubbles: {paymentData.rubbles}" +
            $"\n vendorCode: {paymentData.vendorCode}" +
            $"\n comment: {paymentData.comment}");

        var dailyRevenue = serverDailyDataManager.GetIntegerValue("revenue");
        dailyRevenue += (long)paymentData.rubbles;
        serverDailyDataManager.SetIntegerValue("revenue", dailyRevenue);

        var telegramId = paymentData.telegramId;
        var db = BotController.dataBase.db;

        var session = sessionManager.GetSessionIfExists(telegramId);
        if (session is not null)
        {
            try
            {
                // session exists
                var inBattle = battleManager.GetCurrentBattle(session.player) is not null;
                if (!inBattle)
                {
                    if (!gameDataHolder.shopItemsCache.TryGetValue(paymentData.vendorCode, out var shopItem))
                    {
                        Program.logger.Error($"PAYMENT | Not found item with vendorCode '{paymentData.vendorCode}'" +
                            $"\n orderId: {paymentData.orderId}" +
                            $"\n telegramId: {paymentData.telegramId}" +
                            $"\n rubbles: {paymentData.rubbles}" +
                            $"\n comment: {paymentData.comment}" +
                            $"\n status: {PaymentStatus.ErrorOnTryReceive}" +
                            $"But payment is success. Call the administrator!");
                        paymentData.status = PaymentStatus.ErrorOnTryReceive;
                        db.Update(paymentData);
                        return;
                    }
                    await shopItem.GiveAndShowRewards(session, () => new ShopDialog(session).Start()).FastAwait();                    
                    session.profile.data.revenueRUB += (uint)paymentData.rubbles;
                    session.profile.dailyData.revenueRUB += (uint)paymentData.rubbles;
                    if (shopItem is ShopOfferItem)
                    {
                        session.profile.dynamicData.offers.Where(x => x.GetData().vendorCode == shopItem.vendorCode).FirstOrDefault()?.Deactivate();
                    }

                    Program.logger.Info($"PAYMENT | User {session.actualUser} received a reward" +
                        $"\n orderId: {paymentData.orderId}" +
                        $"\n telegramId: {paymentData.telegramId}" +
                        $"\n rubbles: {paymentData.rubbles}" +
                        $"\n comment: {paymentData.comment}" +
                        $"\n status: {PaymentStatus.Received}");
                    await session.profile.SaveProfile().FastAwait();
                    paymentData.status = PaymentStatus.Received;
                    db.Update(paymentData);
                    return;
                }

                paymentData.status = PaymentStatus.WaitingForGoods;
                db.Update(paymentData);
                session.profile.data.hasWaitingGoods = true;
                session.profile.data.revenueRUB += (uint)paymentData.rubbles;
                session.profile.dailyData.revenueRUB += (uint)paymentData.rubbles;
                await session.profile.SaveProfile().FastAwait();

                Program.logger.Info($"PAYMENT | User {session.actualUser} is waiting for a reward" +
                    $"\n orderId: {paymentData.orderId}" +
                    $"\n telegramId: {paymentData.telegramId}" +
                    $"\n rubbles: {paymentData.rubbles}" +
                    $"\n comment: {paymentData.comment}" +
                    $"\n status: {PaymentStatus.WaitingForGoods}");

                return;
            }
            catch (Exception ex)
            {
                await sessionExceptionHandler.HandleException(session.actualUser, ex).FastAwait();
            }
        }

        // session not exists
        var profileData = db.Table<ProfileData>().Where(x => x.telegram_id == telegramId).FirstOrDefault();
        if (profileData is null)
        {
            Program.logger.Error($"PAYMENT | Not found profileData with telegramId '{telegramId}'" +
                $"\n orderId: {paymentData.orderId}" +
                $"\n telegramId: {paymentData.telegramId}" +
                $"\n rubbles: {paymentData.rubbles}" +
                $"\n comment: {paymentData.comment}" +
                $"\n status: {PaymentStatus.ErrorOnTryReceive}" +
                $"But payment is success. Call the administrator!");

            paymentData.status = PaymentStatus.ErrorOnTryReceive;
            db.Update(paymentData);
            return;
        }
        
        paymentData.status = PaymentStatus.WaitingForGoods;
        db.Update(paymentData);
        profileData.hasWaitingGoods = true;
        profileData.revenueRUB += (uint)paymentData.rubbles;
        db.Update(profileData);

        Program.logger.Info($"PAYMENT | User (ID {profileData.telegram_id}) is waiting for a reward" +
            $"\n orderId: {paymentData.orderId}" +
            $"\n telegramId: {paymentData.telegramId}" +
            $"\n rubbles: {paymentData.rubbles}" +
            $"\n comment: {paymentData.comment}" +
            $"\n status: {PaymentStatus.WaitingForGoods}");

        try
        {
            var dailyData = db.Table<RawProfileDailyData>()
                .Where(x => x.telegram_id == telegramId)
                .FirstOrDefault();

            if (dailyData is not null)
            {
                dailyData.revenueRUB += (uint)paymentData.rubbles;
                db.Update(dailyData);
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public async Task GetNextWaitingGoods(GameSession session, Func<Task> onConinue)
    {
        Program.logger.Info($"PAYEMENT | Get waiting goods for user {session.actualUser}");
        var db = BotController.dataBase.db;
        var telegramId = session.actualUser.Id;
        var paymentData = db.Table<PaymentData>()
                .Where(x => x.telegramId == telegramId && x.status == PaymentStatus.WaitingForGoods)
                .FirstOrDefault();

        if (paymentData is null)
        {
            session.profile.data.hasWaitingGoods = false;
            await onConinue().FastAwait();
            return;
        }

        var vendorCode = paymentData.vendorCode;
        if (!gameDataHolder.shopItemsCache.TryGetValue(paymentData.vendorCode, out var shopItem))
        {
            Program.logger.Error($"PAYMENT | Not found item with vendorCode '{paymentData.vendorCode}'" +
                $"\n orderId: {paymentData.orderId}" +
                $"\n telegramId: {paymentData.telegramId}" +
                $"\n rubbles: {paymentData.rubbles}" +
                $"\n comment: {paymentData.comment}" +
                $"\n status: {PaymentStatus.ErrorOnTryReceive}" +
                $"But payment is success. Call the administrator!");

            paymentData.status = PaymentStatus.ErrorOnTryReceive;
            db.Update(paymentData);
            await onConinue().FastAwait();
            return;
        }

        await shopItem.GiveAndShowRewards(session, onConinue).FastAwait();
        if (shopItem is ShopOfferItem)
        {
            session.profile.dynamicData.offers.Where(x => x.GetData().vendorCode == shopItem.vendorCode).FirstOrDefault()?.Deactivate();
        }

        Program.logger.Info($"PAYMENT | User {session.actualUser} received a reward" +
            $"\n orderId: '{paymentData.orderId}'" +
            $"\n telegramId: {paymentData.telegramId}" +
            $"\n rubbles: {paymentData.rubbles}" +
            $"\n comment: {paymentData.comment}" +
            $"\n status: {PaymentStatus.Received}");
        await session.profile.SaveProfile().FastAwait();
        paymentData.status = PaymentStatus.Received;
        db.Update(paymentData);
    }

}
