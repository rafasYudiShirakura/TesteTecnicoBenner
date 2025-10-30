using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WpfApp.Models
{
    public enum FormaPagamento
    {
        Dinheiro,
        Cartao,
        Boleto
    }

    public enum StatusPedido
    {
        Pendente,
        Pago,
        Enviado,
        Recebido
    }

    public class Pedido
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "A Pessoa é obrigatória.")]
        public Pessoa Pessoa { get; set; }
        
        [Required(ErrorMessage = "A lista de Produtos é obrigatória.")]
        public List<PedidoItem> Produtos { get; set; } = new List<PedidoItem>();
        
        public decimal ValorTotal => Produtos.Sum(item => item.Subtotal);

        public string ProdutosResumo => Produtos != null
            ? string.Join(", ", Produtos.Select(item => $"{item.Produto.Nome} (x{item.Quantidade})"))
            : string.Empty;

        public DateTime DataVenda { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "A Forma de Pagamento é obrigatória.")]
        public FormaPagamento FormaPagamento { get; set; }
        
        public StatusPedido Status { get; set; } = StatusPedido.Pendente;

        public bool PodeSerEditado { get; set; } = true;
    }
}
