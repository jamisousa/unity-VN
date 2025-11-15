using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CHARACTERS
{

    //character that uses sprites or sprite sheets to render its display


    public class Character_Sprite : Character
    {

        public override bool isVisible
            {
                get{return isRevealing || rootCG.alpha == 1;}
                set { rootCG.alpha = value ? 1 : 0; }
        }

        private const string SPRITE_RENDERED_PARENT_NAME = "Renderers";
        private const string SPRITESHEET_DEFAULT_SHEET_NAME = "Default";
        private const char SPRITESHEET_TEXT_SPRITE_DELIMITER = '-';


        public List<CharacterSpriteLayer> layers = new List<CharacterSpriteLayer>();

        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();

        private string artAssetsDirectory = "";

        public Character_Sprite(string name, CharacterConfigData config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab)
        {
            rootCG.alpha = 0;

            artAssetsDirectory = rootAssetsFolder + "/Images";

            GetLayers();

            Debug.Log("Created Sprite Character: " + name);
        }

        //evaluate prefab structure and get layers accordingly
        private void GetLayers()
        {
            Transform rendererRoot = animator.transform.Find(SPRITE_RENDERED_PARENT_NAME);

            if(rendererRoot == null)
            {
                Debug.LogError("Character Sprite prefab is missing Renderers parent object.");
                return;
            }

            for(int i = 0; i < rendererRoot.transform.childCount; i++)
            {
                //find each child of the root object
                Transform child = rendererRoot.GetChild(i);

                Image rendererImage = child.GetComponentInChildren<Image>();

                if (rendererImage != null)
                {
                    CharacterSpriteLayer layer = new CharacterSpriteLayer(rendererImage, i);
                    layers.Add(layer);
                    child.name = $"Layer: {i}";
                }
            }
        }

        public void SetSprite(Sprite sprite, int layer = 0)
        {
            layers[layer].SetSprite(sprite);
        }

      public Sprite GetSprite(string spriteName)
        {
            //this block use the serialized dictionary style
            Debug.Log("sprites count on config" + config.sprites.Count);
            if(config.sprites.Count > 0)
            {
                if(config.sprites.TryGetValue(spriteName, out Sprite sprite))
                {
                    return sprite;
                }
            }

            if(config.characterType == CharacterType.SpriteSheet)
            {
                string[] data = spriteName.Split(SPRITESHEET_TEXT_SPRITE_DELIMITER);
                Sprite[] spriteArray = new Sprite[0];

                if(data.Length == 2)
                {
                    string textureName = data[0];
                    spriteName = data[1];
                    spriteArray = Resources.LoadAll<Sprite>($"{artAssetsDirectory}/{textureName}");
                }
                else
                {
                    spriteArray = Resources.LoadAll<Sprite>($"{artAssetsDirectory}/{SPRITESHEET_DEFAULT_SHEET_NAME}");
                }

                if (spriteArray.Length == 0)
                {
                    Debug.LogError($"Sprite Sheet {SPRITESHEET_DEFAULT_SHEET_NAME} not found in {artAssetsDirectory}");
                }

                return Array.Find(spriteArray, sprite => sprite.name == spriteName);
            }
            else
            {
                return Resources.Load<Sprite>($"{artAssetsDirectory}/{spriteName}");
            }
        }

        //gradually change an image through the layers
        public Coroutine TransitionSprite(Sprite sprite, int layer = 0, float speed = 1)
        {
            CharacterSpriteLayer spriteLayer = layers[layer];

            return spriteLayer.TransitionSprite(sprite, speed);
        }

        public override IEnumerator ShowingOrHiding(bool show, float speedMultiplier = 1f)
        {
            float targetAlpha = show ? 1f : 0f;

            CanvasGroup self = rootCG;

            while(self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime * speedMultiplier);
                yield return null;
            }

            co_revealing = null;
            co_hiding = null;
        }

        //override color for transition logic
        public override void SetColor(Color color)
        {
            base.SetColor(color);
            color = displayColor;

            foreach (CharacterSpriteLayer layer in layers)
            {
                layer.StopChangingColor();
                layer.SetColor(color);
            }
        }

        //override changing color coroutine to specific sprite type behavior
        public override IEnumerator ChangingColor(Color color, float speed)
        {
            foreach(CharacterSpriteLayer layer in layers)
            {
                layer.TransitionColor(color, speed);
            }

            yield return null;

            while(layers.Any(layer => layer.isChangingColor))
            {
                yield return null;
            }

            co_changingColor = null;
        }

        //override the coroutine for highlighting for specific sprite type behavior
        public override IEnumerator Highlighting(bool highlight, float speedMultiplier)
        {
            Color targetColor = displayColor;

            foreach (CharacterSpriteLayer layer in layers)
            {
                layer.TransitionColor(targetColor, speedMultiplier);
            }

            yield return null;


            while (layers.Any(layer => layer.isChangingColor))
            {
                yield return null;
            }

            co_highlighting = null;

        }

        //override face direction logic for sprite characters
        public override IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            foreach(CharacterSpriteLayer layer in layers)
            {
                if (faceLeft)
                {
                    layer.FaceLeft(speedMultiplier, immediate);
                }
                layer.FaceRight(speedMultiplier, immediate);
            }

            yield return null;

            while(layers.Any(layer => layer.isFlipping))
            {
                yield return null;
            }   
        }

        public override void OnReceiveCastingExpression(int layer, string expression)
        {
            Sprite sprite = GetSprite(expression);

            if (sprite == null)
            {
                Debug.LogWarning("Sprite could not be found for character");
                return;
            }

            TransitionSprite(sprite, layer);
        }
    }

}