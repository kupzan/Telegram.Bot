using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Telegram.Bot.Requests.Abstractions;

// ReSharper disable once CheckNamespace
namespace Telegram.Bot.Requests;

/// <summary>
/// Use this method to add a new sticker to a set created by the bot.
/// The format of the added sticker must match the format of the other stickers in the set.
/// <list type="bullet">
/// <item>
/// Emoji sticker sets can have up to 200 stickers.
/// </item>
/// <item>
/// Animated and video sticker sets can have up to 50 stickers.
/// </item>
/// <item>
/// Static sticker sets can have up to 120 stickers.
/// </item>
/// </list>
/// Returns <see langword="true"/> on success.
/// </summary>
[JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class AddStickerToSetRequest : FileRequestBase<bool>, IUserTargetable
{
    /// <inheritdoc />
    [JsonProperty(Required = Required.Always)]
    public required long UserId { get; init; }

    /// <summary>
    /// Sticker set name
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string Name { get; init; }

    /// <summary>
    /// A JSON-serialized object with information about the added sticker.
    /// If exactly the same sticker had already been added to the set, then the set isn't changed.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required InputSticker Sticker { get; init; }

    /// <summary>
    /// Initializes a new request with userId, name and sticker
    /// </summary>
    /// <param name="userId">
    /// User identifier
    /// </param>
    /// <param name="name">
    /// Sticker set name
    /// </param>
    /// <param name="sticker">
    /// A JSON-serialized object with information about the added sticker.
    /// If exactly the same sticker had already been added to the set, then the set isn't changed.
    /// </param>
    [SetsRequiredMembers]
    [Obsolete("Use parameterless constructor with required properties")]
    public AddStickerToSetRequest(
        long userId,
        string name,
        InputSticker sticker)
        : this()
    {
        UserId = userId;
        Name = name;
        Sticker = sticker;
    }

    /// <summary>
    /// Initializes a new request
    /// </summary>
    public AddStickerToSetRequest()
        : base("addStickerToSet")
    { }

    /// <inheritdoc />
    public override HttpContent? ToHttpContent()
        =>
        Sticker.Sticker switch
        {
            InputFileStream sticker => ToMultipartFormDataContent(fileParameterName: sticker.FileName!, inputFile: sticker),
            _                       => base.ToHttpContent()
        };
}
