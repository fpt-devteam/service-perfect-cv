using ServicePerfectCV.Domain.Constants;

namespace ServicePerfectCV.Application.DTOs.User.Responses;

public class UserResponse
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }       
    public required string Role { get; init; }
}