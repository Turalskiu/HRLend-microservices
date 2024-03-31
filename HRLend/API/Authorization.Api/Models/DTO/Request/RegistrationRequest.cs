using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthorizationApi.Models.DTO.Request
{
    public class RegistrationRequest
    {
        [Required]
        [StringLength(25, MinimumLength = 4, ErrorMessage = "Длина имени должна быть от 3 до 25 символов")]
        public string Username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string CabinetTitle { get; set; }

        public string? PageСonfirmationLink { get; set; } = "http://localhost:5034/user/authorization/";
    }
}
