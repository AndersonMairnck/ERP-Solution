using System.ComponentModel.DataAnnotations;

namespace ERPCore.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Usuário é obrigatório")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}