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
        protected Coroutine co_moving;

        public bool isRevealing => co_revealing != null;
        public bool isHiding => co_hiding != null;

        public bool isMoving => co_moving != null;

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

        //set character position for movement
        public virtual void SetPosition(Vector2 position)
        {
            if(root == null)
            {
                return;
            }

            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPosition(position);

            root.anchorMin = minAnchorTarget;
            root.anchorMax = maxAnchorTarget;
        }

    
        public virtual Coroutine MoveToPosition(Vector2 position, float speed = 2f, bool smooth = false)
        {

            if (root == null)
            {
                return null;
            }

            if (isMoving)
            {
                manager.StopCoroutine(co_moving);
            }

            co_moving = manager.StartCoroutine(MovingToPosition(position, speed, smooth));

            return co_moving;
        }

        private IEnumerator MovingToPosition(Vector2 position, float speed, bool smooth)
        {
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPosition(position);
            Vector2 padding = root.anchorMax - root.anchorMin;

            while(root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget)
            {
               root.anchorMin = smooth ? Vector2.Lerp(root.anchorMin, minAnchorTarget, speed * Time.deltaTime)
                    : Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed * Time.deltaTime * 0.35f);


                root.anchorMax = root.anchorMin + padding;

                if (smooth && Vector2.Distance(root.anchorMin, minAnchorTarget) <= 0.001f)
                {
                    root.anchorMin = minAnchorTarget;
                    root.anchorMax = maxAnchorTarget;
                    break;
                }

                yield return null;
            }

            Debug.Log("Done moving");
            co_moving = null;

        }

        //convert ui target position to relative character anchor targets
        public (Vector2, Vector2) ConvertUITargetPosition(Vector2 position) { 

            Vector2 padding = root.anchorMax - root.anchorMin;

            float maxX = 1f - padding.x;
            float maxY = 1f - padding.y;

            Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
            Vector2 maxAnchorTarget = minAnchorTarget + padding;

            return (minAnchorTarget, maxAnchorTarget);
        }

    }
}