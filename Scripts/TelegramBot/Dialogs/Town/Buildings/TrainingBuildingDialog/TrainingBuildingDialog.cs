using System.Text;
using System.Threading.Tasks;
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

        private async Task ShowCurrentUnit(sbyte unitIndex)
        {
            ClearButtons();
            var unitLevel = _building.GetUnitLevel(_data, unitIndex);
            var maxUnitLevel = _building.GetCurrentMaxUnitLevel(_data);
            var firstTrainingUnit = _building.GetFirstTrainingUnitIndex(_data);
            var secondTrainingUnit = _building.GetSecondTrainingUnitIndex(_data);
            bool hasFreeTrainingSlot = _building.HasFreeTrainingSlot(_data);
            bool currentUnitIsTraining = unitIndex == firstTrainingUnit || unitIndex == secondTrainingUnit;

            var sb = new StringBuilder();
            sb.Append($"{_building.GetUnitIcon(_data, unitIndex)} {_building.GetUnitName(session, _data, unitIndex)}");
            sb.AppendLine(string.Format(Localization.Get(session, "building_level_suffix"), unitLevel));

            if (unitLevel >= maxUnitLevel)
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "dialog_training_max_level_reached"));
            }
            else if (currentUnitIsTraining)
            {
                //TODO
            }
            else if (hasFreeTrainingSlot)
            {
                //TODO
            }
            else
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "dialog_training_no_slots_available"));
            }

            RegisterBackButton(() => ShowUnitsList());
            await messageSender.SendTextDialog(session.chatId, sb.ToString(), GetMultilineKeyboard());
        }

    }
}
