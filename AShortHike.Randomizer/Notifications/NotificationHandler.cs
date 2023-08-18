using AShortHike.Randomizer.Items;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AShortHike.Randomizer.Notifications
{
    public class NotificationHandler
    {
        private RectTransform _notificationHolder;

        private readonly List<NotificationMessage> _messages = new();

        public void AddNotification(string itemName, string playerName)
        {
            if (_notificationHolder == null)
                SetupNotifications();

            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);
            ItemPrompt prompt = ui.CreateItemPrompt(ItemCreator.CreateReceivedItem(itemName, playerName));
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
            _messages.Add(new NotificationMessage(rect, Time.realtimeSinceStartup));
            Object.Destroy(prompt);
        }

        public void UpdateNotifications()
        {
            for (int i = 0; i < _messages.Count; i++)
            {
                NotificationMessage message = _messages[i];
                if (message.timeDisplayed < Time.realtimeSinceStartup - DISPLAY_SECONDS)
                {
                    Object.Destroy(message.messageObject.gameObject);
                    _messages.RemoveAt(i);
                    i--;
                }
                else
                {
                    float xPos = _notificationHolder.rect.width - message.messageObject.rect.width;
                    float yPos = _notificationHolder.rect.height / 2 + (i + 1) * -20 - 50;
                    message.messageObject.anchoredPosition = new Vector2(xPos, yPos);
                }
            }
        }

        private void SetupNotifications()
        {
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);
            var obj = new GameObject("Notification Holder", typeof(RectTransform));
            ui.AddUI(obj, true);

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = Vector2.zero;

            _notificationHolder = rect;
        }

        private const float DISPLAY_SECONDS = 4f;
    }
}
