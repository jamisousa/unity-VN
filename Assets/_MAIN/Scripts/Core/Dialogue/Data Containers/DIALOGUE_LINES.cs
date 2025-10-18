using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//data container representing a single line/entry in a dialogue script.

namespace DIALOGUE
{
    public class DIALOGUE_LINES
    {
        public string speaker;
        public DL_DIALOGUE_DATA dialogue;
        public string commands;

        public bool hasDialogue => dialogue.hasDialogue;
        public bool hasCommands => commands != string.Empty;

        public bool hasSpeaker => speaker != string.Empty;

        public DIALOGUE_LINES(string speaker, string dialogue, string commands)
        {
           this.speaker = speaker;
           this.dialogue = new DL_DIALOGUE_DATA(dialogue);
           this.commands = commands;
        }
    }

}