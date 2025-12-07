using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using TMPro;
using UnityEngine;

namespace CHARACTERS
{
    public abstract class Character
    {
        private const float UNHIGHLIGHTED_DARKEN_STRENGTH = 0.65f;
        public const bool DEFAULT_ORIENTATION_IS_FACING_LEFT = true;
        public const string ANIMATION_REFRESH_TRIGGER = "Refresh";

        public string name = "";
        public string displayName = "";
        public string castingName = "";
        public RectTransform root = null;
        public CharacterConfigData config;
        public Animator animator;

        public Color color { get; protected set; } = Color.white;

        protected Color displayColor => highlighted ? highlightedColor : unhighlightedColor;

        protected Color highlightedColor => color;
        protected Color unhighlightedColor => new Color(color.r * UNHIGHLIGHTED_DARKEN_STRENGTH, color.g * UNHIGHLIGHTED_DARKEN_STRENGTH, color.b * UNHIGHLIGHTED_DARKEN_STRENGTH, color.a);
        public bool highlighted { get; protected set; } = true;

        protected bool facingLeft = DEFAULT_ORIENTATION_IS_FACING_LEFT;
        public int priority { get; protected set; }

        public Vector2 targetPosition { get; private set; }

        protected CharacterManager characterManager => CharacterManager.instance;
        public DialogueSystem dialogueSystem => DialogueSystem.instance;

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

        public enum CharacterType
        {
            Text,
            Sprite,
            SpriteSheet,
            Live2D,
            Model3D
        }

        public Coroutine Say(string dialogue) => Say(new List<string> { dialogue });

        public Coroutine Say(List<string> dialogue)
        {
            dialogueSystem.ShowSpeakerName(displayName);
            dialogue = FormatDialogue(dialogue);


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

        public void SetNameColor(Color color) => config.nameColor = color;
        public void SetDialogueColor(Color color) => config.dialogueColor = color;

        public void SetNameFont(TMP_FontAsset font) => config.nameFont = font;
        public void SetDialogueFont(TMP_FontAsset font) => config.dialogueFont = font;

        public void UpdateTextCustomizationsOnScreen() => dialogueSystem.ApplySpeakerDataToDialogueContainer(config);

        public void ResetConfigurationData() => config = CharacterManager.instance.GetCharacterConfig(name);

        public virtual Coroutine Show(float speedMultiplier = 1f)
        {
            if (isRevealing)
            {
                characterManager.StopCoroutine(co_revealing);
            }

            if (isHiding)
            {
                characterManager.StopCoroutine(co_hiding);
            }

            co_revealing = characterManager.StartCoroutine(ShowingOrHiding(true, speedMultiplier));

            return co_revealing;
        }

        public virtual Coroutine Hide(float speedMultiplier = 1f)
        {
            if (isHiding)
            {
                characterManager.StopCoroutine(co_hiding);
            }


            if (isRevealing)
            {
                characterManager.StopCoroutine(co_revealing);
            }

            co_hiding = characterManager.StartCoroutine(ShowingOrHiding(false, speedMultiplier));

            return co_hiding;
        }

        public virtual IEnumerator ShowingOrHiding(bool show, float speedMultiplier = 1f)
        {
            yield return null;
        }

        public virtual void SetPosition(Vector2 position)
        {
            if (root == null)
            {
                return;
            }

            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPosition(position);

            root.anchorMin = minAnchorTarget;
            root.anchorMax = maxAnchorTarget;

            targetPosition = position;
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

            targetPosition = position;

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
            co_moving = null;

        }

        public (Vector2, Vector2) ConvertUITargetPosition(Vector2 position)
        {

            Vector2 padding = root.anchorMax - root.anchorMin;

            float maxX = 1f - padding.x;
            float maxY = 1f - padding.y;

            Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
            Vector2 maxAnchorTarget = minAnchorTarget + padding;

            return (minAnchorTarget, maxAnchorTarget);
        }

        public virtual void SetColor(Color color)
        {
            this.color = color;
        }

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
            yield return null;
        }

        public Coroutine Highlight(float speed = 1f, bool immediate = false)
        {
            if (isHighlighting || isUnhighlighting)
            {
                characterManager.StopCoroutine(co_highlighting);
            }

            highlighted = true;
            co_highlighting = characterManager.StartCoroutine(Highlighting(speed, immediate));
            return co_highlighting;
        }

        public Coroutine Unhighlight(float speed = 1f, bool immediate = false)
        {
            if (isUnhighlighting || isHighlighting)
            {
                characterManager.StopCoroutine(co_highlighting);
            }
 
            highlighted = false;
            co_highlighting = characterManager.StartCoroutine(Highlighting(speed, immediate));
            return co_highlighting;
        }

        public virtual IEnumerator Highlighting(float speedMultiplier, bool immediate = false)
        {
            yield return null;
        }

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
                characterManager.StopCoroutine(co_flipping);

            facingLeft = true;
            co_flipping = characterManager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));

            return co_flipping;
        }

        public Coroutine FaceRight(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                characterManager.StopCoroutine(co_flipping);

            facingLeft = false;
            co_flipping = characterManager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));

            return co_flipping;
        }

        public virtual IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            yield return null;
        }

        public void SetPriority(int priority, bool autoSortCharactersOnUi = true)
        {
            if (autoSortCharactersOnUi)
            {
                characterManager.SortCharacters();
            }

            this.priority = priority;
            root.SetSiblingIndex(priority);
        }
        public void Animate(string animation)
        {
            animator.SetTrigger(animation);
        }
        
        public void Animate(string animation, bool state)
        {
            animator.SetBool(animation, state);
            animator.SetTrigger(ANIMATION_REFRESH_TRIGGER);
        }

        public void StopAnimation(string animation, bool state)
        {
            animator.SetBool(animation, false);
            animator.SetTrigger(ANIMATION_REFRESH_TRIGGER);
        }
        public virtual void OnReceiveCastingExpression(int layer, string expression)
        {
            return;
        }

    }
}