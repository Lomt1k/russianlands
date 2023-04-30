using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using TextGameRPG.Models.Editor;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Services.GameData;
using TextGameRPG.Views.Editor.BuildingsEditor;
using TextGameRPG.Views.Editor.ItemsEditor;
using TextGameRPG.Views.Editor.LocationMobsEditor;
using TextGameRPG.Views.Editor.MobsEditor;
using TextGameRPG.Views.Editor.PotionsEditor;
using TextGameRPG.Views.Editor.QuestsEditor;

namespace TextGameRPG.ViewModels.Editor
{
    public class MainEditorViewModel : ViewModelBase
    {
        private readonly GameDataHolder gameDataHolder = Services.Get<GameDataHolder>();

        private MainEditorCategory _selectedCategory;

        public ObservableCollection<MainEditorCategory> categories { get; }
        public MainEditorCategory selectedCategory
        {
            get => _selectedCategory;
            set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
        }

        public ReactiveCommand<Unit, Unit> saveCommand { get; }
        public ReactiveCommand<Unit, Unit> reloadCommand { get; }

        public MainEditorViewModel()
        {
            categories = new ObservableCollection<MainEditorCategory>();
            InitializeCategories();

            saveCommand = ReactiveCommand.Create(SaveCommand);
            reloadCommand = ReactiveCommand.Create(ReloadCommand);
        }

        private void InitializeCategories()
        {
            categories.Add(new MainEditorCategory("Buildings", new BuildingsEditorView()));
            categories.Add(new MainEditorCategory("Items", new ItemsEditorView() ));
            categories.Add(new MainEditorCategory("Quests", new QuestsEditorView()));
            categories.Add(new MainEditorCategory("Mobs", new MobsEditorView()));
            categories.Add(new MainEditorCategory("LocationMobs", new LocationMobsEditorView()));
            categories.Add(new MainEditorCategory("Potions", new PotionsEditorView()));
        }

        private void SaveCommand()
        {
            // TODO
        }

        private void ReloadCommand()
        {
            gameDataHolder.LoadAllData();
        }


    }
}
