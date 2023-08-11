using AShortHike.Randomizer.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
                if (input == null)
                    return input;

                string trimmed = input.Trim();
                return trimmed.Length > 0 ? trimmed : null;
            }
        }

        private void Start()
        {
            _textField = GetComponentInChildren<TMP_Text>();
            input = _textField.text;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
                input = "localhost";
            else if (Input.GetKeyDown(KeyCode.I))
                input = "archipelago.gg:24242";
            else if (Input.GetKeyDown(KeyCode.U))
                input = "";

            _textField.text = input.DisplayAsDashIfNull();
        }
    }
}
