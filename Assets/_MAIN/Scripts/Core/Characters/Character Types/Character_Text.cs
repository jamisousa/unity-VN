using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{

    //character with no graphical art, text operations only
    public class Character_Text : Character
    {
        //whenever we call this constructor we also call the base constructor
        public Character_Text(string name) : base(name)
        {
            Debug.Log("Created Text Character: " + name);
        }
    }
}