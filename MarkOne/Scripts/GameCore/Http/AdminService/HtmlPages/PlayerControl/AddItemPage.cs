using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData.DataTypes;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Resources;
using Obisoft.HSharp.Models;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Items.Generators;
using System.Linq;
using FastTelegramBot.DataTypes;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages.PlayerControl;
internal class AddItemPage : IHtmlPage
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();

    public string page => "addItem";

    public async Task ShowPage(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        var showActivePlayers = query["showActivePlayers"];
        var fromActivePlayers = showActivePlayers is not null;
        if (!long.TryParse(query["telegramId"], out var parsedTelegramId))
        {
            var error = HtmlHelper.CreateMessagePage("Incorrent Telegram ID", $"Incorrent 'telegramId'", localPath);
            response.AsTextUTF8(error.GenerateHTML());
            response.Close();
            return;
        }

        if (!sessionInfo.withoutLogin)
        {
            var db = BotController.dataBase.db;
            var profileData = db.Table<ProfileData>().Where(x => x.telegram_id == sessionInfo.telegramId).FirstOrDefault();
            if (profileData is null || profileData.adminStatus < AdminStatus.Admin)
            {
                var error = HtmlHelper.CreateMessagePage("Forbidden", $"You dont have admin rights", GetBackUrl(query, localPath));
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }
        }

        var itemCode = query["itemCode"];
        if (itemCode is not null)
        {
            // add item
            try
            {
                var item = new InventoryItem(itemCode);
                if (item is null || string.IsNullOrEmpty(item.GetFullName(sessionInfo.languageCode)))
                {
                    var error = HtmlHelper.CreateMessagePage("Bad request", $"Incorrent 'itemCode'", localPath);
                    response.AsTextUTF8(error.GenerateHTML());
                    response.Close();
                    return;
                }
                await AddItem(response, sessionInfo, query, localPath, parsedTelegramId, item).FastAwait();
                return;
            }
            catch (Exception ex)
            {
                var error = HtmlHelper.CreateMessagePage("Bad request", $"Catched exception on try create item with code '{itemCode}'", localPath);
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }
        }

        // show add item dialog
        var document = HtmlHelper.CreateDocument("Add Item");
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());

        var centerScreenBlock = new HTag("div", StylesHelper.CenterScreenBlock(1200, 900));
        document["html"]["body"].Add(centerScreenBlock);

        centerScreenBlock.Add(GetItemCodesInfo());

        var form = new HtmlFormBuilder(localPath)
            .AddHeader($"Add Item (ID {parsedTelegramId})")
            .AddHiddenInput("page", page)
            .AddHiddenInput("telegramId", parsedTelegramId.ToString())
            .AddHiddenInputIfNotNull("showActivePlayers", showActivePlayers)
            .AddInput("itemCode", "item code / id")
            .AddButton("Add")
            .GetResult();

        centerScreenBlock.Add("div").Add(form);
        centerScreenBlock.Add("br");
        centerScreenBlock.Add("br");
        centerScreenBlock.Add(HtmlHelper.CreateLinkButton("<< Back", GetBackUrl(query, localPath)));

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private async Task AddItem(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, long telegramId, InventoryItem item)
    {
        var session = sessionManager.GetSessionIfExists(telegramId);
        if (session is not null)
        {
            session.player.inventory.ForceAddItem(item);
            var notification = Localization.Get(session, "notification_admin_add_resource", sessionInfo.GetAdminView(), item.GetFullName(session));
            session.profile.data.AddSpecialNotification(notification);
            await session.profile.SaveProfile().FastAwait();
            ShowSuccessfullAddItem(response, sessionInfo, query, localPath, session.profile.data, item);
        }
        else
        {
            var db = BotController.dataBase.db;
            var profileData = db.Table<ProfileData>().Where(x => x.telegram_id == telegramId).FirstOrDefault();
            if (profileData == null)
            {
                var error = HtmlHelper.CreateMessagePage("Bad request", $"ProfileData with 'telegram_id' == {telegramId} not exists", localPath);
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }
            var rawDynamicData = db.Table<RawProfileDynamicData>().Where(x => x.dbid == profileData.dbid).FirstOrDefault();
            if (rawDynamicData == null)
            {
                var error = HtmlHelper.CreateMessagePage("Bad request", $"RawProfileDynamicData for 'telegram_id' == {telegramId} not exists", localPath);
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }

            var profileDynamicData = ProfileDynamicData.Deserialize(rawDynamicData);
            profileDynamicData.inventory.ForceAddItem(item);
            rawDynamicData.Fill(profileDynamicData);            

            var notification = Localization.Get(profileData.language, "notification_admin_add_resource", sessionInfo.GetAdminView(), item.GetFullName(profileData.language));
            profileData.AddSpecialNotification(notification);
            db.Update(profileData);
            db.Update(rawDynamicData);
            ShowSuccessfullAddItem(response, sessionInfo, query, localPath, profileData, item);
        }
    }

    private void ShowSuccessfullAddItem(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, ProfileData profileData, InventoryItem item)
    {
        var playerUser = new User
        {
            Id = profileData.telegram_id,
            FirstName = profileData.firstName,
            LastName = profileData.lastName,
            Username = profileData.username,
        };
        Program.logger.Info($"Administrator {sessionInfo.user} gave the player {playerUser} {item.GetFullName(sessionInfo.languageCode)}");

        var success = HtmlHelper.CreateMessagePage("Success", $"Successfully adds\n{item.GetFullName(sessionInfo.languageCode)}\nfor {playerUser}", GetBackUrl(query, localPath));
        response.AsTextUTF8(success.GenerateHTML());
        response.Close();
    }

    private string GetBackUrl(NameValueCollection query, string localPath)
    {
        return localPath + $"?page=playerSearch&telegramId={query["telegramId"]}"
            + (query["showActivePlayers"] is not null ? "&showActivePlayers=" : string.Empty);
    }

    private HTag GetItemCodesInfo()
    {
        var itemTypesInfo = new HTag("div", new HProp("style", "float: left;"))
        {
            { "h3", "Items" },
            new HTag("table", new HProp("frame", "hsides"))
            {
                CreateTableRow(ItemType.Sword.GetEmoji() + "SW"),
                CreateTableRow(ItemType.Bow.GetEmoji() + "BO"),
                CreateTableRow(ItemType.Stick.GetEmoji() + "ST"),
                CreateTableRow(ItemType.Scroll.GetEmoji() + "SC"),
                CreateTableRow(ItemType.Armor.GetEmoji() + "AR"),
                CreateTableRow(ItemType.Helmet.GetEmoji() + "HE"),
                CreateTableRow(ItemType.Boots.GetEmoji() + "BT"),
                CreateTableRow(ItemType.Shield.GetEmoji() + "SH"),
                CreateTableRow(ItemType.Amulet.GetEmoji() + "AM"),
                CreateTableRow(ItemType.Ring.GetEmoji() + "RI"),
            }
        };

        var townhallsInfo = new HTag("div", new HProp("style", "float: left; margin-left: 30px;"))
        {
            { "h3", "Townhalls" },
            new HTag("table", new HProp("frame", "hsides")),
        };
        foreach (var townhall in ItemGenerationHelper.itemGradesByTownHall.Keys)
        {
            townhallsInfo["table"].Add(CreateTableRow(townhall.ToString()));
        }

        var gradesInfo = new HTag("div", new HProp("style", "float: left; margin-left: 30px;"))
        {
            { "h3", "Grades" },
            new HTag("table", new HProp("frame", "hsides")),
        };
        foreach (var gradesArray in ItemGenerationHelper.itemGradesByTownHall.Values)
        {
            var grades = string.Empty;
            foreach (var grade in gradesArray.ToHashSet())
            {
                grades += $"G{grade} ";
            }
            gradesInfo["table"].Add(CreateTableRow(grades));
        }

        var raritiesInfo = new HTag("div", new HProp("style", "float: left; margin-left: 30px;"))
        {
            { "h3", "Rarities" },
            new HTag("table", new HProp("frame", "hsides")),
            { "h3", "Mana Cost (scrolls)" },
        };
        foreach (var rarity in Enum.GetValues<Rarity>())
        {
            raritiesInfo["table"].Add(CreateTableRow($"R{(int)rarity} - {rarity}"));
        }
        var manaCostsTable = new HTag("table", new HProp("frame", "hsides"))
        {
            CreateTableRow("M2"),
            CreateTableRow("M3"),
            CreateTableRow("M4"),
            CreateTableRow("M5"),
        };
        raritiesInfo.Add(manaCostsTable);

        var propertiesInfo = new HTag("div", new HProp("style", "float: left; margin-left: 30px;"))
        {
            { "h3", "Properties & Abilities" },
            new HTag("table", new HProp("frame", "hsides"))
            {
                CreateTableRow("DF - fire damage / resistance"),
                CreateTableRow("DC - cold damage / resistance"),
                CreateTableRow("DL - lightning damage / resistance"),
                CreateTableRow("P2 - increase max health (ring / amulet)"),
                CreateTableRow("A3 - restore health every turn (ring / amulet)"),
                CreateTableRow("A4 - add mana every turn (ring / amulet)"),
                CreateTableRow("A5 - sword block keyword (sword)"),
                CreateTableRow("A6 - bow last shot keyword (bow)"),
                CreateTableRow("A7 - add arrow keyword (sword)"),
                CreateTableRow("A8 - steal mana keyword (sword / bow / stick)"),
                CreateTableRow("A9 - fire damage keyword (sword / bow / stick)"),
                CreateTableRow("A10 - cold damage keyword (sword / bow / stick)"),
                CreateTableRow("A11 - lightning damage keyword (sword / bow / stick)"),
                CreateTableRow("A12 - rage keyword (sword / stick)"),
                CreateTableRow("A13 - finishing keyword (sword / bow / stick)"),
                CreateTableRow("A14 - absorption keyword (sword / bow / stick)"),
                CreateTableRow("A15 - add mana keyword (stick)"),
                CreateTableRow("A16 - stun keyword (sword / bow / stick)"),
                CreateTableRow("A17 - sanctions keyword (sword / bow / stick)"),
            }
        };

        var examplesInfo = new HTag("div", new HProp("style", "clear: both; margin-top:60px;"))
        { 
            { "p", "<b>Examples:</b> SW3G5R3DFA7A16, RI8G10R2P2A4, SC7G4R3M5DL, AR8G2R3DFDCDL" }
        };

        var itemCodesInfo = new HTag("div", new HProp("align", "left"))
        {
            itemTypesInfo,
            townhallsInfo,
            gradesInfo,
            raritiesInfo,
            propertiesInfo,
            examplesInfo,
        };
        return itemCodesInfo;
    }

    private HTag CreateTableRow(string text)
    {
        var row = new HTag("tr")
        {
            { "td", text, new HProp("align", "left") }
        };
        return row;
    }

}
