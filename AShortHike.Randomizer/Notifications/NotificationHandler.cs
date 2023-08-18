using AShortHike.Randomizer.Items;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AShortHike.Randomizer.Notifications
{
    public class NotificationHandler
    {

        public void test()
        {
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);
            ItemPrompt prompt = ui.CreateItemPrompt(ItemCreator.CreateReceivedItem("Golden Feather", "Other"));
            GameObject obj = prompt.gameObject;
            obj.GetComponentInChildren<Image>().enabled = false;
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.one;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = new Vector2(-100, -100);
            Object.Destroy(prompt);
        }

        public void UpdateNotifications()
        {

        }
    }
}
