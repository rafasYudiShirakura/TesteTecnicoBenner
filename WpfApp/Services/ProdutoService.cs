using WpfApp.Models;

namespace WpfApp.Services
{
    public class ProdutoService : JsonDataService<Produto>
    {
        public ProdutoService() : base("produtos.json") { }
    }
}
