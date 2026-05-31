using System.Xml.Serialization;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.Repositories;

public class XmlRepository<T>(string filePath) : IRepository<T> where T : class, IIdentifiable
{
    private readonly XmlSerializer _serializer = new(typeof(List<T>));

    private async Task<List<T>> LoadAsync()
    {
        if (!File.Exists(filePath)) return [];
        await using var stream = File.OpenRead(filePath);
        return (List<T>?)_serializer.Deserialize(stream) ?? [];
    }

    private async Task SaveAsync(List<T> data)
    {
        await using var stream = File.Create(filePath);
        _serializer.Serialize(stream, data);
        await stream.FlushAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync() => await LoadAsync();

    public async Task<T?> GetByIdAsync(Guid id)
    {
        var data = await LoadAsync();
        return data.FirstOrDefault(x => x.Id == id);
    }

    public async Task AddAsync(T entity)
    {
        var data = await LoadAsync();
        data.Add(entity);
        await SaveAsync(data);
    }

    public async Task UpdateAsync(T entity)
    {
        var data = await LoadAsync();
        var index = data.FindIndex(x => x.Id == entity.Id);
        if (index == -1) throw new KeyNotFoundException($"Entity {entity.Id} not found.");
        data[index] = entity;
        await SaveAsync(data);
    }

    public async Task DeleteAsync(Guid id)
    {
        var data = await LoadAsync();
        var removed = data.RemoveAll(x => x.Id == id);
        if (removed == 0) throw new KeyNotFoundException($"Entity {id} not found.");
        await SaveAsync(data);
    }
}
