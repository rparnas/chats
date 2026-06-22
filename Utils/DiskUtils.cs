using System.Diagnostics.CodeAnalysis;

namespace Chats.Utils;

static class DiskUtils
{
  public static void EnsureDirectory(string path)
  {
    if (!Directory.Exists(path))
    {
      Directory.CreateDirectory(path);
    }
  }

  public static bool IsValidFile([NotNullWhen(true)] string? path) =>
    !string.IsNullOrEmpty(path) && File.Exists(path);

  public static bool IsValidFolder([NotNullWhen(true)] string? path) =>
    !string.IsNullOrEmpty(path) && Directory.Exists(path);

  public static string? PickFile(string title, string filter)
  {
    var ofd = new OpenFileDialog
    {
      Filter = filter,
      Title = title,
    };
    return ofd.ShowDialog() == DialogResult.OK ? ofd.FileName : null;
  }

  public static string? PickFolder(string description)
  {
    var fbd = new FolderBrowserDialog
    {
      Description = description,
    };
    return fbd.ShowDialog() == DialogResult.OK ? fbd.SelectedPath : null;
  }
}

static class FileFilters
{
  public const string JsonFilter = "JSON files (*.json)|*.json";
  public const string MboxFilter = "MBOX files (*.mbox)|*.mbox";
}