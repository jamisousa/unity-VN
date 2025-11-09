using System.Collections;
using System.Collections.Generic;
using CHARACTERS;
using COMMANDS;
using UnityEngine;

//manages the flow of a conversation, including dialogue display and command execution

namespace DIALOGUE
{
    public class ConversationManager
    {
        private Coroutine process = null;
        public bool isRunning => process != null;
        private DialogueSystem dialogueSystem => DialogueSystem.instance;
        public TextArchitect architect = null;
        private bool userPrompt = false;

        private TagManager tagManager;
        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;
            tagManager = new TagManager();
        }

        private void OnUserPrompt_Next()
        {
            userPrompt = true;
        }

        public Coroutine StartConversation(List<string> conversation)
        {
            StopConversation();

            process = dialogueSystem.StartCoroutine(RunningConversation(conversation));

            return process;
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
                if (line.hasDialogue)
                {
                    yield return Line_RunDialogue(line);
                }

                //run commands
                if (line.hasCommands) yield return Line_RunCommands(line);

                //wait for user input if dialogue was in this line
                if (line.hasDialogue)
                {
                    yield return WaitForUserInput();
                    CommandManager.instance.StopAllProcesses();
                }

            }
        }

        IEnumerator Line_RunDialogue(DIALOGUE_LINES line)
        {
            //Show or hide the speaker name if there is one present.
            if (line.hasSpeaker)
                HandleSpeakerLogic(line.speakerData);

            //If the dialogue box is not visible - make sure it becomes visible automatically
            if (!dialogueSystem.dialogueContainer.isVisible)
                dialogueSystem.dialogueContainer.Show();

            //Build dialogue
            yield return BuildLineSegments(line.dialogueData);
        }


        private void HandleSpeakerLogic(DL_SPEAKER_DATA speakerData)
        {
            bool characterMustBeCreated = (speakerData.makeCharacterEnter || speakerData.isCastingPosition || speakerData.isCastingExpressions);

            Character character = CharacterManager.instance.GetCharacter(speakerData.name, createIfDoesNotExist: characterMustBeCreated);

            if (speakerData.makeCharacterEnter && (!character.isVisible && !character.isRevealing))
                character.Show();

            //Add character name to the UI
            dialogueSystem.ShowSpeakerName(tagManager.Inject(speakerData.displayName));

            //Now customize the dialogue for this character - if applicable
            DialogueSystem.instance.ApplySpeakerDataToDialogueContainer(speakerData.name);

            //Cast position
            if (speakerData.isCastingPosition)
                character.MoveToPosition(speakerData.castPosition);

            //Cast Expression
            if (speakerData.isCastingExpressions)
            {
                foreach (var ce in speakerData.CastExpressions)
                    character.OnReceiveCastingExpression(ce.layer, ce.expression);
            }
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

        public bool isWaitingOnAutoTimer { get; private set; } = false;

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
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    break;
                default:
                    break;

            }
        }

        IEnumerator Line_RunCommands(DIALOGUE_LINES line)
        {
            List<DL_COMMAND_DATA.Command> commands = line.commandData.commands;

            foreach(DL_COMMAND_DATA.Command command in commands)
            {
                if (command.waitForCompletion || command.name == "wait")
                {
                    CoroutineWrapper cw = CommandManager.instance.Execute(command.name, command.arguments);
                    while (!cw.IsDone)
                    {
                        if (userPrompt)
                        {
                            CommandManager.instance.StopCurrentProcess();
                            userPrompt = false;
                        }
                        yield return null;
                    }
                }
                else
                {
                    CommandManager.instance.Execute(command.name, command.arguments);
                }
            }

            Debug.Log(line.commandData);
            yield return null;
        }

        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {

                dialogue = tagManager.Inject(dialogue);

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

            dialogueSystem.prompt.Show();

            while (!userPrompt)
            {
                yield return null;
            }

            dialogueSystem.prompt.Hide();

            userPrompt = false;
        }
    }
}
