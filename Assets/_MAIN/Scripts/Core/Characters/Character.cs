using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using TMPro;
using UnityEngine;

namespace CHARACTERS
{


    //base class from which all characters derive from

    public abstract class Character
    {
        public string name = "";
        public string displayName = "";
        public RectTransform root = null;
        public CharacterConfigData config;
        public Animator animator;

        protected CharacterManager manager => CharacterManager.instance;
        public DialogueSystem dialogueSystem => DialogueSystem.instance;

        //Coroutines
        protected Coroutine co_revealing, co_hiding;

        public bool isRevealing => co_revealing != null;
        public bool isHiding => co_hiding != null;
        public virtual bool isVisible => false;


        public Character(string name, CharacterConfigData config, GameObject prefab)
        {
            //base constructor
            this.name = name;
            displayName = name;
            this.config = config;

            if (prefab != null)
            {
                GameObject ob = Object.Instantiate(prefab, manager.characterPanel);
                ob.SetActive(true);
                root = ob.GetComponent<RectTransform>();
                animator = root.GetComponentInChildren<Animator>();
            }
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
         * Say lines independent from dialogue system and files (optional)
         ********************************************************/
        public Coroutine Say(string dialogue) => Say(new List<string> { dialogue });

        public Coroutine Say(List<string> dialogue)
        {
            dialogueSystem.ShowSpeakerName(displayName);
            dialogue = FormatDialogue(dialogue);

            //dialogueSystem.ApplySpeakerDataToDialogueContainer(name);

            UpdateTextCustomizationsOnScreen();

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
         * End Say lines independent from dialogue system and files (optional)
         ********************************************************/


        //Real time screen customs
        public void SetNameColor(Color color) => config.nameColor = color;
        public void SetDialogueColor(Color color) => config.dialogueColor = color;

        public void SetNameFont(TMP_FontAsset font) => config.nameFont = font;
        public void SetDialogueFont(TMP_FontAsset font) => config.dialogueFont = font;

        public void UpdateTextCustomizationsOnScreen() => dialogueSystem.ApplySpeakerDataToDialogueContainer(config);

        public void ResetConfigurationData() => config = CharacterManager.instance.GetCharacterConfig(name);

        //reveal characters on screen
        public virtual Coroutine Show()
        {
            if (isRevealing)
            {
                return co_revealing;
            }

            if (isHiding)
            {
                manager.StopCoroutine(co_hiding);
            }

            co_revealing = manager.StartCoroutine(ShowingOrHiding(true));

            return co_revealing;
        }

        public virtual Coroutine Hide()
        {
            if (isHiding)
            {
                return co_hiding;
            }


            if (isRevealing)
            {
                manager.StopCoroutine(co_revealing);
            }

            co_hiding = manager.StartCoroutine(ShowingOrHiding(false));

            return co_hiding;
        }

        public virtual IEnumerator ShowingOrHiding(bool show)
        {
            Debug.Log("Show and hide cannot be called from a base character type.");
            yield return null;
        }

    }
}