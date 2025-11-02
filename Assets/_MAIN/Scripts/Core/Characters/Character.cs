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
        private const float UNHIGHLIGHTED_DARKEN_STRENGTH = 0.65f;
        public const bool DEFAULT_ORIENTATION_IS_FACING_LEFT = true;
        public const string ANIMATION_REFRESH_TRIGGER = "Refresh";

        public string name = "";
        public string displayName = "";
        public RectTransform root = null;
        public CharacterConfigData config;
        public Animator animator;

        //character color despite highlighting
        public Color color { get; protected set; } = Color.white;

        //color that takes highlighted into account
        protected Color displayColor => highlighted ? highlightedColor : unhighlightedColor;

        protected Color highlightedColor => color;
        protected Color unhighlightedColor => new Color(color.r * UNHIGHLIGHTED_DARKEN_STRENGTH, color.g * UNHIGHLIGHTED_DARKEN_STRENGTH, color.b * UNHIGHLIGHTED_DARKEN_STRENGTH, color.a);
        public bool highlighted { get; protected set; } = true;

        protected bool facingLeft = DEFAULT_ORIENTATION_IS_FACING_LEFT;
        public int priority { get; protected set; }

        protected CharacterManager characterManager => CharacterManager.instance;
        public DialogueSystem dialogueSystem => DialogueSystem.instance;

        //Coroutines
        protected Coroutine co_revealing, co_hiding;
        protected Coroutine co_moving;
        protected Coroutine co_changingColor;
        protected Coroutine co_highlighting;
        protected Coroutine co_flipping;

        public bool isRevealing => co_revealing != null;
        public bool isHiding => co_hiding != null;
        public bool isMoving => co_moving != null;
        public bool isChangingColor => co_changingColor != null;
        public bool isHighlighting => (highlighted && co_highlighting != null);
        public bool isUnhighlighting => (!highlighted && co_highlighting != null);
        public bool isFlipping => co_flipping != null;


        public virtual bool isVisible { get; set; }
        public bool isFacingLeft => facingLeft;
        public bool isFacingRight => !facingLeft;


        public Character(string name, CharacterConfigData config, GameObject prefab)
        {
            //base constructor
            this.name = name;
            displayName = name;
            this.config = config;

            if (prefab != null)
            {
                GameObject ob = Object.Instantiate(prefab, characterManager.characterPanel);
                ob.name = characterManager.FormatCharacterPath(characterManager.characterPrefabNameFormat, name);
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
                characterManager.StopCoroutine(co_hiding);
            }

            co_revealing = characterManager.StartCoroutine(ShowingOrHiding(true));

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
                characterManager.StopCoroutine(co_revealing);
            }

            co_hiding = characterManager.StartCoroutine(ShowingOrHiding(false));

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
            if (root == null)
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
                characterManager.StopCoroutine(co_moving);
            }

            co_moving = characterManager.StartCoroutine(MovingToPosition(position, speed, smooth));

            return co_moving;
        }

        private IEnumerator MovingToPosition(Vector2 position, float speed, bool smooth)
        {
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPosition(position);
            Vector2 padding = root.anchorMax - root.anchorMin;

            while (root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget)
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
        public (Vector2, Vector2) ConvertUITargetPosition(Vector2 position)
        {

            Vector2 padding = root.anchorMax - root.anchorMin;

            float maxX = 1f - padding.x;
            float maxY = 1f - padding.y;

            Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
            Vector2 maxAnchorTarget = minAnchorTarget + padding;

            return (minAnchorTarget, maxAnchorTarget);
        }

        //character color transition - differentiate active speaker
        public virtual void SetColor(Color color)
        {
            this.color = color;
        }

        //fade in color
        public Coroutine TransitionColor(Color color, float speed)
        {
            this.color = color;

            if (isChangingColor)
            {
                characterManager.StopCoroutine(co_changingColor);
            }

            co_changingColor = characterManager.StartCoroutine(ChangingColor(displayColor, speed));

            return co_changingColor;
        }

        public virtual IEnumerator ChangingColor(Color targetColor, float speed)
        {
            Debug.Log("SetColor cannot be called for this character type!");

            yield return null;

        }

        //highlighting character - differentiate active speaker
        public Coroutine Highlight(float speed = 1f)
        {
            if (isHighlighting)
            {
                return co_highlighting;
            }
            if (isUnhighlighting)
            {
                characterManager.StopCoroutine(co_highlighting);
            }

            highlighted = true;
            co_highlighting = characterManager.StartCoroutine(Highlighting(highlighted, speed));
            return co_highlighting;
        }

        public Coroutine Unhighlight(float speed = 1f)
        {
            if (isUnhighlighting)
            {
                return co_highlighting;
            }
            if (isHighlighting)
            {
                characterManager.StopCoroutine(co_highlighting);
            }

            highlighted = false;
            co_highlighting = characterManager.StartCoroutine(Highlighting(highlighted, speed));
            return co_highlighting;
        }

        public virtual IEnumerator Highlighting(bool highlight, float speedMultiplier)
        {
            Debug.Log("Highlighting is not available on this character type.");

            yield return null;
        }

        //character flipping
        public Coroutine Flip(float speed = 1, bool immediate = false)
        {
            if(isFacingLeft){
                return FaceRight();
            }
            return FaceLeft();
        }

        public Coroutine FaceLeft(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
            {
                characterManager.StopCoroutine(co_flipping);
            }

            facingLeft = true;
            co_flipping = characterManager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));
            return co_flipping;
        }

        public Coroutine FaceRight(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
            {
                characterManager.StopCoroutine(co_flipping);
            }
            facingLeft = false;
            co_flipping = characterManager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));
            return co_flipping;
        }

        public virtual IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            Debug.Log("Face direction is not available on this character type.");
            yield return null;
        }

        //layer priorities
        public void SetPriority(int priority, bool autoSortCharactersOnUi = true)
        {
            if (autoSortCharactersOnUi)
            {
                characterManager.SortCharacters();
            }

            this.priority = priority;
            root.SetSiblingIndex(priority);
        }

        //animations
        public void Animate(string animation)
        {
            animator.SetTrigger(animation);
        }

        public void Animate(string animation, bool state)
        {
            animator.SetBool(animation, state);
            animator.SetTrigger(ANIMATION_REFRESH_TRIGGER);
        }

    }
}