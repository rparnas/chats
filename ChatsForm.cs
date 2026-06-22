using Chats.Data;
using Chats.Utils;

namespace Chats;

public partial class ChatsForm : Form
{
  static readonly int ChatPageSize = 5000;
  static readonly StringComparer EmailComparer = StringComparer.OrdinalIgnoreCase;

  string? _ContactsPath;
  string? _GChatPath;
  string? _GTalkPath;
  string? ContactsPath { get => _ContactsPath; set { _ContactsPath = value; if (Settings.Default.ContactsPath != value) { Settings.Default.ContactsPath = value; Settings.Default.Save(); } tb_ContactsPath.Text = value; } }
  string? GChatPath    { get => _GChatPath;    set { _GChatPath    = value; if (Settings.Default.GChat        != value) { Settings.Default.GChat        = value; Settings.Default.Save(); } tb_GChatPath.Text    = value; } }
  string? GTalkPath    { get => _GTalkPath;    set { _GTalkPath    = value; if (Settings.Default.GTalkPath    != value) { Settings.Default.GTalkPath    = value; Settings.Default.Save(); } tb_GTalkPath.Text    = value; } }

  DisplayChat? CurrentChat;
  int CurrentChatPage;
  TimeZoneInfo? TimeZone;

  public ChatsForm()
  {
    InitializeComponent();

    ContactsPath = DiskUtils.IsValidFile  (Settings.Default.ContactsPath) ? Settings.Default.ContactsPath : null;
    GTalkPath    = DiskUtils.IsValidFolder(Settings.Default.GTalkPath   ) ? Settings.Default.GTalkPath    : null;
    GChatPath    = DiskUtils.IsValidFolder(Settings.Default.GChat       ) ? Settings.Default.GChat        : null;

    CurrentChat = null;
    CurrentChatPage = 0;
    TimeZone = TimeZoneInfo.Local;

    Shown += async (s, e) =>
    {
      // wv
      await wv_Chats.EnsureCoreWebView2Async();
      wv_Chats.CoreWebView2.WebMessageReceived += (s, e) =>
      {
        var msg = e.TryGetWebMessageAsString();

        if (msg == "prev")
          ShowChat(CurrentChat, CurrentChatPage - 1);

        if (msg == "next")
          ShowChat(CurrentChat, CurrentChatPage + 1);
      };
      wv_Chats.NavigateToString(string.Empty);

      Wait(LoadData);
      if (lb_Chats.Items.Count > 0)
      {
        tc.SelectedIndex = 1;
      }
    };

    btn_ContactsPath.Click += (s, e) => ContactsPath = DiskUtils.PickFile("Pick Contacts File", FileFilters.JsonFilter) ?? ContactsPath;
    tb_ContactsPath.TextChanged += (s, e) => ContactsPath = tb_ContactsPath.Text;

    btn_GChatPath.Click += (s, e) => GChatPath = DiskUtils.PickFolder("Pick Google Groups Path") ?? GChatPath;
    tb_GChatPath.TextChanged += (s, e) => GChatPath = tb_GChatPath.Text;

    btn_GTalkPath.Click += (s, e) => GTalkPath = DiskUtils.PickFolder("Pick Google Talk Path") ?? GTalkPath;
    tb_GTalkPath.TextChanged += (s, e) => GTalkPath = tb_GTalkPath.Text;

    btn_LoadData.Click += (s, e) =>
    {
      Wait(LoadData);
      tc.SelectedIndex = 1;
    };

    btn_ExtractGTalkFromMBox.Click += (s, e) =>
    {
      var mboxPath = DiskUtils.PickFile("Choose Google Takeouts GMail MBOX", FileFilters.MboxFilter);
      if (mboxPath is null)
      {
        return;
      }

      var outPath = DiskUtils.PickFolder("Choose .eml export directory");
      if (outPath is null)
      {
        return;
      }

      Wait(() => ParsingUtils.GTalks.ExtractFromMbox([mboxPath], outPath));
    };

    cb_TimeZone.DataSource = TimeZoneInfo.GetSystemTimeZones();
    cb_TimeZone.DisplayMember = nameof(TimeZoneInfo.DisplayName);
    cb_TimeZone.ValueMember = nameof(TimeZoneInfo.Id);
    cb_TimeZone.SelectedValue = TimeZone.Id;
    cb_TimeZone.SelectedIndexChanged += (s, e) =>
    {
      TimeZone = (TimeZoneInfo)cb_TimeZone.SelectedItem!;
      ShowChat(CurrentChat, CurrentChatPage);
    };

    lb_Chats.SelectedIndexChanged += (s, e) =>
    {
      var chat = lb_Chats.SelectedIndex >= 0 ? (DisplayChat)lb_Chats.Items[lb_Chats.SelectedIndex] : null;
      ShowChat(chat, 0);
    };
  }

  void LoadData()
  {
    // Load Data
    var gChat = DiskUtils.IsValidFolder(GChatPath) ? ParsingUtils.GChats.LoadFromDirectory(GChatPath) : [];
    var gTalk = DiskUtils.IsValidFolder(GTalkPath) ? ParsingUtils.GTalks.LoadFromEml([GTalkPath]) : [];
    var allChats = Array.Empty<IChat>().Concat(gChat).Concat(gTalk).ToArray();
    var contacts = DiskUtils.IsValidFile(ContactsPath) ? ParsingUtils.Contacts.LoadFromJson(ContactsPath) : [];
    var self = contacts.SingleOrDefault(x => x.isSelf);

    // Organize Data
    var messagesByContactDict = new Dictionary<string, List<IMessage>>();
    foreach (var chat in allChats)
    {
      var participantsWithoutSelf = chat.Emails
        .Select(pEmail =>
        {
          if (self is not null && self.emails.Contains(pEmail, EmailComparer))
          {
            return null;
          }

          var contact = contacts.SingleOrDefault(x => x.emails.Contains(pEmail, EmailComparer));
          if (contact is null)
          {
            return $@"({pEmail})";
          }

          return contact.name;
        })
        .OfType<string>()
        .OrderBy(name => name)
        .ToArray();

      var key = string.Join(" + ", participantsWithoutSelf);
      if (!messagesByContactDict.TryGetValue(key, out var list))
      {
        list = new List<IMessage>();
        messagesByContactDict[key] = list;
      }
      list.AddRange(chat.Messages);
    }
    var chatsByContact = messagesByContactDict
      .Select(x => new DisplayChat(x.Key, x.Value.OrderBy(x => x.Utc).ToArray()))
      .OrderBy(x => x.Name)
      .ToArray();

    // Update UI
    lb_Chats.Items.Clear();
    wv_Chats.NavigateToString(string.Empty);
    foreach (var chat in chatsByContact)
    {
      lb_Chats.Items.Add(chat);
    }
  }

  void ShowChat(DisplayChat? chat, int page)
  {
    var html = chat is null ? 
      string.Empty :
      RenderingUtils.RenderChatMessages(chat.Messages, page, ChatPageSize, chat.Name, TimeZone!);
    
    wv_Chats.NavigateToString(html);

    CurrentChat = chat;
    CurrentChatPage = page;
  }

  void Wait(Action action)
  {
    var oldCursor = Cursor;

    try
    {
      Cursor = Cursors.WaitCursor;
      action();
    }
    finally
    {
      Cursor = oldCursor;
    }
  }

  record DisplayChat(
    string Name,
    IMessage[] Messages)
  {
    public override string ToString() => Name;
  }
}
