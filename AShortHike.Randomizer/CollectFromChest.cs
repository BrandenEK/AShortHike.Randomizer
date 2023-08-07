using System;
using UnityEngine;

namespace AShortHike.Randomizer
{
    public class CollectFromChest : MonoBehaviour, ICollectable
    {
        public event Action onCollect;

        public CollectableItem item;

        public void Collect()
        {
            onCollect?.Invoke();

            Singleton<GameServiceLocator>.instance.levelController.player.StartCoroutine(item.PickUpRoutine(1));
            Destroy(gameObject);
        }
    }
}
