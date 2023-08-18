using AShortHike.Randomizer.Items;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AShortHike.Randomizer.Notifications
{
    public class NotificationHandler
    {
        private RectTransform _notificationHolder;

        public void AddNotification()
        {
            if (_notificationHolder == null)
                SetupNotifications();

            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);
            ItemPrompt prompt = ui.CreateItemPrompt(ItemCreator.CreateReceivedItem("Golden Feather", "Other"));
            GameObject obj = prompt.gameObject;
            obj.GetComponentInChildren<Image>().enabled = false;

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.SetParent(_notificationHolder, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 1);

            float xPos = _notificationHolder.rect.width - rect.rect.width;
            float yPos = _notificationHolder.rect.height / 2 + _notificationHolder.childCount * -20 - 50;

            rect.anchoredPosition = new Vector2(xPos, yPos);
            Object.Destroy(prompt);
        }

        public void UpdateNotifications()
        {

        }

        private void SetupNotifications()
        {
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);
            var obj = new GameObject("Notification Holder", typeof(RectTransform));
            ui.AddUI(obj, true);

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.35f, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = Vector2.zero;

            //rect.gameObject.AddComponent<Image>();

            _notificationHolder = rect;
        }
    }
}
