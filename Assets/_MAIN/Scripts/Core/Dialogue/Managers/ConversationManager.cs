using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//manages the flow of a conversation, including dialogue display and command execution

namespace DIALOGUE
{
    public class ConversationManager
    {
        private Coroutine process = null;
        public bool isRunning => process != null;
        private DialogueSystem dialogueSystem => DialogueSystem.instance;
        private TextArchitect architect = null;
        private bool userPrompt = false;

        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;
        }

        private void OnUserPrompt_Next()
        {
            userPrompt = true;
        }

        public void StartConversation(List<string> conversation)
        {
            StopConversation();

            process = dialogueSystem.StartCoroutine(RunningConversation(conversation));
        }

        public void StopConversation()
        {
            if (!isRunning)
            {
                return;
            }

            dialogueSystem.StopCoroutine(process);
            process = null;

        }

        IEnumerator RunningConversation(List<string> conversation)
        {
            Debug.Log("Entire conversation" + conversation);


            for (int i = 0; i < conversation.Count; i++)
            {
                //Skip empty lines
                if (string.IsNullOrWhiteSpace(conversation[i])) continue;

                DIALOGUE_LINES line = DialogueParser.Parse(conversation[i]);

                //Show dialogue
                if (line.hasDialogue) yield return Line_RunDialogue(line);

                //run commands
                if (line.hasCommands) yield return Line_RunCommands(line);

            }
        }

        IEnumerator Line_RunDialogue(DIALOGUE_LINES line)
        {
            //show or hide the speaker name
            if (line.hasSpeaker)
            {
                dialogueSystem.ShowSpeakerName(line.speakerData.displayName);
            }
            else
            {  
                dialogueSystem.HideSpeakerName();
            }


            yield return BuildLineSegments(line.dialogueData);

            //wait for user input
            yield return WaitForUserInput();
        }


        IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line)
        {
            for (int i = 0; i < line.segments.Count; i++)
            {
                DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment = line.segments[i];

                yield return WaitForSegmentSignalToBeTriggered(segment);

                yield return BuildDialogue(segment.dialogue, segment.appendText);
            }
        }

        IEnumerator WaitForSegmentSignalToBeTriggered(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment)
        {

            switch (segment.startSignal)
            {
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.C:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WA:
                    yield return new WaitForSeconds(segment.signalDelay);
                    break;
                default:
                    break;

            }
        }

        IEnumerator Line_RunCommands(DIALOGUE_LINES line)
        {
            Debug.Log(line.commandData);
            yield return null;
        }

        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {

                if (!append)
                {
                    //Build dialogue
                    architect.Build(dialogue);
                }
                else
                {
                    architect.Append(dialogue);
                }


                while (architect.isBuilding)
                {
                    if (userPrompt)
                    {
                        if (!architect.hurryText)
                        {
                            architect.hurryText = true;
                        }
                        else
                        {
                            architect.ForceComplete();

                        }

                        userPrompt = false;
                    }
                    yield return null;
                }
        }

        IEnumerator WaitForUserInput()
        {
            while (!userPrompt)
            {
                yield return null;
            }

            userPrompt = false;
        }
    }
}
