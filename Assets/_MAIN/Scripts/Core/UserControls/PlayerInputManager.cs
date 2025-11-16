using System;
using System.Collections.Generic;
using History;
using UnityEngine;
using UnityEngine.InputSystem;


//capture player input to control dialogue progression

namespace DIALOGUE
{
    public class PlayerInputManager : MonoBehaviour
    {

        private PlayerInput input;
        private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext> command)>();

        private void Awake()
        {
            input = GetComponent<PlayerInput>();

            InitializeActions();
        }

        private void InitializeActions()
        {
            actions.Add((input.actions["Next"], PromptAdvance));
            actions.Add((input.actions["HistoryBack"], OnHistoryBack));
            actions.Add((input.actions["HistoryForward"], OnHistoryForward));

        }

        private void OnEnable()
        {
            foreach (var inputAction in actions){
                inputAction.action.performed += inputAction.command;
            }
        }

        private void OnDisable()
        {
            foreach (var inputAction in actions)
            {
                inputAction.action.performed -= inputAction.command;
            }
        }


        public void PromptAdvance(InputAction.CallbackContext c)
        {
            DialogueSystem.instance.OnUserPrompt_Next();
        }


        public void OnHistoryBack(InputAction.CallbackContext c)
        {
            HistoryManager.instance.GoBack();
        }

        public void OnHistoryForward(InputAction.CallbackContext c)
        {
            HistoryManager.instance.GoForward();
        }

    }

}