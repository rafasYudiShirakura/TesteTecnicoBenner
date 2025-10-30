using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WpfApp.Services;

namespace WpfApp.ViewModels
{
    public abstract class CrudViewModelBase<T> : ObservableObject where T : class, new()
    {
        protected readonly IDataService<T> _dataService;
        private ObservableCollection<T> _items;
        private T _selectedItem;
        private T _currentItem;

        public ObservableCollection<T> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public T SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    if (value != null)
                    {
                        CurrentItem = Clone(value);
                    }
                    else
                    {
                        CurrentItem = new T();
                    }
                }
            }
        }

        public T CurrentItem
        {
            get => _currentItem;
            set => SetProperty(ref _currentItem, value);
        }

        public ICommand NewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand IncluirPedidoCommand { get; }

        public CrudViewModelBase(IDataService<T> dataService)
        {
            _dataService = dataService;
            LoadItems();
            NewCommand = new RelayCommand(OnNew);
            SaveCommand = new RelayCommand(OnSave);
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            IncluirPedidoCommand = new RelayCommand(OnIncluirPedido, CanIncluirPedido);
            CurrentItem = new T();
        }

        protected abstract T Clone(T item);
        protected abstract int GetId(T item);
        protected abstract void SetId(T item, int id);

        protected void LoadItems()
        {
            Items = new ObservableCollection<T>(_dataService.GetAll());
        }

        protected void OnNew(object obj)
        {
            SelectedItem = null;
            CurrentItem = new T();
        }

        protected virtual void OnSave(object obj)
        {
            try
            {
                if (GetId(CurrentItem) == 0)
                {
                    _dataService.Add(CurrentItem);
                }
                else
                {
                    _dataService.Update(CurrentItem);
                }
                LoadItems();
                OnNew(null);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        private bool CanDelete(object obj) => SelectedItem != null && GetId(SelectedItem) != 0;

        protected virtual bool CanIncluirPedido(object obj) => SelectedItem != null && GetId(SelectedItem) != 0;

        protected virtual void OnIncluirPedido(object obj)
        {
        }

        private void OnDelete(object obj)
        {
            try
            {
                _dataService.Delete(GetId(SelectedItem));
                LoadItems();
                OnNew(null); 
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
    }
}
