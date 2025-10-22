using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//data container representing a single line/entry in a dialogue script.

namespace DIALOGUE
{
    public class DIALOGUE_LINES
    {
        public DL_SPEAKER_DATA speaker;
        public DL_DIALOGUE_DATA dialogue;
        public string commands;

        public bool hasDialogue => speaker != null;
        public bool hasCommands => commands != string.Empty;

        public bool hasSpeaker => speaker.displayName != string.Empty; 

        public DIALOGUE_LINES(string speaker, string dialogue, string commands)
        {
           this.speaker =  (string.IsNullOrWhiteSpace(speaker) ? null: new DL_SPEAKER_DATA(speaker));
           this.dialogue = new DL_DIALOGUE_DATA(dialogue);
           this.commands = commands;
        }
    }

}