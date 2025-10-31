using WpfApp.Models;
using WpfApp.Services;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using System.Windows.Input;
using System.Windows;
using System.Collections.Generic;

namespace WpfApp.ViewModels
{
    public class PessoaViewModel : CrudViewModelBase<Pessoa>
    {
        private string _filtroNome;
        private string _filtroCpf;

        private readonly PedidoService _pedidoService = new PedidoService();
        private ObservableCollection<Pedido> _pedidosDaPessoaSelecionada;
        private List<Pedido> _allPedidosDaPessoa;

        public ObservableCollection<Pedido> PedidosDaPessoaSelecionada
        {
            get => _pedidosDaPessoaSelecionada;
            set => SetProperty(ref _pedidosDaPessoaSelecionada, value);
        }

        public event Action<Pessoa> NavigateToPedidoRequested;

        public ICommand MarcarPagoCommand { get; }
        public ICommand MarcarEnviadoCommand { get; }
        public ICommand MarcarRecebidoCommand { get; }
        public ICommand FiltrarEntreguesCommand { get; }
        public ICommand FiltrarPagosCommand { get; }
        public ICommand FiltrarPendentesCommand { get; }
        public ICommand LimparFiltroCommand { get; }

        public string FiltroNome
        {
            get => _filtroNome;
            set { if (SetProperty(ref _filtroNome, value)) ApplyFilter(); }
        }

        public string FiltroCpf
        {
            get => _filtroCpf;
            set { if (SetProperty(ref _filtroCpf, value)) ApplyFilter(); }
        }

        public new Pessoa SelectedItem
        {
            get => base.SelectedItem;
            set
            {
                base.SelectedItem = value;

                if (base.SelectedItem != null)
                {
                    LoadPedidosDaPessoa(base.SelectedItem.Id);
                }
                else
                {
                    PedidosDaPessoaSelecionada?.Clear();
                    _allPedidosDaPessoa?.Clear();
                }
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public PessoaViewModel() : base(new PessoaService())
        {
            PedidosDaPessoaSelecionada = new ObservableCollection<Pedido>();
            _allPedidosDaPessoa = new List<Pedido>();

            Func<object, bool> canExecutePessoaSelecionada = (p) => SelectedItem != null;

            MarcarPagoCommand = new RelayCommand(p => MarcarStatus((Pedido)p, StatusPedido.Pago), p => CanMarcarStatus((Pedido)p, StatusPedido.Pago));
            MarcarEnviadoCommand = new RelayCommand(p => MarcarStatus((Pedido)p, StatusPedido.Enviado), p => CanMarcarStatus((Pedido)p, StatusPedido.Enviado));
            MarcarRecebidoCommand = new RelayCommand(p => MarcarStatus((Pedido)p, StatusPedido.Recebido), p => CanMarcarStatus((Pedido)p, StatusPedido.Recebido));

            FiltrarEntreguesCommand = new RelayCommand(_ => Filtrar(StatusPedido.Recebido), canExecutePessoaSelecionada);
            FiltrarPagosCommand = new RelayCommand(_ => Filtrar(StatusPedido.Pago), canExecutePessoaSelecionada);
            FiltrarPendentesCommand = new RelayCommand(_ => Filtrar(StatusPedido.Pendente), canExecutePessoaSelecionada);
            LimparFiltroCommand = new RelayCommand(_ => LoadPedidosDaPessoa(SelectedItem.Id, true), canExecutePessoaSelecionada);
        }

        protected override bool CanIncluirPedido(object obj) => SelectedItem != null;

        protected override Pessoa Clone(Pessoa item)
        {
            if (item == null) return new Pessoa(); 
            return new Pessoa
            {
                Id = item.Id,
                Nome = item.Nome,
                Cpf = item.Cpf,
                Endereco = item.Endereco
            };
        }

        protected override int GetId(Pessoa item) => item?.Id ?? 0; 
        protected override void SetId(Pessoa item, int id)
        {
            if (item != null) item.Id = id; 
        }

        protected override void OnSave(object obj)
        {
            if (CurrentItem == null)
            {
                MessageBox.Show("Não há dados para salvar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!ValidationService.IsValidCpf(CurrentItem.Cpf))
            {
                MessageBox.Show("O CPF fornecido é inválido ou está incompleto (deve ter 11 dígitos).", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentItem.Nome))
            {
                MessageBox.Show("O Nome é obrigatório.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var todasPessoas = _dataService.GetAll();
            bool cpfDuplicado = todasPessoas.Any(p => p.Cpf == CurrentItem.Cpf && p.Id != CurrentItem.Id);
            if (cpfDuplicado)
            {
                MessageBox.Show("Este CPF já está cadastrado no sistema.", "Erro de Duplicidade", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            base.OnSave(obj);
        }

        protected override void OnIncluirPedido(object obj)
        {
            if (SelectedItem != null)
            {
                NavigateToPedidoRequested?.Invoke(SelectedItem);
            }
        }

        private void ApplyFilter()
        {
            var allItems = _dataService.GetAll();
            var filteredItems = allItems.AsQueryable();

            if (!string.IsNullOrWhiteSpace(FiltroNome))
            {
                filteredItems = filteredItems.Where(p => p.Nome.Contains(FiltroNome, System.StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(FiltroCpf))
            {
                filteredItems = filteredItems.Where(p => p.Cpf.Contains(FiltroCpf));
            }

            Items = new ObservableCollection<Pessoa>(filteredItems.ToList());
        }

        private void LoadPedidosDaPessoa(int pessoaId, bool forceReload = true)
        {
            if (forceReload || _allPedidosDaPessoa == null)
            {
                _allPedidosDaPessoa = _pedidoService.GetAll()
                   .Where(p => p.Pessoa.Id == pessoaId)
                   .ToList();
            }
            foreach (var pedido in _allPedidosDaPessoa)
            {
                if (pedido.Pessoa == null) pedido.Pessoa = SelectedItem;
                foreach (var item in pedido.Produtos.Where(i => i.Produto == null))
                {
                    item.Produto = new Produto { Nome = "Produto Excluído" };
                }
            }
            PedidosDaPessoaSelecionada = new ObservableCollection<Pedido>(_allPedidosDaPessoa);
        }

        private void Filtrar(StatusPedido status)
        {
            if (_allPedidosDaPessoa == null) return;
            PedidosDaPessoaSelecionada = new ObservableCollection<Pedido>(_allPedidosDaPessoa.Where(p => p.Status == status).ToList());
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
                    AtualizarDadosPedidos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao atualizar status do pedido: {ex.Message}");
                }
            }
        }

        public void AtualizarDadosPedidos()
        {
            if (SelectedItem != null)
            {
                LoadPedidosDaPessoa(SelectedItem.Id, true); 
            }
        }
    }
}