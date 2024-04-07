using AShortHike.Randomizer.Connection;
using AShortHike.Randomizer.Models;
using AShortHike.Randomizer.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AShortHike.Randomizer;

public class ItemMapper(ConnectionHandler connection, LocationStorage locations)
{
    private Dictionary<ItemLocation, Item> _mappedItems = new();
    private readonly ConnectionHandler _connection = connection;
    private readonly LocationStorage _locations = locations;

    public async Task<bool> OnConnect()
    {
        Main.Log("Scouting all locations");

        try
        {
            await ScoutAllLocations();
            return true;
        }
        catch
        {
            Main.LogError("Failed to scout locations");
            _connection.Disconnect();
            _mappedItems.Clear();

            return false;
        }
    }

    public void OnDisconnect()
    {
        Main.Log("Clearing all mapped locations");
        _mappedItems.Clear();
    }

    private async Task ScoutAllLocations()
    {
        _mappedItems = await _connection.ScoutLocations(_locations.GetAllLocations());

        Main.LogWarning($"Scouted {_mappedItems.Count} locations");
    }

    public Item GetItemAtLocation(ItemLocation location)
    {
        return _mappedItems.TryGetValue(location, out Item item) ? item : Item.Unknown;
    }
}
