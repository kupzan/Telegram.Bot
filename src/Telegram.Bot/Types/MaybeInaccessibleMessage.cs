using Telegram.Bot.Converters;

namespace Telegram.Bot.Types;

/// <summary>
/// This object describes a message that can be inaccessible to the bot. It can be one of
/// <list type="bullet">
/// <item><see cref="Message"/></item>
/// <item><see cref="InaccessibleMessage"/></item>
/// </list>
/// </summary>
[JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
[JsonConverter(typeof(MaybeInaccessibleMessageConverter))]
public abstract class MaybeInaccessibleMessage;
