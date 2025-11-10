using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE.LogicalLines
{

    //the logical line for accepting user input into the VN
    public class LL_Input : ILogicalLine
    {
        public string keyword => "input";

        public IEnumerator Execute(DIALOGUE_LINES line)
        {
            string title = line.dialogueData.rawData;
            InputPanel panel = InputPanel.instance;
            panel.Show(title);

            while (panel.isWaitingOnUserInput)
            {
                yield return null;
            }
        }

        public bool Matches(DIALOGUE_LINES line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }
    }
}