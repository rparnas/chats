using System.Globalization;

namespace Chats.Data;

record GChat(
  GChatInfoFile InfoFile,
  GChatMessageFile MessageFile) : IChat
{
  // IChat
  public string[] Emails => InfoFile.members.Select(x => x.email).ToArray();
  public IMessage[] Messages => MessageFile.messages;

  public override string ToString() => InfoFile.ToString();
}

class GChatInfoFile
{
  public required GChatMember[] members { get; set; }

  public override string ToString()
  {
    return string.Join(", ", members.Select(x => x.shortname).OrderBy(x => x).ToArray());
  }
}

class GChatMessageFile
{
  public required GChatMessage[] messages { get; set; }
}

class GChatAnnotation
{
  public required int start_index { get; set; }
  public required int length { get; set; }
  public GChatUrlMetadata? url_metadata { get; set; }
}

class GChatMember
{
  public required string email { get; set; }
  public required string name { get; set; }
  public required string user_type { get; set; }

  public string shortname =>
    (name.Contains('(') ? name.Substring(0, name.IndexOf('(')).Trim() : name) + $@" ({email})";

  public override string ToString()
  {
    return shortname;
  }
}

class GChatMessage : IMessage
{
  public GChatAnnotation[]? annotations { get; set; }
  public string? created_date { get; set; }
  public required GChatMember creator { get; set; }
  public string? text { get; set; }
  public required string topic_id { get; set; }
  public required string message_id { get; set; }

  // IMessage
  public string? Author => creator.shortname;
  public string? Text => text;
  public DateTimeOffset? Utc => GetCreatedTimeUtc();

  public DateTimeOffset? GetCreatedTimeUtc()
  {
    if (created_date is null)
      return null;

    var input = created_date
      .Replace('\u202F', ' ');

    if (!input.EndsWith(" UTC", StringComparison.Ordinal))
    {
      throw new NotImplementedException("unexpected created_date format");
    }

    return DateTimeOffset.ParseExact(
      input:          input,
      format:         "dddd, MMMM d, yyyy 'at' h:mm:ss tt 'UTC'",
      formatProvider: CultureInfo.InvariantCulture,
      styles:         DateTimeStyles.AssumeUniversal);
  }

  public override string ToString()
  {
    return $@"{creator} | {text}";
  }
}

class GChatUrlMetadata
{
  public required string title { get; set; }
  public required string snippet { get; set; }
  public required string image_url { get; set; }
  public required GChatUrl url { get; set; }
}

class GChatUrl
{
  public required string private_do_not_access_or_else_safe_url_wrapped_value { get; set; }
}
