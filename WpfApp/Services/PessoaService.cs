using WpfApp.Models;

namespace WpfApp.Services
{
    public class PessoaService : JsonDataService<Pessoa>
    {
        public PessoaService() : base("pessoas.json") { }
    }
}
