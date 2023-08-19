using UnityEngine;

namespace AShortHike.Randomizer.Notifications
{
    public class NotificationMessage
    {
        public RectTransform messageObject;
        public float timeDisplayed;

        public NotificationMessage(RectTransform messageObject, float timeDisplayed)
        {
            this.messageObject = messageObject;
            this.timeDisplayed = timeDisplayed;
        }
    }
}
