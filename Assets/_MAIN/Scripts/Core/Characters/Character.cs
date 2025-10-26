using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{


    //base class from which all characters derive from

    public abstract class Character
    {
        public string name = "";
        public RectTransform root = null;

        public Character(string name)
        {
            //base constructor
            this.name = name;
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
    }
}