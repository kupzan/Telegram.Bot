using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace Telegram.Bot.Types.InlineQueryResults;

/// <summary>
/// Represents the content of a contact message to be sent as the result of an <see cref="InlineQuery">inline query</see>.
/// </summary>
[JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class InputContactMessageContent : InputMessageContent
{
    /// <summary>
    /// Contact's phone number
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string PhoneNumber { get; init; }

    /// <summary>
    /// Contact's first name
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string FirstName { get; init; }

    /// <summary>
    /// Optional. Contact's last name
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? LastName { get; set; }

    /// <summary>
    /// Optional. Additional data about the contact in the form of a vCard, 0-2048 bytes
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? Vcard { get; set; }

    /// <summary>
    /// Initializes a new input contact message content
    /// </summary>
    /// <param name="phoneNumber">The phone number of the contact</param>
    /// <param name="firstName">The first name of the contact</param>
    [SetsRequiredMembers]
    [Obsolete("Use parameterless constructor with required properties")]
    public InputContactMessageContent(string phoneNumber, string firstName)
    {
        PhoneNumber = phoneNumber;
        FirstName = firstName;
    }

    /// <summary>
    /// Initializes a new input contact message content
    /// </summary>
    public InputContactMessageContent()
    { }
}
