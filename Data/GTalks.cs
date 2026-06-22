namespace Chats.Data;

class GTalk : IChat
{
  public GTalkLine[] Lines { get; init; }

  // IChat
  public string[] Emails { get; init; }
  public IMessage[] Messages => Lines;

  public GTalk(GTalkLine[] lines)
  {
    var participants = Array.Empty<string>()
      .Concat(lines.Select(x => x.Author))
      .Concat(lines.Select(x => x.To))
      .Distinct()
      .OfType<string>()
      .OrderBy(x => x)
      .ToArray();

    Lines = lines;
    Emails = participants.ToArray();
  }

  public override string ToString()
  {
    return string.Join(", ", Emails.ToArray());
  }
}

record GTalkLine(
  string? From,
  string To,
  string Text,
  DateTimeOffset Utc) : IMessage
{
  // IMessage
  public string? Author => From;
  DateTimeOffset? IMessage.Utc => Utc;
}
