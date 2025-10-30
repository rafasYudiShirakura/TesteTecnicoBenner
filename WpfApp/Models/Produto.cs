using System.ComponentModel.DataAnnotations;

namespace WpfApp.Models
{
    public class Produto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        public string Nome { get; set; }
        
        [Required(ErrorMessage = "O Código é obrigatório.")]
        public string Codigo { get; set; }
        
        [Required(ErrorMessage = "O Valor é obrigatório.")]
        public decimal Valor { get; set; }
    }
}
