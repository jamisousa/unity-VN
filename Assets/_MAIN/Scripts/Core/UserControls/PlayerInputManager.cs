using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//capture player input to control dialogue progression

namespace DIALOGUE
{
    public class PlayerInputManager : MonoBehaviour
    {
        void Start()
        {

        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Return))
            {
                PromptAdvance();
            }

        }

        public void PromptAdvance()
        {
            DialogueSystem.instance.OnUserPrompt_Next();
        }
    }

}