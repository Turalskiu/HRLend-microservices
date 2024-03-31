using System.ComponentModel.DataAnnotations;

namespace AuthorizationApi.Models.DTO.Request
{
    public class RegistrationByTokenReguest
    {

        [Required]
        [StringLength(25, MinimumLength = 4, ErrorMessage = "Длина имени должна быть от 3 до 25 символов")]
        public string Username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Token { get; set; }

        public string? PageСonfirmationLink { get; set; } = "http://localhost:5034/user/authorization/";
    }
}
