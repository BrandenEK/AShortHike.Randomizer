using UnityEngine;

namespace AShortHike.Randomizer.Notifications
{
    public class NotificationMessage
    {
        public GameObject messageObject;
        public long timeDisplayed;

        public NotificationMessage(GameObject messageObject, long timeDisplayed)
        {
            this.messageObject = messageObject;
            this.timeDisplayed = timeDisplayed;
        }
    }
}
