namespace Shared.Contracts.Auth;

/// <summary>
/// Represents a JWT claim exposed by the profile API.
/// </summary>
/// <param name="Type">The claim type.</param>
/// <param name="Value">The claim value.</param>
public sealed record ClaimResponse(string Type, string Value);
