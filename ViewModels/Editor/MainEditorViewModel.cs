using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using Avalonia;
using System.Collections.ObjectModel;
using TextGameRPG.Models.Editor;
using TextGameRPG.Views.Editor.ItemsEditor;
using TextGameRPG.Views.Editor.LocationsEditor;

namespace TextGameRPG.ViewModels.Editor
{
    internal class MainEditorViewModel : ViewModelBase
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
            categories.Add(new MainEditorCategory("Items", new ItemsEditorView() ));
            categories.Add(new MainEditorCategory("Locations", new LocationsEditorView()));
        }


    }
}
