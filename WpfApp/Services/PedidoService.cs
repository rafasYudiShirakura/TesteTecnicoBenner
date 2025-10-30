using WpfApp.Models;

namespace WpfApp.Services
{
    public class PedidoService : JsonDataService<Pedido>
    {
        public PedidoService() : base("pedidos.json") { }
    }
}
