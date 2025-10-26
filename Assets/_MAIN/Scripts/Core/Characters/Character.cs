using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

namespace CHARACTERS
{


    //base class from which all characters derive from

    public abstract class Character
    {
        public string name = "";
        public string displayName = "";
        public RectTransform root = null;
        
        public DialogueSystem dialogueSystem => DialogueSystem.instance;

        public Character(string name)
        {
            //base constructor
            this.name = name;
            displayName = name;
        }

        //supported character types
        public enum CharacterType
        {
            Text,
            Sprite,
            SpriteSheet,
            Live2D,
            Model3D
        }


        /*******************************************************
         * Say lines independent from dialogue system and files
         ********************************************************/
        public Coroutine Say(string dialogue) => Say(new List<string> { dialogue });

        public Coroutine Say(List<string> dialogue)
        {
            dialogueSystem.ShowSpeakerName(displayName);
            dialogue = FormatDialogue(dialogue);
            return dialogueSystem.Say(dialogue);
        }

        private List<string> FormatDialogue(List<string> dialogue)

        {
            List<string> formattedDialogue = new List<string>();

            foreach (string line in dialogue)

            {

                formattedDialogue.Add($"{displayName} \"{line}\"");

            }

            return formattedDialogue;
        }

        /*******************************************************
         * End Say lines independent from dialogue system and files
         ********************************************************/


    }
}