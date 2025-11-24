using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Conditions;
using UnityEditor.Rendering;

namespace DIALOGUE.LogicalLines
{

    //logical line to evaluate conditions and execute lines of dialogue
    public class LL_Condition : ILogicalLine
    {
        public string keyword => "if";
        public const string ELSE = "else";
        private readonly string[] CONTAINERS = new string[] { "(", ")"};

        public IEnumerator Execute(DIALOGUE_LINES line)
        {

            string rawCondition = ExtractCondition(line.rawData.Trim());
            bool conditionResult = EvaluateCondition(rawCondition);

            Conversation currentConversation = DialogueSystem.instance.conversationManager.conversation;
            int currentProgress = DialogueSystem.instance.conversationManager.conversationProgress;

            EncapsulatedData ifData = RipEncapsulationData(currentConversation, currentProgress, ripHeaderAndEncapsulators: false, parentStartingIndex: currentConversation.fileStartIndex);
            EncapsulatedData elseData = new EncapsulatedData();

            if (ifData.endingIndex + 1 < currentConversation.Count)
            {
                string nextLine = currentConversation.GetLines()[ifData.endingIndex+1].Trim();
                if(nextLine == ELSE)
                {
                    elseData = RipEncapsulationData(currentConversation, ifData.endingIndex + 1, ripHeaderAndEncapsulators: false, parentStartingIndex: currentConversation.fileStartIndex);
                    ifData.endingIndex = elseData.endingIndex;
                }
            }

            currentConversation.SetProgress(elseData.isNull ? ifData.endingIndex : elseData.endingIndex);
            EncapsulatedData selData = conditionResult ? ifData: elseData;

            if (!selData.isNull && selData.lines.Count > 0)
            {
                //remove the header and endcapsulator lines from the conversation indexes
                selData.startingIndex += 2; //remove header and starting encapsulator
                selData.endingIndex -= 1;   //remove ending encapsulator

                Conversation newConversation = new Conversation(selData.lines, file: currentConversation.file, fileStartIndex: selData.startingIndex, fileEndIndex: selData.endingIndex);
                DialogueSystem.instance.conversationManager.conversation.SetProgress(selData.endingIndex);
                DialogueSystem.instance.conversationManager.EnqueuePriority(newConversation);
            }

            yield return null;
        }

        public bool Matches(DIALOGUE_LINES line)
        {
            return line.rawData.Trim().StartsWith(keyword);

        }

        private string ExtractCondition(string line)
        {
            int startIndex = line.IndexOf(CONTAINERS[0]) + 1;

            int endIndex = line.IndexOf(CONTAINERS[1]);

            return line.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}