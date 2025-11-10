using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE
{
    public class DL_SPEAKER_DATA
    {
        public string rawData { get; private set; } = string.Empty;

        //needed to display character on screen
        public string name, castName;
        public string displayName => isCastingName ? castName : name;
        public Vector2 castPosition;
        public List<(int layer, string expression)> CastExpressions { get; set; }

        public bool makeCharacterEnter = false;

        private const string NAMECAST_ID = " as ";
        private const string POSITIONCAST_ID = " at ";
        private const string EXPRESSIONCAST_ID = " [";
        private const string AXISDELIMITER = ":";
        private const char EXPRESSIONLAYER_JOINER = ',';
        private const char EXPRESSIONLAYER_DELIMITER = ':';

        public bool isCastingName => castName != string.Empty;
        public bool isCastingPosition = false;
        public bool isCastingExpressions => CastExpressions.Count > 0;


        //key to cause a character to enter the screen
        private const string ENTER_KEYWORD = "enter ";

        private string ProcessKeywords(string rawSpeaker)
        {
            //check for enter keyword and remove it from the raw speaker string
            if (rawSpeaker.StartsWith(ENTER_KEYWORD))
            {
                rawSpeaker = rawSpeaker.Substring(ENTER_KEYWORD.Length);
                makeCharacterEnter = true;
            }
            return rawSpeaker;
        }

        public DL_SPEAKER_DATA(string rawSpeaker)
        {
            rawData = rawSpeaker;
            rawSpeaker = ProcessKeywords(rawSpeaker);

            //regex to look for necessary words
            string pattern = @$"{NAMECAST_ID}|{POSITIONCAST_ID}|{EXPRESSIONCAST_ID.Insert(EXPRESSIONCAST_ID.Length - 1, @"\")}";

            MatchCollection matches = Regex.Matches(rawSpeaker, pattern);

            //populate this data to avoid null refs to values
            castName = "";
            castPosition = Vector2.zero;
            CastExpressions = new List<(int layer, string expression)>();


            //if no matches, then the entire line is the speaker name
            if (matches.Count == 0)
            {
                name = rawSpeaker;

                return;
            }

            //isolate speaker name from the casting data
            int index = matches[0].Index;
            name = rawSpeaker.Substring(0, index);

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                int startIndex = 0, endIndex = 0;


                if (match.Value == NAMECAST_ID)
                {
                    startIndex = match.Index + NAMECAST_ID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    castName = rawSpeaker.Substring(startIndex, endIndex - startIndex);
                }
                else if (match.Value == POSITIONCAST_ID)
                {
                    isCastingPosition = true;
                    startIndex = match.Index + POSITIONCAST_ID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    string castPos = rawSpeaker.Substring(startIndex, endIndex - startIndex);

                    string[] axis = castPos.Split(AXISDELIMITER, System.StringSplitOptions.RemoveEmptyEntries);

                    float.TryParse(axis[0], out castPosition.x);


                    if (axis.Length > 1)
                    {
                        float.TryParse(axis[1], out castPosition.y);

                    }
                }
                else if (match.Value == EXPRESSIONCAST_ID)
                {
                    startIndex = match.Index + EXPRESSIONCAST_ID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    string castExp = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));

                    //split into array and making each item split from the other 
                    CastExpressions = castExp.Split(EXPRESSIONLAYER_JOINER)
                        .Select(x =>
                        {
                            var parts = x.Trim().Split(EXPRESSIONLAYER_DELIMITER);

                            if (parts.Length == 2)
                            {
                                return (int.Parse(parts[0]), parts[1]);
                            }
                            else return (0, parts[0]);
                        }).ToList();
                }

            }
        }
    }
}