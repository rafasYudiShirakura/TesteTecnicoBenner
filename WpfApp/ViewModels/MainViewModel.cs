using WpfApp.Models;
using System.Linq;
using System.Windows.Input;

namespace WpfApp.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private object _currentViewModel;

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public ICommand NavigateCommand { get; }

        public MainViewModel()
        {
            NavigateCommand = new RelayCommand(Navigate);
            Navigate("Pessoa"); 
        }

        private void Navigate(object parameter)
        {
            if (parameter is string viewName)
            {
                if (CurrentViewModel is PessoaViewModel oldPessoaVM)
                {
                    oldPessoaVM.NavigateToPedidoRequested -= NavigateToPedido;
                }

                switch (viewName)
                {
                    case "Pessoa":
                        var pessoaVM = new PessoaViewModel();
                        pessoaVM.NavigateToPedidoRequested += NavigateToPedido;
                        CurrentViewModel = pessoaVM;
                        break;
                    case "Produto":
                        CurrentViewModel = new ProdutoViewModel();
                        break;
                    case "Pedido":
                        CurrentViewModel = new PedidoViewModel();
                        break;
                }
            }
        }

        private void NavigateToPedido(Pessoa pessoa)
        {
            if (CurrentViewModel is PessoaViewModel oldPessoaVM)
            {
                oldPessoaVM.NavigateToPedidoRequested -= NavigateToPedido;
            }

            CurrentViewModel = new PedidoViewModel(pessoa);
        }
    }
}