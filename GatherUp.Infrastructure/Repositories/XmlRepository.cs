using System.Xml.Serialization;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.Repositories;

public class XmlRepository<T>(string filePath) : IRepository<T> where T : class, IIdentifiable
{
    private readonly XmlSerializer _serializer = new(typeof(List<T>));

    private List<T> Load()
    {
        if (!File.Exists(filePath)) return [];
        using var stream = File.OpenRead(filePath);
        return (List<T>?)_serializer.Deserialize(stream) ?? [];
    }

    private void Save(List<T> data)
    {
        using var stream = File.Create(filePath);
        _serializer.Serialize(stream, data);
    }

    public IEnumerable<T> GetAll() => Load();

    public T? GetById(Guid id) => Load().FirstOrDefault(x => x.Id == id);

    public void Add(T entity)
    {
        var data = Load();
        data.Add(entity);
        Save(data);
    }

    public void Update(T entity)
    {
        var data = Load();
        var index = data.FindIndex(x => x.Id == entity.Id);
        if (index == -1) throw new KeyNotFoundException($"Entity {entity.Id} not found.");
        data[index] = entity;
        Save(data);
    }

    public void Delete(Guid id)
    {
        var data = Load();
        var removed = data.RemoveAll(x => x.Id == id);
        if (removed == 0) throw new KeyNotFoundException($"Entity {id} not found.");
        Save(data);
    }
}
