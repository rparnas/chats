using System.Xml.Linq;

namespace Chats.Data;

static class XmlUtils
{
  public static bool Is(XElement xe, string xName)
  {
    var xn = ParseXName(xName);
    return
      xe.Name.NamespaceName == xn.NamespaceName &&
      xe.Name.LocalName == xn.LocalName;
  }

  public static XName ParseXName(string xName)
  {
    if (xName.StartsWith('{'))
    {
      var end = xName.IndexOf('}');
      if (end < 0)
        throw new FormatException($"Invalid xName: {xName}");

      var ns = xName[1..end];
      var name = xName[(end + 1)..];

      return XName.Get(name, ns);
    }
    return XName.Get(xName);
  }
}

class XmlElementBag
{
  readonly Dictionary<XName, XAttribute> Attributes;
  readonly List<XElement> ReversedChildren;
  readonly XElement Element;

  public XName XName => Element.Name;
  public string? Value => Element.Value;

  public XmlElementBag(XElement xe)
  {
    Attributes = new Dictionary<XName, XAttribute>();
    foreach (var x in xe.Attributes().Where(x => !x.IsNamespaceDeclaration))
    {
      Attributes[x.Name] = x;
    }

    ReversedChildren = xe.Elements()
      .Reverse()
      .ToList();

    Element = xe;
  }

  public string TakeAttribute(string xName) =>
    TakeAttributeOrDefault(xName) ?? throw new NotImplementedException($@"Missing attribute {xName} on {Element.Name}");

  public string? TakeAttributeOrDefault(string xName) =>
    Attributes.Remove(XmlUtils.ParseXName(xName), out var attribute) ? attribute.Value : null;

  public XmlElementBag TakeChildSingle(string xName) =>
    TakeChildSingleOrDefault(xName) ?? throw new NotImplementedException($@"Missing element {xName} on {Element.Name}");

  public XmlElementBag? TakeChildSingleOrDefault(string xName)
  {
    var key = XmlUtils.ParseXName(xName);

    var matchIndex = (int?)null;
    for (var i = ReversedChildren.Count - 1; i >= 0; i--)
    {
      var element = ReversedChildren[i];
      if (element.Name == key)
      {
        if (matchIndex.HasValue)
          throw new NotImplementedException($@"Element {xName} appears multiple times in {Element.Name}");
        else
          matchIndex = i;
      }
    }

    if (!matchIndex.HasValue)
      return null;

    var ret = ReversedChildren[matchIndex.Value];
    ReversedChildren.RemoveAt(matchIndex.Value);
    return new XmlElementBag(ret);
  }

  public XmlElementBag? TakeNextChild()
  {
    if (ReversedChildren.Count == 0)
      return null;

    var ret = ReversedChildren[ReversedChildren.Count - 1];
    ReversedChildren.RemoveAt(ReversedChildren.Count - 1);
    return new XmlElementBag(ret);
  }

  public void ThrowIfAnyRemaining()
  {
    var messages = new List<string>();

    if (Attributes.Any())
    {
      var keys = string.Join(", ", Attributes.Keys.Select(x => x.ToString()).ToArray());
      messages.Add($@"attributes ({keys})");
    }

    if (ReversedChildren.Any())
    {
      var keys = string.Join(", ", ReversedChildren.AsEnumerable().Reverse().Select(x => x.Name.ToString()).ToArray());
      messages.Add($@"children ({keys})");
    }

    if (messages.Any())
    {
      var finalMessage = string.Join(", ", messages.ToArray());

      throw new NotImplementedException($@"Unknowns on {Element.Name}: {finalMessage}");
    }
  }
}
