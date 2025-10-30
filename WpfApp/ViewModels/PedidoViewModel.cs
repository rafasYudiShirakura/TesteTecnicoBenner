using WpfApp.Models;
using WpfApp.Services;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;


namespace WpfApp.ViewModels
{
    public class PedidoViewModel : CrudViewModelBase<Pedido>
    {
        private readonly PessoaService _pessoaService = new PessoaService();
        private readonly ProdutoService _produtoService = new ProdutoService();
        private readonly PedidoService _pedidoService = new PedidoService();

        public ObservableCollection<Pessoa> Pessoas { get; set; }
        public ObservableCollection<Produto> ProdutosDisponiveis { get; set; }
        public ObservableCollection<FormaPagamento> FormasPagamento { get; set; }
        public ObservableCollection<StatusPedido> StatusOpcoes { get; set; }

        private PedidoItem _novoItem;
        public PedidoItem NovoItem
        {
            get => _novoItem;
            set => SetProperty(ref _novoItem, value);
        }

        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand FinalizarPedidoCommand { get; }

        public PedidoViewModel() : base(new PedidoService())
        {
            Pessoas = new ObservableCollection<Pessoa>(_pessoaService.GetAll());
            ProdutosDisponiveis = new ObservableCollection<Produto>(_produtoService.GetAll());
            FormasPagamento = new ObservableCollection<FormaPagamento>(Enum.GetValues(typeof(FormaPagamento)).Cast<FormaPagamento>());
            StatusOpcoes = new ObservableCollection<StatusPedido>(Enum.GetValues(typeof(StatusPedido)).Cast<StatusPedido>());
            
            NovoItem = new PedidoItem { Produto = ProdutosDisponiveis.FirstOrDefault(), Quantidade = 1 };

            AddItemCommand = new RelayCommand(OnAddItem, CanAddItem);
            RemoveItemCommand = new RelayCommand(OnRemoveItem, CanRemoveItem);
            FinalizarPedidoCommand = new RelayCommand(OnFinalizarPedido, CanFinalizarPedido);
        }

        public PedidoViewModel(Pessoa pessoa) : this() 
        {

            OnNew(null);

            var pessoaCompleta = Pessoas.FirstOrDefault(p => p.Id == pessoa.Id);

            CurrentItem.Pessoa = pessoaCompleta;
        }

        protected override Pedido Clone(Pedido item)
        {
            return new Pedido
            {
                Id = item.Id,
                Pessoa = item.Pessoa,
                Produtos = item.Produtos.Select(pi => new PedidoItem { Produto = pi.Produto, Quantidade = pi.Quantidade }).ToList(),
                DataVenda = item.DataVenda,
                FormaPagamento = item.FormaPagamento,
                Status = item.Status,
                PodeSerEditado = item.PodeSerEditado
            };
        }

        protected override int GetId(Pedido item) => item.Id;
        protected override void SetId(Pedido item, int id) => item.Id = id;

        private bool CanAddItem(object obj) => NovoItem?.Produto != null && NovoItem.Quantidade > 0 && CurrentItem.PodeSerEditado;
        private void OnAddItem(object obj)
        {
            if (CurrentItem.Produtos.Any(p => p.Produto.Id == NovoItem.Produto.Id))
            {
                var existing = CurrentItem.Produtos.First(p => p.Produto.Id == NovoItem.Produto.Id);
                existing.Quantidade += NovoItem.Quantidade;
            }
            else
            {
                CurrentItem.Produtos.Add(new PedidoItem { Produto = NovoItem.Produto, Quantidade = NovoItem.Quantidade });
            }
            OnPropertyChanged(nameof(CurrentItem));
            NovoItem = new PedidoItem { Produto = ProdutosDisponiveis.FirstOrDefault(), Quantidade = 1 };
        }

        private bool CanRemoveItem(object obj) => obj is PedidoItem && CurrentItem.PodeSerEditado;
        private void OnRemoveItem(object obj)
        {
            if (obj is PedidoItem item)
            {
                CurrentItem.Produtos.Remove(item);
                OnPropertyChanged(nameof(CurrentItem));
            }
        }

        private bool CanFinalizarPedido(object obj) => CurrentItem.Pessoa != null && CurrentItem.Produtos.Any() && CurrentItem.PodeSerEditado;
        private void OnFinalizarPedido(object obj)
        {
            if (CanFinalizarPedido(obj))
            {
                CurrentItem.PodeSerEditado = false;

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
                    SelectedItem = null; 
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Erro ao finalizar pedido: {ex.Message}");
                }
            }
        }

        protected new void LoadItems()
        {
            var pessoas = _pessoaService.GetAll().ToDictionary(p => p.Id);
            var produtos = _produtoService.GetAll().ToDictionary(p => p.Id);

            var pedidos = _pedidoService.GetAll().Select(p =>
            {
                if (pessoas.TryGetValue(p.Pessoa.Id, out var pessoa))
                {
                    p.Pessoa = pessoa;
                }

                foreach (var item in p.Produtos)
                {
                    if (produtos.TryGetValue(item.Produto.Id, out var produto))
                    {
                        item.Produto = produto;
                    }
                }
                return p;
            });

            Items = new ObservableCollection<Pedido>(pedidos);
        }
    }
}
