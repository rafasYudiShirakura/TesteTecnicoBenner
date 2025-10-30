using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.ViewModels
{
    public class PessoaPedidosViewModel : ObservableObject
    {
        private readonly PedidoService _pedidoService = new PedidoService();
        private readonly Pessoa _pessoa;
        private ObservableCollection<Pedido> _pedidos;

        public Pessoa Pessoa => _pessoa;

        public ObservableCollection<Pedido> Pedidos
        {
            get => _pedidos;
            set => SetProperty(ref _pedidos, value);
        }

        public ICommand MarcarPagoCommand { get; }
        public ICommand MarcarEnviadoCommand { get; }
        public ICommand MarcarRecebidoCommand { get; }
        public ICommand FiltrarEntreguesCommand { get; }
        public ICommand FiltrarPagosCommand { get; }
        public ICommand FiltrarPendentesCommand { get; }
        public ICommand LimparFiltroCommand { get; }

        public PessoaPedidosViewModel(Pessoa pessoa)
        {
            _pessoa = pessoa;
            LoadPedidos();
            
            MarcarPagoCommand = new RelayCommand(p => MarcarStatus((Pedido)p, StatusPedido.Pago), p => CanMarcarStatus((Pedido)p, StatusPedido.Pago));
            MarcarEnviadoCommand = new RelayCommand(p => MarcarStatus((Pedido)p, StatusPedido.Enviado), p => CanMarcarStatus((Pedido)p, StatusPedido.Enviado));
            MarcarRecebidoCommand = new RelayCommand(p => MarcarStatus((Pedido)p, StatusPedido.Recebido), p => CanMarcarStatus((Pedido)p, StatusPedido.Recebido));
            
            FiltrarEntreguesCommand = new RelayCommand(_ => Filtrar(StatusPedido.Recebido));
            FiltrarPagosCommand = new RelayCommand(_ => Filtrar(StatusPedido.Pago));
            FiltrarPendentesCommand = new RelayCommand(_ => Filtrar(StatusPedido.Pendente));
            LimparFiltroCommand = new RelayCommand(_ => LoadPedidos(true));
        }

        private void LoadPedidos(bool forceReload = false)
        {
            var allPedidos = _pedidoService.GetAll()
                .Where(p => p.Pessoa.Id == _pessoa.Id)
                .ToList();
            
            Pedidos = new ObservableCollection<Pedido>(allPedidos);
        }

        private void Filtrar(StatusPedido status)
        {
            LoadPedidos(true);
            Pedidos = new ObservableCollection<Pedido>(Pedidos.Where(p => p.Status == status).ToList());
        }

        private bool CanMarcarStatus(Pedido pedido, StatusPedido novoStatus)
        {
            return pedido != null && pedido.Status != novoStatus;
        }

        private void MarcarStatus(Pedido pedido, StatusPedido novoStatus)
        {
            if (pedido != null)
            {
                pedido.Status = novoStatus;
                try
                {
                    _pedidoService.Update(pedido);
                    LoadPedidos(true);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"Erro ao atualizar status do pedido: {ex.Message}");
                }
            }
        }
    }
}
