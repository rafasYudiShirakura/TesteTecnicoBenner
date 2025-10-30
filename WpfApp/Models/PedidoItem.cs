namespace WpfApp.Models
{
    public class PedidoItem
    {
        public Produto Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal Subtotal => Produto.Valor * Quantidade;
    }
}
