using System.Collections;

namespace DIALOGUE.LogicalLines
{

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