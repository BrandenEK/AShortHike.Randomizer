using AShortHike.Randomizer.Extensions;
using TMPro;
using UnityEngine;

namespace AShortHike.Randomizer.Settings
{
    public class TextInputItem : MonoBehaviour
    {
        private TMP_Text _textField;

        private string input;

        public string FinalInput
        {
            get
            {
                string trimmed = input.Trim().Replace("ap:", "archipelago.gg:");

                return trimmed.Length > 0 ? trimmed : null;
            }
        }

        private void Start()
        {
            _textField = GetComponentInChildren<TMP_Text>();
            input = _textField.text ?? string.Empty;
        }

        private void Update()
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b') // Backspace
                {
                    if (input.Length > 0)
                        input = input.Substring(0, input.Length - 1);
                }
                else if (c == '\n' || c == '\r') // Confirm
                {
                    Main.Randomizer.Settings.CloseTextMenu();
                }
                //else if (c == '\\' && !input.Contains(AP_TEXT))//'a' && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) // Fill ap
                //{
                //    input += AP_TEXT;
                //}
                else // Regular character
                {
                    input += c;
                }
            }

            _textField.text = input.DisplayAsDashIfNull();
        }

        private const string AP_TEXT = "archipelago.gg:";
    }
}
