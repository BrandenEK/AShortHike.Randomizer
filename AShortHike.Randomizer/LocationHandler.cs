using AShortHike.Randomizer.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace AShortHike.Randomizer;

public class LocationHandler
{
    private readonly Dictionary<string, ItemLocation> _locations;

    public LocationHandler()
    {
        _locations = LoadLocationsFromFile();
        Main.Log($"Loaded {_locations.Count} locations");
    }

    public bool TryGetLocation(string id, out ItemLocation location)
    {
        return _locations.TryGetValue(id, out location);
    }

    public ItemLocation GetLocation(string id)
    {
        return _locations[id];
    }

    public ReadOnlyDictionary<string, ItemLocation> GetAllLocations()
    {
        return new ReadOnlyDictionary<string, ItemLocation>(_locations);
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
