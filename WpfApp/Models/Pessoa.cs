using System.ComponentModel.DataAnnotations;

namespace WpfApp.Models
{
    public class Pessoa
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        public string Nome { get; set; }
        
        [Required(ErrorMessage = "O CPF é obrigatório.")]
        public string Cpf { get; set; }
        
        public string Endereco { get; set; }
    }
}
