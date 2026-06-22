using Chats.Data;
using System.Net;

namespace Chats.Utils;

static class RenderingUtils
{
  public static string RenderChatMessages(
    IMessage[] messages, 
    int page, 
    int pageSize,
    string title,
    TimeZoneInfo timeZone)
  {
    static MessagesByDate[] GetMessagesByLocalDate(IMessage[] chatMessages, TimeZoneInfo timeZone)
    {
      var ret = new List<MessagesByDate>();

      var currDate = (string?)null;
      var buffer = new List<Message>();
      void flush()
      {
        if (currDate is not null && buffer.Any())
        {
          ret.Add(new MessagesByDate(currDate, buffer.ToArray()));
        }

        currDate = null;
        buffer.Clear();
      }

      foreach (var rawMessage in chatMessages)
      {
        var utc = rawMessage.Utc;
        var loc = utc is null ? (DateTimeOffset?)null : TimeZoneInfo.ConvertTime(utc.Value, timeZone);
        var date = loc is null ? "Unknown" : loc.Value.ToString("ddd, MMM d, yyyy");
        var time = loc is null ? "Unknown" : loc.Value.ToString("h:mm tt");
        var auth = rawMessage.Author;

        if (currDate is null || currDate != date)
        {
          flush();

          currDate = date;
          buffer = new List<Message>();
        }

        var message = new Message(time, auth, rawMessage.Text);
        buffer.Add(message);
      }
      flush();

      return ret
        .ToArray();
    }

    static MessagePage[] MakePages(MessagesByDate[] days, int targetPageSize)
    {
      var ret = new List<MessagePage>();

      var curr = new List<MessagesByDate>();
      var currCount = 0;

      void flush()
      {
        ret.Add(new MessagePage(curr.ToArray()));

        curr.Clear();
        currCount = 0;
      }

      foreach (var day in days)
      {
        var dayCount = day.Messages.Length;

        if (curr.Count > 0 && currCount + dayCount > targetPageSize)
        {
          flush();
        }

        curr.Add(day);
        currCount += dayCount;
      }

      flush();

      return ret
        .ToArray();
    }

    var messagesByDate = GetMessagesByLocalDate(messages, timeZone);
    var pages = MakePages(messagesByDate, pageSize);

    var pageCount = Math.Max(1, pages.Length);
    var finalPage = Math.Clamp(page, 0, pageCount - 1);
    var visiblePage = pages.Length == 0
        ? new MessagePage(Array.Empty<MessagesByDate>())
        : pages[finalPage];

    var body = new List<string>();
    foreach (var byDate in visiblePage.Days)
    {
      var header = MakeChatDisplayHeaderLine(byDate.Date);
      body.Add(header);

      string? previousTime = null;
      string? previousAuth = null;
      foreach (var message in byDate.Messages)
      {
        var line = MakeChatDisplayLine(
          time: message.Time == previousTime ? null : message.Time,
          author: message.Auth == previousAuth ? null : message.Auth,
          text: message.Text);

        previousTime = message.Time;
        previousAuth = message.Auth;

        body.Add(line);
      }
    }
    body.Add(MakePagerLine(page, pageCount));

    return MakeChatDisplayDoc(string.Join(Environment.NewLine, body.ToArray()));
  }

  static string HtmlEncode(string? s)
  {
    return WebUtility.HtmlEncode(s ?? string.Empty);
  }

  static string MakeChatDisplayDoc(string body)
  {
    var html = $$"""
      <!doctype html>
      <html>
        <head>
          <meta charset="utf-8">
          <style>
            body {
              font-family: Segoe UI, Arial, sans-serif;
              font-size: 14px;
              margin: 16px;
            }
            
            .date-header {
              display: flex;
              align-items: center;
              margin: 16px 0;
              color: #666;
              font-weight: bold;
            }
            .date-header::before,
            .date-header::after {
              content: "";
              flex: 1;
              border-top: 1px solid #ccc;
            }
            .date-header span {
              padding: 0 12px;
            }
            
            .msg {
              display: flex;
              margin: 2px 0;
            }
            
            .pager {
              text-align: center;
              margin: 12px 0;
              color: #666;
            }
            .pager button {
              margin: 0 8px;
            }
            
            .time {
                width: 80px;
                color: gray;
                flex-shrink: 0;
            }
            
            .body {
              min-width: 0;
              overflow-wrap: anywhere;
              white-space: pre-wrap;
            }
            
            .author {
              font-weight: bold;
            }
          </style>
        </head>
        <body>
          {{body}}
        </body>
      </html>
    """;

    return html;
  }

  static string MakeChatDisplayHeaderLine(string text)
  {
    return $"""<div class="date-header">{text}</div>""";
  }

  static string MakeChatDisplayLine(string? time, string? author, string? text)
  {
    var authorHtml = string.IsNullOrEmpty(author) ?
      string.Empty :
      $"""<span class="author">{HtmlEncode(author)}:</span> """;

    var lineHtml = $"""
      <div class="msg">
        <div class="time">{HtmlEncode(time)}</div>
        <div class="body">{authorHtml}{HtmlEncode(text)}</div>
      </div>
    """;

    return lineHtml;
  }

  static string MakePagerLine(int page, int pageCount)
  {
    return $$"""
      <div class="pager">
        <button onclick="chrome.webview.postMessage('prev')" {{(page <= 0 ? "disabled" : "")}}>
          Previous
        </button>

        <span>Page {{page + 1}} of {{pageCount}}</span>

        <button onclick="chrome.webview.postMessage('next')" {{(page >= pageCount - 1 ? "disabled" : "")}}>
          Next
        </button>
      </div>
    """;
  }

  record MessagePage(
    MessagesByDate[] Days);

  record MessagesByDate(
    string Date,
    Message[] Messages);

  record Message(
    string? Time,
    string? Auth,
    string? Text);
}
