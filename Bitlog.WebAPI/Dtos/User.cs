using System.ComponentModel.DataAnnotations;

namespace Bitlog.WebApi.Dtos
{
    public record UserDto
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public record UserPayload : UserDto
    {
        [Required]
        [StringLength(maximumLength: int.MaxValue, MinimumLength = 8)]
        public string Password { get; set; }
    }

    record Result<T>(bool success, T data);

    record Error(string message);
}