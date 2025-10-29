using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{

    //character that uses sprites or sprite sheets to render its display


    public class Character_Sprite : Character
    {
        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();

        public Character_Sprite(string name, CharacterConfigData config, GameObject prefab) : base(name, config, prefab)
        {
            rootCG.alpha = 0;

            Debug.Log("Created Sprite Character: " + name);
        }

        public override IEnumerator ShowingOrHiding(bool show)
        {
            float targetAlpha = show ? 1f : 0f;

            CanvasGroup self = rootCG;

            while(self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, Time.deltaTime * 3f);
                yield return null;
            }

            co_revealing = null;
            co_hiding = null;
        }
    }

}