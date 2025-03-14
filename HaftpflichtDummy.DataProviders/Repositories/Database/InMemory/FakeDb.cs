using System.Data.Common;
using HaftpflichtDummy.DataProviders.Models.Database;

namespace HaftpflichtDummy.DataProviders.Repositories.Database;

public class FakeDb
{
    private static Dictionary<string, List<object>> Database;

    public async Task InsertItem<T>(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        var tableName = typeof(T).Name;
        if (!Database.ContainsKey(tableName))
            Database.Add(tableName, []);
        await Task.Run(() =>
            // TODO: Das ist natürlich nicht wirklich async
            Database[tableName].Add(item)
        );
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

    public async Task UpdateItem<T>(int id, T item) where T : BaseTable
    {
        var allItems = (await ListItems<T>()).ToList();
        var existingItem = allItems.FirstOrDefault(itm => itm.Id == id);
        if (existingItem == null) throw new KeyNotFoundException("No item with that id could be found");
        allItems[allItems.IndexOf(existingItem)] = item;
    }

    public int GetNextId<T>()
    {
        var tableName = typeof(T).Name;
        return !Database.TryGetValue(tableName, out var value) ? 1 : value.Count + 1;
    }
}