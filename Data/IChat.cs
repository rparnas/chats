namespace Chats.Data;

interface IChat
{
  string[] Emails { get; }
  IMessage[] Messages { get; }
}

interface IMessage
{
  string? Author { get; }
  DateTimeOffset? Utc { get; }
  string? Text { get; }
}
