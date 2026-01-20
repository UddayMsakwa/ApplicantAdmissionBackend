using System.Text.Json.Serialization;


namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Auth;

public class AuthResponseDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;
}
