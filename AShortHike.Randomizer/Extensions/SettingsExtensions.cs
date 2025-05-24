using AShortHike.Randomizer.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AShortHike.Randomizer.Extensions
{
    public static class SettingsExtensions
    {
        public static string DisplayAsDashIfNull(this string str)
        {
            return string.IsNullOrEmpty(str) ? "---" : str;
        }

        public static string DisplayAsHidden(this string str, bool hidden)
        {
            if (string.IsNullOrEmpty(str))
                return "---";

            if (hidden)
                return new string('*', str.Length);

            return str;
        }

        public static string DisplayONOFF(this bool b)
        {
            return b ? "ON" : "OFF";
        }

        /// <summary>
        /// Creates a text object at the top of a menu
        /// </summary>
        public static LinearMenu AddTitle(this LinearMenu menu, string title)
        {
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);

            GameObject titleObject = ui.CreateTextMenuItem(title);
            titleObject.transform.SetParent(menu.transform, false);
            titleObject.transform.SetAsFirstSibling();
            return menu;
        }

        /// <summary>
        /// Creates an editable text input field
        /// </summary>
        public static LinearMenu AddTextInput(this LinearMenu menu, string text, bool secret)
        {
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);

            GameObject valueObject = ui.CreateTextMenuItem(text);
            valueObject.transform.SetParent(menu.transform, false);
            valueObject.transform.SetAsFirstSibling();
            valueObject.GetComponentInChildren<TMP_Text>().color = Color.red;
            
            var input = valueObject.AddComponent<TextInputItem>();
            if (secret)
                input.IsSecret = true;

            return menu;
        }

        /// <summary>
        /// Rebuilds and centers the content in the menu
        /// </summary>
        public static LinearMenu Finalize(this LinearMenu menu, int index)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(menu.transform as RectTransform);
            (menu.transform as RectTransform).CenterWithinParent();
            menu.selectedIndex = index;
            return menu;
        }
    }
}
