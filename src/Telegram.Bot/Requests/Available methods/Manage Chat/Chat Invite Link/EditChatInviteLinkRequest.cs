using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Converters;
using Telegram.Bot.Requests.Abstractions;

// ReSharper disable once CheckNamespace
namespace Telegram.Bot.Requests;

/// <summary>
/// Use this method to edit a non-primary invite link created by the bot. The bot must be an administrator
/// in the chat for this to work and must have the appropriate admin rights. Returns the edited invite
/// link as a <see cref="Types.ChatInviteLink"/> object.
/// </summary>
[JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class EditChatInviteLinkRequest : RequestBase<ChatInviteLink>, IChatTargetable
{
    /// <inheritdoc />
    [JsonProperty(Required = Required.Always)]
    public required ChatId ChatId { get; init; }

    /// <summary>
    /// The invite link to edit
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string InviteLink { get; init; }

    /// <summary>
    /// Invite link name; 0-32 characters
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? Name { get; set; }

    /// <summary>
    /// Point in time when the link will expire
    /// </summary>
    [JsonConverter(typeof(UnixDateTimeConverter))]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? ExpireDate { get; set; }

    /// <summary>
    ///	Maximum number of users that can be members of the chat simultaneously after joining the
    /// chat via this invite link; 1-99999
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int? MemberLimit { get; set; }

    /// <summary>
    /// Set to <see langword="true"/>, if users joining the chat via the link need to be approved by chat administrators.
    /// If <see langword="true"/>, <see cref="MemberLimit"/> can't be specified
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool? CreatesJoinRequest { get; set; }

    /// <summary>
    /// Initializes a new request with chatId and inviteLink
    /// </summary>
    /// <param name="chatId">Unique identifier for the target chat or username of the target channel
    /// (in the format <c>@channelusername</c>)
    /// </param>
    /// <param name="inviteLink">The invite link to edit</param>
    [SetsRequiredMembers]
    [Obsolete("Use parameterless constructor with required properties")]
    public EditChatInviteLinkRequest(ChatId chatId, string inviteLink)
        : this()
    {
        ChatId = chatId;
        InviteLink = inviteLink;
    }

    /// <summary>
    /// Initializes a new request
    /// </summary>
    public EditChatInviteLinkRequest()
        : base("editChatInviteLink")
    { }
}
