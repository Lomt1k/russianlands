using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Buildings.TrainingBuildingDialog
{
    public class TrainingBuildingDialog : DialogBase
    {
        private TrainingBuildingBase _building;
        private ProfileBuildingsData _data;

        public TrainingBuildingDialog(GameSession session, TrainingBuildingBase building, ProfileBuildingsData data) : base(session)
        {
            _building = building;
            _data = data;
        }

        public override async Task Start()
        {
            await ShowUnitsList();
        }

        private async Task ShowUnitsList()
        {
            var text = Localization.Get(session, "dialog_training_selection_" + _building.buildingType);

            var units = _building.GetAllUnits(_data);
            foreach (var unit in units)
            {
                var button = $"{_building.GetUnitIcon(_data, unit)} {_building.GetUnitName(session, _data, unit)}";
                RegisterButton(button, () => ShowCurrentUnit(unit));
            }

            RegisterBackButton(() => new BuildingsDialog(session).Start());
            await messageSender.SendTextDialog(session.chatId, text, GetKeyboardWithFixedRowSize(2));
        }

        private async Task ShowCurrentUnit(int unitIndex)
        {
            //TODO
        }

    }
}
