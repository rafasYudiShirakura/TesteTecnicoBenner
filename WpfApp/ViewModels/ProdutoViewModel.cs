using WpfApp.Models;
using WpfApp.Services;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows; 

namespace WpfApp.ViewModels
{
    public class ProdutoViewModel : CrudViewModelBase<Produto>
    {
        private string _filtroNome;
        private string _filtroCodigo;
        private decimal? _filtroValorInicial;
        private decimal? _filtroValorFinal;

        public string FiltroNome
        {
            get => _filtroNome;
            set { if (SetProperty(ref _filtroNome, value)) ApplyFilter(); }
        }

        public string FiltroCodigo
        {
            get => _filtroCodigo;
            set { if (SetProperty(ref _filtroCodigo, value)) ApplyFilter(); }
        }

        public decimal? FiltroValorInicial
        {
            get => _filtroValorInicial;
            set { if (SetProperty(ref _filtroValorInicial, value)) ApplyFilter(); }
        }

        public decimal? FiltroValorFinal
        {
            get => _filtroValorFinal;
            set { if (SetProperty(ref _filtroValorFinal, value)) ApplyFilter(); }
        }

        public ProdutoViewModel() : base(new ProdutoService()) { }

        protected override Produto Clone(Produto item)
        {
            return new Produto
            {
                Id = item.Id,
                Nome = item.Nome,
                Codigo = item.Codigo,
                Valor = item.Valor
            };
        }

        protected override int GetId(Produto item) => item.Id;
        protected override void SetId(Produto item, int id) => item.Id = id;

        protected override void OnSave(object obj)
        {
            if (CurrentItem == null) return;

            if (string.IsNullOrWhiteSpace(CurrentItem.Nome) ||
                string.IsNullOrWhiteSpace(CurrentItem.Codigo))
            {
                MessageBox.Show("Nome e Código são campos obrigatórios.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CurrentItem.Valor <= 0)
            {
                MessageBox.Show("O Valor do produto deve ser maior que zero.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            base.OnSave(obj);
        }

        private void ApplyFilter()
        {
            var allItems = _dataService.GetAll();
            var filteredItems = allItems.AsQueryable();

            if (!string.IsNullOrWhiteSpace(FiltroNome))
            {
                filteredItems = filteredItems.Where(p => p.Nome.Contains(FiltroNome, System.StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(FiltroCodigo))
            {
                filteredItems = filteredItems.Where(p => p.Codigo.Contains(FiltroCodigo, System.StringComparison.OrdinalIgnoreCase));
            }

            if (FiltroValorInicial.HasValue)
            {
                filteredItems = filteredItems.Where(p => p.Valor >= FiltroValorInicial.Value);
            }

            if (FiltroValorFinal.HasValue)
            {
                filteredItems = filteredItems.Where(p => p.Valor <= FiltroValorFinal.Value);
            }

            Items = new ObservableCollection<Produto>(filteredItems.ToList());
        }
    }
}