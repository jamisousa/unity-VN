using System.Text.RegularExpressions;
using UnityEngine;

// handles parsing raw dialogue lines into structured dialogue data

namespace DIALOGUE { 

    public class DialogueParser
    {

        private const string commandRegexPattern = "\\w*[^\\s]\\(";

        public static DIALOGUE_LINES Parse(string rawLine)
        {   
            (string speaker, string dialogue, string commands) = RipContent(rawLine);   

            return new DIALOGUE_LINES(speaker, dialogue, commands);
        }

        private static (string, string, string) RipContent(string rawLine)
        {
            string speaker = "", dialogue = "", commands = "";

            int dialogueStart = -1;
            int dialogueEnd = -1;
            bool isEscaped = false;

            for (int i = 0; i < rawLine.Length; i++)
            {
                char current = rawLine[i];

                if (current == '\\')
                {
                    isEscaped = !isEscaped;
                }
                else if (current == '"' && !isEscaped)
                {
                    if (dialogueStart == -1)
                    {
                        dialogueStart = i;
                    }
                    else if (dialogueEnd == -1)
                    {
                        dialogueEnd = i;
                        break; // We found both start and end, exit loop
                    }
                }
                else
                {
                    isEscaped = false; // Reset escape status if current char is not '\'
                }

            }

            //Identify command pattern
            Regex commandRegex = new Regex(commandRegexPattern);
            Match commandMatch = commandRegex.Match(rawLine);
            int commandStart =  -1;
            if (commandMatch.Success)
            {
                commandStart = commandMatch.Index;

                if(dialogueStart == -1 && dialogueEnd == -1)
                {
                    return ("", "", rawLine.Trim());

                }

            }

            //If we got here then we either got a dialogue or multi word argument in a command, find out which
            if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
            {
                //got dialogue
                speaker = rawLine.Substring(0, dialogueStart).Trim();
                dialogue = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");
                if(commandStart != -1)
                {
                    commands = rawLine.Substring(commandStart).Trim();
                }
            }
            else if (commandStart != -1d && dialogueStart > commandStart)
            {
                commands = rawLine;
            }
            else
            {
                speaker = rawLine;
            }

            return (speaker, dialogue, commands);
        }
    }
}
