using Avalonia.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reactive;
using ReactiveUI;
using System.Threading.Tasks;

namespace TextGameRPG.ViewModels.UserControls
{
    internal abstract class EditorListViewModel<T> : ViewModelBase
    {
        private List<T> _itemsList = new();
        private UserControl? _selectedView;

        public ObservableCollection<UserControl> viewsCollection { get; private set; } = new();
        public UserControl? selectedView
        {
            get => _selectedView;
            set => this.RaiseAndSetIfChanged(ref _selectedView, value);
        }

        public ReactiveCommand<Unit, Unit> addItemCommand { get; }
        public ReactiveCommand<Unit, Unit> removeItemCommand { get; }

        public EditorListViewModel()
        {
            addItemCommand = ReactiveCommand.Create(AddNewValue);
            removeItemCommand = ReactiveCommand.Create(RemoveValue);
        }

        public void SetModel(List<T> itemsList)
        {
            _itemsList = itemsList;
            RefreshViewsCollection();
        }

        private async void AddNewValue()
        {
            var value = await CreateNewListItem();
            if (value is null)
            {
                return;
            }
            _itemsList.Add(value);
            RefreshViewsCollection();
        }

        private void RemoveValue()
        {
            if (selectedView is null)
            {
                return;
            }
            var index = viewsCollection.IndexOf(selectedView);
            _itemsList.RemoveAt(index);
            RefreshViewsCollection();
        }

        private void RefreshViewsCollection()
        {
            viewsCollection.Clear();
            foreach (var item in _itemsList)
            {
                var view = CreateViewForItem(item);
                viewsCollection.Add(view);
            }
        }

        protected abstract Task<T?> CreateNewListItem();
        protected abstract UserControl CreateViewForItem(T item);        
    }
}
