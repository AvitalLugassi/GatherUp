namespace GatherUp.Infrastructure.XML;

static class XMLSerializer
{
  public static void Serialize<T>(string path, T obj) where T : class, new()
  {
    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
    using var fileStream = new FileStream(path, FileMode.Create);
    serializer.Serialize(fileStream, obj);
  }

  public static T Deserialize<T>(string path) where T : class, new()
  {
    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
    using var fileStream = new FileStream(path, FileMode.Open);
    return (T?)serializer.Deserialize(fileStream) ?? throw new InvalidOperationException($"Failed to deserialize XML from {path}");
  }
}