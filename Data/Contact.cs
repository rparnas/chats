namespace Chats.Data;

public class Contact
{
  public required string name { get; set; }
  public required string[] emails { get; set; }
  public bool isSelf { get; set; }

  public override string ToString() => name;
}
