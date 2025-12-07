using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;


public class InputRestriction : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    private Regex allowedChars = new Regex(@"^[\p{L}\s]+$");

    private void Awake()
    {
        if (inputField == null)
            inputField = GetComponent<TMP_InputField>();

        inputField.onValueChanged.AddListener(ValidateText);
    }

    private void ValidateText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        if (!allowedChars.IsMatch(text))
        {
            string filtered = FilterText(text);

            inputField.onValueChanged.RemoveListener(ValidateText);
            inputField.text = filtered;
            inputField.onValueChanged.AddListener(ValidateText);
        }
    }

    private string FilterText(string input)
    {
        var chars = input.ToCharArray();
        string output = "";

        foreach (char c in chars)
        {
            if (allowedChars.IsMatch(c.ToString()))
                output += c;
        }

        return output;
    }
}
