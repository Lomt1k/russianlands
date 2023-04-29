﻿using ReactiveUI;
using System.Collections.ObjectModel;
using TextGameRPG.Models.Editor;
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
        private MainEditorCategory _selectedCategory;

        public ObservableCollection<MainEditorCategory> categories { get; }
        public MainEditorCategory selectedCategory
        {
            get => _selectedCategory;
            set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
        }

        public MainEditorViewModel()
        {
            categories = new ObservableCollection<MainEditorCategory>();

            InitializeCategories();
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


    }
}
