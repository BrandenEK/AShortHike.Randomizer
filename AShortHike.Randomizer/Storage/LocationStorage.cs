using AShortHike.Randomizer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AShortHike.Randomizer.Storage;

public class LocationStorage
{
    private readonly Dictionary<string, ItemLocation> _locations;

    public LocationStorage()
    {
        _locations = LoadLocationsFromFile();
        Main.Randomizer.LogHandler.Info($"Loaded {_locations.Count} locations");
    }

    public bool TryGetLocation(string id, out ItemLocation location)
    {
        return _locations.TryGetValue(id, out location);
    }

    public ItemLocation GetLocation(string id)
    {
        return _locations[id];
    }

    public IEnumerable<ItemLocation> GetAllLocations()
    {
        return _locations.Values;
    }

    public IEnumerable<ItemLocation> GetAllCheckedLocations()
    {
        Tags tags = Singleton<GlobalData>.instance.gameData.tags;

        return _locations.Values.Where(x => tags.GetBool($"Opened_{x.Id}"));
    }

    private Dictionary<string, ItemLocation> LoadLocationsFromFile()
    {
        string locationsPath = Path.Combine(DATA_PATH, "locations.json");

        if (!File.Exists(locationsPath))
            throw new FileNotFoundException("Failed to load location data", locationsPath);

        string json = File.ReadAllText(locationsPath);
        return JsonConvert.DeserializeObject<ItemLocation[]>(json)
            .ToDictionary(x => x.Id, x => x);
    }

    private readonly string DATA_PATH = Path.Combine(Environment.CurrentDirectory, "Modding", "data", "Randomizer");
}
