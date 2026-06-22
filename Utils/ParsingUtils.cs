using Chats.Data;
using MimeKit;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Xml.Linq;

namespace Chats.Utils;

static class ParsingUtils
{
  public static class Contacts
  {
    public static Contact[] LoadFromJson(string contactsPath) =>
     DeserializeJsonFile<Contact[]>(contactsPath);
  }

  public static class GChats
  {
    public static GChat[] LoadFromDirectory(string directoryPath)
    {
      var ret = new List<GChat>();

      var directoryInfo = new DirectoryInfo(directoryPath);
      foreach (var subDir in directoryInfo.GetDirectories())
      {
        var infoPath = Path.Combine(subDir.FullName, "group_info.json");
        var info = DeserializeJsonFile<GChatInfoFile>(infoPath);
        if (info is null)
          throw new NotImplementedException($@"missing {infoPath}");

        var messagesPath = Path.Combine(subDir.FullName, "messages.json");
        var messages = DeserializeJsonFile<GChatMessageFile>(messagesPath) ?? new GChatMessageFile { messages = [] };

        var chat = new GChat(info, messages);
        ret.Add(chat);
      }

      return ret
        .ToArray();
    }
  }

  public static class GTalks
  {
    const string GMailChatLabel = "Chat";

    public static void ExtractFromMbox(string[] mboxPaths, string outputDirectoryPath)
    {
      DiskUtils.EnsureDirectory(outputDirectoryPath);

      MimeUtils.ForEachMessageInMbox(mboxPaths, message =>
      {
        var labels = GetGmailLabels(message);
        if (!labels.Contains(GMailChatLabel))
        {
          return;
        }

        var messageId = GetMessageId(message);
        if (messageId is null)
        {
          throw new NotImplementedException("missing message id");
        }

        var eml = MimeUtils.ToEml(message);
        var fileName = Path.Combine(outputDirectoryPath, $@"{messageId.Trim('<', '>')}.eml");
        File.WriteAllBytes(fileName, eml);
      });
    }

    public static GTalk[] LoadFromEml(string[] emlPaths) =>
      MimeUtils.MapEachMessageInEml (emlPaths,  GetGTalk);

    public static GTalk[] LoadFromMbox(string[] mboxPaths) => 
      MimeUtils.MapEachMessageInMbox(mboxPaths, GetGTalk);

    static string[] GetGmailLabels(MimeMessage message)
    {
      return (message.Headers["X-Gmail-Labels"] ?? string.Empty)
        .Split(',')
        .OrderBy(x => x)
        .ToArray();
    }

    static string? GetMessageId(MimeMessage message)
    {
      return message.Headers["Message-ID"] ?? null;
    }

    [return: NotNullIfNotNull(nameof(x))]
    static string? ProcessUsername(string? x) => x?.Split("/")[0].ToLowerInvariant();

