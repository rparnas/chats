using MimeKit;

namespace Chats.Utils;

static class MimeUtils
{
  public static void ForEachMessageInEml(string[] emlPaths, Action<MimeMessage> process) =>
    _ = MapEachMessageInEml(emlPaths, x => { process(x); return 0; });
  
  public static void ForEachMessageInMbox(string[] mboxPaths, Action<MimeMessage> process) =>
    _ = MapEachMessageInMbox(mboxPaths, x => { process(x); return 0; });

  public static T[] MapEachMessageInEml<T>(string[] emlPaths, Func<MimeMessage, T?> process)
  {
    var ret = new List<T>();

    foreach (var emlPath in emlPaths.SelectMany(Directory.GetFiles))
    {
      using var stream = File.OpenRead(emlPath);
      var message = MimeMessage.Load(stream);
      var mapped = process(message);
      if (mapped is not null)
      {
        ret.Add(mapped);
      }
    }

    return ret
      .ToArray();
  }

  public static T[] MapEachMessageInMbox<T>(string[] mboxPaths, Func<MimeMessage, T?> process)
  {
    var ret = new List<T>();

    foreach (var mboxPath in mboxPaths)
    {
      using var stream = new FileStream(
        path:       mboxPath,
        mode:       FileMode.Open,
        access:     FileAccess.Read, FileShare.Read,
        bufferSize: 1024 * 1024,
        options:    FileOptions.SequentialScan);

      var parser = new MimeParser(stream, MimeFormat.Mbox);
      while (!parser.IsEndOfStream)
      {
        var message = parser.ParseMessage();
        var mapped = process(message);
        if (mapped is not null)
        {
          ret.Add(mapped);
        }
      }
    }

    return ret
      .ToArray();
  }

  public static byte[] ToEml(MimeMessage mimeMessage)
  {
    using var ms = new MemoryStream();
    mimeMessage.WriteTo(ms);
    return ms.ToArray();
  }
}
