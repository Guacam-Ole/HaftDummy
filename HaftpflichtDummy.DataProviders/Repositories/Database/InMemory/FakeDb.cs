using HaftpflichtDummy.DataProviders.Models.Database;
using Microsoft.Extensions.Logging;

namespace HaftpflichtDummy.DataProviders.Repositories.Database;

// TODO: Das sollte nicht als Teil des Projekts angesehen werden, sondern die Simulation einer (sehr inperformanten) Datenbank
public class FakeDb
{
    private readonly ILogger<FakeDb> _logger;

    private static readonly Dictionary<string, List<object>> Database = new();

    public FakeDb(ILogger<FakeDb> logger)
    {
        _logger = logger;
    }

    // todo: Nur für Dummydaten in diesem Test 
    public void BulkReplace<T>(string table, List<T> contents)
    {
        _logger.LogDebug("Database has been filled with data for '{tableName}'", table);
        Database[table] = contents.Select(itm => (object)(T)itm).ToList(); //.Select(itm => (object)itm).ToList();
    }

    public void Empty()
    {
        _logger.LogDebug("Database has been emptied");
        Database.Clear();
    }

    public async Task<T> InsertItem<T>(T item)
    {
        if (item == null)
        {
            _logger.LogDebug("Tried to insert an empty item to '{tableName}' failed", typeof(T).Name);
            throw new ArgumentNullException(nameof(item));
        }

        var tableName = typeof(T).Name;
        if (!Database.ContainsKey(tableName))
            Database.Add(tableName, []);
        await Task.Run(() =>
            // Das ist natürlich nicht wirklich async; Soll lediglich den Asynccall bei einer echten DB simulieren
            Database[tableName].Add(item)
        );

        _logger.LogDebug("Inserted new item");
        return item;
    }

    public async Task RemoveItem(TariffFeature item)
    {
        if (!Database.TryGetValue(nameof(TariffFeature), out var value)) throw new KeyNotFoundException();
        var elements = value.Select(tf => (TariffFeature)tf).ToList();
        var existingItem =
            elements.FirstOrDefault(tf => tf.FeatureId == item.FeatureId && tf.TariffId == item.TariffId);
        if (existingItem == null)
        {
            _logger.LogError("Tried to remove non-existing item with FeatureId '{FeatureId}' and TariffId '{TariffId}'",
                item.FeatureId, item.TariffId);
            throw new KeyNotFoundException();
        }

        await Task.Run(() => value.RemoveAt(elements.IndexOf(existingItem)));
        _logger.LogDebug("Removed Feature '{FeatureId}' from Tariff '{TariffId}'", item.FeatureId, item.TariffId);
    }

    public async Task RemoveItem<T>(T item) where T : BaseTable
    {
        string tableName = typeof(T).Name;
        if (!Database.TryGetValue(tableName, out var value)) throw new KeyNotFoundException();
        var elements = value.Select(q => (T)q).ToList();
        var existingItem = elements.FirstOrDefault(q => q.Id == item.Id);
        if (existingItem == null)
        {
            _logger.LogError("Tried to remove non-existing item with id '{Id}' from table '{TableName}'", item.Id,
                tableName);
            throw new KeyNotFoundException();
        }

        await Task.Run(() => value.RemoveAt(elements.IndexOf(existingItem)));
        _logger.LogDebug("Removed item with id '{id}' from '{TableName}'", item.Id, tableName);
    }

    public async Task<IEnumerable<T>> ListItems<T>()
    {
        var tableName = typeof(T).Name;
        var tableData = !Database.TryGetValue(tableName, out var value) ? new List<T>() : value.Select(q => (T)q);
        return await Task.FromResult(tableData);
    }

    public async Task<T?> GetItemById<T>(int id) where T : BaseTable
    {
        var allItems = await ListItems<T>();
        return allItems.FirstOrDefault(itm => itm.Id == id);
    }

    public async Task<T> UpdateItem<T>(int id, T item) where T : BaseTable
    {
        var tableName = typeof(T).Name;
        var allItems = (await ListItems<T>()).ToList();
        var existingItem = allItems.FirstOrDefault(itm => itm.Id == id);
        if (existingItem == null)
        {
            _logger.LogError("Tried to update non-existing item with id '{Id}' from '{TableName}'", id, tableName);
            throw new KeyNotFoundException("No item with that id could be found");
        }

        allItems[allItems.IndexOf(existingItem)] = item;
        _logger.LogDebug("Updated item with id '{Id}' on '{TableName}", id, tableName);
        return item;
    }

    public static int GetNextId<T>()
    {
        var tableName = typeof(T).Name;
        return !Database.TryGetValue(tableName, out var value) ? 1 : value.Count + 1;
    }
}