using AShortHike.Randomizer.Connection;
using AShortHike.Randomizer.Items;
using UnityEngine;

namespace AShortHike.Randomizer
{
    public class Randomizer
    {
        private readonly ConnectionHandler _connection = new();
        private readonly ItemHandler _items = new();
        private readonly DataStorage _data = new();

        public ConnectionHandler Connection => _connection;
        public ItemHandler Items => _items;
        public DataStorage Data => _data;

        public void OnSceneLoaded()
        {
            _items.LoadItemObjects();
            _items.ReplaceWorldObjectsWithChests();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backslash))
            {
                _connection.Connect("localhost", "Player", null);
            }
        }
    }
}