    static GTalk GetGTalk(MimeMessage mimeMessage)
    {
      var ret = new List<GTalkLine>();

      var xmlPart = mimeMessage.BodyParts
        .Where(mimeEntity => mimeEntity is TextPart && mimeEntity.ContentType.MimeType == "text/xml")
        .Cast<TextPart>()
        .Single();
      var xml = xmlPart.Text;
      var xmlDoc = XDocument.Parse(xml);
      if (xmlDoc.Root is null)
        throw new NotImplementedException($@"GTalk xml root is missing");

      // <con:conversation xmlns:con="google:archive:conversation"> 
      var con = new XmlElementBag(xmlDoc.Root);
      if (con.XName != "{google:archive:conversation}conversation")
        throw new NotImplementedException($@"GTalk xml unexpected type {con.XName}");

      while (con.TakeNextChild() is { } message)
      {
        /*

          <message aim:new-session="true" to="alice@gmail.com" from="bob@aim" xmlns="jabber:client" xmlns:aim="google:aim">
            <body>hi</body>
            <x stamp="20080104T02:53:18" xmlns="jabber:x:delay" />
            <time ms="1199415198492" xmlns="google:timestamp" />
          </message>
          
          <cli:message to="alice@gmail.com" from="bob@gmail.com" int:cid="11111111111111111111" int:sequence-no="1" int:time-stamp="1330187102456" xmlns:cli="jabber:client" xmlns:int="google:internal">
            <cli:body>hi</cli:body>
            <x stamp="20120225T16:25:02" xmlns="jabber:x:delay" />
            <time ms="1330187102479" xmlns="google:timestamp" />
          </cli:message>
          
          <cli:message to="alice@gmail.com/Talk.v11111111111" iconset="classic" from="bob@gmail.com" int:cid="111111111111111111" int:sequence-no="1" int:time-stamp="1337049004640" xmlns:cli="jabber:client" xmlns:int="google:internal">
            <cli:body>hi</cli:body>
            <met:google-mail-signature xmlns:met="google:metadata">xxxxxxxxxxxxxxxxxxxxxxxxxxx</met:google-mail-signature>
            <x stamp="20120515T02:30:04" xmlns="jabber:x:delay" />
            <time ms="1337049004725" xmlns="google:timestamp" />
          </cli:message>
          
          <cli:message to="abbby@gmail.com" from="bob@gmail.com" int:cid="1111111111111111111" int:sequence-no="3" int:time-stamp="1363049222660" int:interop-stanza="true" int:dual-delivery="true" xmlns:cli="jabber:client" xmlns:int="google:internal">
            <cli:body>hi</cli:body>
            <x stamp="20130312T00:47:02" xmlns="jabber:x:delay" />
            <time ms="1363049222699" xmlns="google:timestamp" />
          </cli:message>
          
          <cli:message to="alice@gmail.com" xml:lang="en" from="bob@gmail.com" int:cid="111111111111111111" int:sequence-no="1" int:time-stamp="1365987260752" int:interop-stanza="true" int:dual-delivery="true" xmlns:cli="jabber:client" xmlns:int="google:internal">
            <cli:body>I f</cli:body>
            <x stamp="20130415T00:54:20" xmlns="jabber:x:delay" />
            <time ms="1365987260807" xmlns="google:timestamp" />
          </cli:message>
          
          <cli:message from="bob@gmail.com" to="alice@gmail.com" int:interop-stanza="true" int:sequence-no="1" int:cid="11111111111111111" int:interop-disable-legacy-archiver="true" xmlns:cli="jabber:client" xmlns:int="google:internal">
            <cli:body>hello</cli:body>
            <nos:x value="disabled" xmlns:nos="google:nosave" />
            <arc:record otr="false" xmlns:arc="http://jabber.org/protocol/archive" />
            <x stamp="20130509T02:12:19" xmlns="jabber:x:delay" />
            <time ms="1368065539942" xmlns="google:timestamp" />
          </cli:message>
          
          <message from="bob@gmail.com" sid="xxxxxxxxxxxxxxxxxxxx" stimestamp="2012-11-30T22:34:13.232Z" to="alice@gmail.com" x-receiver="alice@gmail.com~gt" int:cid="1111111111111111" int:sequence-no="1" int:time-stamp="1354314853230" xmlns="jabber:client" xmlns:int="google:internal">
            <body>hello</body>
            <request xmlns="urn:xmpp:receipts" />
            <x stamp="20121130T22:34:35" xmlns="jabber:x:delay" />
            <time ms="1354314875193" xmlns="google:timestamp" />
          </message>
          
          <cli:message from="private-chat-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx@groupchat.google.com" to="alice@gmail.com" jid="bob@gmail.com" type="groupchat" xmlns:cli="jabber:client">
            <user:x xmlns:user="http://jabber.org/protocol/muc#user">
              <user:invite from="bob@gmail.com/gmail.xxxxxxxx">
                <user:reason>You've been invited to this chat room!</user:reason>
              </user:invite>
            </user:x>
            <cli:body>You've been invited to this chat room!</cli:body>
            <x stamp="20100520T07:12:54" xmlns="jabber:x:delay" />
            <time ms="1274339574776" xmlns="google:timestamp" />
          </cli:message>
          
          <message aim:auto-reply="true" to="bob@gmail.com/gmail.xxxxxxxx" from="alice@aim" xmlns="jabber:client" xmlns:aim="google:aim">
            <body>I am away from my computer right now.</body>
            <x stamp="20080305T02:28:50" xmlns="jabber:x:delay" />
            <time ms="1204684130091" xmlns="google:timestamp" />
          </message>
          
          <cli:message to="bob@gmail.com" from="alice@gmail.com" int:cid="111111111111111111" int:sequence-no="1" int:time-stamp="1281723097091" int:from-jid="alice@gmail.com" xmlns:int="google:internal" xmlns:cli="jabber:client">
            <cli:body>hello</cli:body>
            <x stamp="20100813T18:11:37" xmlns="jabber:x:delay" />
            <time ms="1281723097144" xmlns="google:timestamp" />
          </cli:message>
          
          <cli:message to="bob@gmail.com/Talk.vXXXXXXXXXXX" iconset="classic" from="alice@gmail.com" int:cid="111111111111111111" int:sequence-no="1" int:time-stamp="1337049004640" xmlns:cli="jabber:client" xmlns:int="google:internal">
            <cli:body>hello</cli:body>
            <met:google-mail-signature xmlns:met="google:metadata">xxxxxxxxx-xxxxxxxxxxxxxx-xx</met:google-mail-signature>
            <x stamp="20120515T02:30:04" xmlns="jabber:x:delay" />
            <time ms="1337049004725" xmlns="google:timestamp" />
          </cli:message>
          
          <cli:message to="blak@gmail.com/gmail.XXXXXXXX" from="alice@gmail.com" int:cid="111111111111111111" int:sequence-no="1" int:time-stamp="1260769351816" xmlns:int="google:internal" xmlns:cli="jabber:client">
            <cli:body>yes</cli:body>
            <html xmlns="http://jabber.org/protocol/xhtml-im">
              <body xmlns="http://www.w3.org/1999/xhtml">hello</body>
            </html>
            <x stamp="20091214T05:42:31" xmlns="jabber:x:delay" />
            <time ms="1260769351821" xmlns="google:timestamp" />
          </cli:message>
          
          <cli:message to="blak@gmail.com/gmail.XXXXXXXX" from="alice@gmail.com" xmlns:cli="jabber:client">
            <cli:body>hello</cli:body>
            <x xmlns="jabber:x:event">
              <composing />
            </x>
            <x stamp="20070205T01:52:30" xmlns="jabber:x:delay" />
            <time ms="1170640350030" xmlns="google:timestamp" />
          </cli:message>
          
          <cli:message to="bob@gmail.com/XXXXXXXXXXX" iconset="classic" from="alice@gmail.com" int:cid="11111111111111111" int:sequence-no="12" int:time-stamp="1327356555303" xmlns:cli="jabber:client" xmlns:int="google:internal">
            <cli:body>hello</cli:body>
            <met:google-mail-signature xmlns:met="google:metadata">xxxxxxxx-xxxxxxxxxxxxxxxxxx</met:google-mail-signature>
            <cha:inactive xmlns:cha="http://jabber.org/protocol/chatstates" />
            <x stamp="20120123T22:09:15" xmlns="jabber:x:delay" />
            <time ms="1327356555402" xmlns="google:timestamp" />
          </cli:message>
          
          <message from="alice@gmail.com" sid="XXXXXXXXXXXXX-XXXXXX" stimestamp="2012-11-30T22:34:13.232Z" to="bob@gmail.com" x-receiver="bob@gmail.com~gt" int:cid="1111111111111111" int:sequence-no="1" int:time-stamp="1354314853230" xmlns="jabber:client" xmlns:int="google:internal">
            <body>hello</body>
            <request xmlns="urn:xmpp:receipts" />
            <x stamp="20121130T22:34:35" xmlns="jabber:x:delay" />
            <time ms="1354314875193" xmlns="google:timestamp" />
          </message>
          
          <cli:message from="alice@gmail.com" to="bob@gmail.com" xmlns:cli="jabber:client">
            <con:gap xmlns:con="google:archive:conversation" />
            <time ms="1140548881010" xmlns="google:timestamp" />
          </cli:message>

          <cli:message type="groupchat" from="private-chat-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx@groupchat.google.com" to="bob@gmail.com/gmail.XXXXXXXX" xmlns:cli="jabber:client">
            <cli:body>
              <user:x xmlns:user="http://jabber.org/protocol/muc#user">
                <user:item nick="bob_gmail.com" status="unavailable" jid="alice@gmail.com/gmail.XXXXXXXX" />
              </user:x>
            </cli:body>
            <x stamp="20100520T07:32:51" xmlns="jabber:x:delay" />
            <time ms="1274340771845" xmlns="google:timestamp" />
          </cli:message>
        
        */

        if (message.XName != "{jabber:client}message")
          throw new NotImplementedException($@"GTalk xml unexpected type {message.XName}");
        var messageAttr = new
        {
          from       = message.TakeAttribute         ("from"      ),
          to         = message.TakeAttribute         ("to"        ),
          iconset    = message.TakeAttributeOrDefault("iconset"   ),
          jid        = message.TakeAttributeOrDefault("jid"       ),
          sid        = message.TakeAttributeOrDefault("sid"       ),
          stimestamp = message.TakeAttributeOrDefault("stimestamp"),
          type       = message.TakeAttributeOrDefault("type"      ),
          xReceiver  = message.TakeAttributeOrDefault("x-receiver"),

          aim = new
          {
            autoReply  = message.TakeAttributeOrDefault("{google:aim}auto-reply" ),
            newSession = message.TakeAttributeOrDefault("{google:aim}new-session"),
          },

          google = new
          {
            cid                          = message.TakeAttributeOrDefault("{google:internal}cid"                            ),
            dualDelivery                 = message.TakeAttributeOrDefault("{google:internal}dual-delivery"                  ),
            fromJid                      = message.TakeAttributeOrDefault("{google:internal}from-jid"                       ),
            interopDisableLegacyArchiver = message.TakeAttributeOrDefault("{google:internal}interop-disable-legacy-archiver"),
            interopStanza                = message.TakeAttributeOrDefault("{google:internal}interop-stanza"                 ),
            seqNo                        = message.TakeAttributeOrDefault("{google:internal}sequence-no"                    ),
            timestamp                    = message.TakeAttributeOrDefault("{google:internal}time-stamp"                     ),
          },

          xml = new
          {
            lang = message.TakeAttributeOrDefault("{http://www.w3.org/XML/1998/namespace}lang"),
          },
        };
        var messageElem = new
        {
          google = new
          {
            emailSig   = message.TakeChildSingleOrDefault("{google:metadata}google-mail-signature"), // unknown
            gap        = message.TakeChildSingleOrDefault("{google:archive:conversation}gap"      ), // conversation gap
            timestamp  = message.TakeChildSingle         ("{google:timestamp}time"                ), // utc time in ms
            noSave     = message.TakeChildSingleOrDefault("{google:nosave}x"                      ), // off-the-record metadata
          },

          jabber = new
          {
            body       = message.TakeChildSingleOrDefault("{jabber:client}body"                            ), // plain-text message body
            delay      = message.TakeChildSingleOrDefault("{jabber:x:delay}x"                              ), // utc time
            @event     = message.TakeChildSingleOrDefault("{jabber:x:event}x"                              ), // event notification
            html       = message.TakeChildSingleOrDefault("{http://jabber.org/protocol/xhtml-im}html"      ), // XHTML-IM formatted rich-text
            @record    = message.TakeChildSingleOrDefault("{http://jabber.org/protocol/archive}record"     ), // off-the-record metadata
            inactive   = message.TakeChildSingleOrDefault("{http://jabber.org/protocol/chatstates}inactive"), // sender inactive status
            mucUser    = message.TakeChildSingleOrDefault("{http://jabber.org/protocol/muc#user}x"         ), // Multi-User Chat user metadata container
          },

          xmpp = new
          {
            receipt = message.TakeChildSingleOrDefault("{urn:xmpp:receipts}request"), // XMPP delivery receipt request 
          },
        };
        // message.ThrowIfAnyRemaining();

        // interpretations
        var from  = ProcessUsername(messageAttr.jid ?? messageAttr.from);
        var to    = ProcessUsername(messageAttr.to);
        var isGap = messageElem.google.gap is not null;
        var ms    = long.Parse(messageElem.google.timestamp.TakeAttribute("ms"));
        var utc   = DateTimeOffset.FromUnixTimeMilliseconds(ms);
        var text  = messageElem.jabber.body?.Value;

        // special case: group chat status
        if (messageElem.jabber.body is not null)
        {
          var user = messageElem.jabber.body.TakeChildSingleOrDefault("{http://jabber.org/protocol/muc#user}x");
          if (user is not null)
          {
            var item = user.TakeChildSingleOrDefault("{http://jabber.org/protocol/muc#user}item");
            if (item is not null)
            {
              var _status    = item.TakeAttribute("status");
              var _statusJid = item.TakeAttribute("jid"   );

              var _statusText =
                _status == "available" ? "has entered the chat" :
                _status == "unavailable" ? "has left the chat" :
                throw new NotImplementedException($@"unknown status event status: {_status}");

              ret.Add(new GTalkLine(null, to, $@"<<<{ProcessUsername(_statusJid)} {_statusText}>>>", utc));
              continue;
            }
            else
            {
              throw new NotImplementedException("Unexpected user element in body");
            }
          }
        }

        // special case: group chat invite
        if (messageElem.jabber.mucUser is not null)
        {
          var invite = messageElem.jabber.mucUser.TakeChildSingleOrDefault("{http://jabber.org/protocol/muc#user}invite");
          if (invite is not null)
          {
            var _from   = invite.TakeAttributeOrDefault("from");
            var _reason = invite.TakeChildSingleOrDefault("{http://jabber.org/protocol/muc#user}reason");
            var _text   = (_reason?.Value ?? "Invited to chat") + (_from is null ? string.Empty : $@" by {ProcessUsername(_from)}");
            ret.Add(new GTalkLine(null, to, $@"<<<{_text}>>>", utc));
            continue;
          }
          else
          {
            throw new NotImplementedException("Unexpected user element in message");
          }
        }

        if (isGap && !string.IsNullOrEmpty(text))
        {
          throw new NotImplementedException("gap has body");
        }
        else if (isGap)
        {
          ret.Add(new GTalkLine(null, to, "<<<Off the record>>>", utc));
        }
        else if (string.IsNullOrEmpty(text))
        {
          continue;
        }
        else
        {
          ret.Add(new GTalkLine(from, to, text, utc));
        }
      }
      // con.ThrowIfAnyRemaining();

      return new GTalk(ret.ToArray());
    }
  }

  static T? DeserializeJsonFile<T>(string filePath)
  {
    if (!File.Exists(filePath))
    {
      return default;
    }

    var text = File.ReadAllText(filePath);

    try
    {
      return JsonSerializer.Deserialize<T>(text)!;
    }
    catch (Exception e)
    {
      throw new NotImplementedException($@"Failed to deserialize {filePath}", e);
    }
  }
}
