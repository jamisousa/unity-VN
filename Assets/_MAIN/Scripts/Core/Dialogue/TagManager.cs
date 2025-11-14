using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


//inject data into tags contained within dialogue files
public class TagManager
{
    private static readonly Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>()
    {
        { "<mainChar>", () => "Player"},
        { "<time>", () => DateTime.Now.ToString("hh:mm tt")},
        {"<playerLevel", ()=> "0" },
        { "<tempVal1>", () => "0"},
        { "<input>", () => InputPanel.instance.lastInput},
    };

    private static readonly Regex tagRegex = new Regex("<\\w+>");

    public static string Inject(string text)
    {
        if (tagRegex.IsMatch(text))
        {
            foreach (Match match in tagRegex.Matches(text)) {
                if (tags.TryGetValue(match.Value, out var tagValueRequest))
                {
                    text = text.Replace(match.Value, tagValueRequest());
                }
            }
        }

        return text;
    }
}
