using System.Data.Common;
using System.Security.AccessControl;
using HaftpflichtDummy.DataProviders.Models.Database;

namespace HaftpflichtDummy.DataProviders.Repositories.Database;

// TODO: Das sollte nicht als Teil des Projekts angesehen werden, sondern die Simulation einer (sehr inperformanten) Datenbank
public class FakeDb
{
    private static readonly Dictionary<string, List<object>> Database = new();

    // todo: Nur für Dummydaten in diesem Test 
    public void BulkReplace<T>(string table, List<T> contents)
    {
        Database[table] = contents.Select(itm=>(object)(T)itm).ToList(); //.Select(itm => (object)itm).ToList();
    }

    public void Empty()
    {
        Database.Clear();
    }

    public async Task<T> InsertItem<T>(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        var tableName = typeof(T).Name;
        if (!Database.ContainsKey(tableName))
            Database.Add(tableName, []);
        await Task.Run(() =>
            // Das ist natürlich nicht wirklich async; Soll lediglich den Asynccall bei einer echten DB simulieren
            Database[tableName].Add(item)
        );
        return item;
    }

    public async Task RemoveItem(TariffFeature item)
    {
        if (!Database.TryGetValue(nameof(TariffFeature), out var value)) throw new KeyNotFoundException();
        var elements = value.Select(tf => (TariffFeature)tf).ToList();
        var existingItem =
            elements.FirstOrDefault(tf => tf.FeatureId == item.FeatureId && tf.TariffId == item.TariffId);
        if (existingItem == null) throw new KeyNotFoundException();
        await Task.Run(() => value.RemoveAt(elements.IndexOf(existingItem)));
    }

    public async Task RemoveItem<T>(T item) where T : BaseTable
    {
        if (!Database.TryGetValue(typeof(T).Name, out var value)) throw new KeyNotFoundException();
        var elements = value.Select(q => (T)q).ToList();
        var existingItem = elements.FirstOrDefault(q => q.Id == item.Id);
        if (existingItem == null) throw new KeyNotFoundException();
        await Task.Run(() => value.RemoveAt(elements.IndexOf(existingItem)));
    }

    public async Task<IEnumerable<T>> ListItems<T>()
    {
        var tableName = typeof(T).Name;
        var tableData = !Database.TryGetValue(tableName, out var value) ? new List<T>() : value.Select(q => (T)q);
        // TODO: Das ist natürlich nicht wirklich async
        return await Task.FromResult(tableData);
    }

    public async Task<T?> GetItemById<T>(int id) where T : BaseTable
    {
        var allItems = await ListItems<T>();
        return allItems.FirstOrDefault(itm => itm.Id == id);
    }

    public async Task<T> UpdateItem<T>(int id, T item) where T : BaseTable
    {
        var allItems = (await ListItems<T>()).ToList();
        var existingItem = allItems.FirstOrDefault(itm => itm.Id == id);
        if (existingItem == null) throw new KeyNotFoundException("No item with that id could be found");
        allItems[allItems.IndexOf(existingItem)] = item;
        return item;
    }

    public int GetNextId<T>()
    {
        var tableName = typeof(T).Name;
        return !Database.TryGetValue(tableName, out var value) ? 1 : value.Count + 1;
    }
}