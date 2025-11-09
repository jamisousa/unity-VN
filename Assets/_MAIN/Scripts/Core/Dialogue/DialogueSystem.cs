using System.Collections.Generic;
using CHARACTERS;
using UnityEngine;

//central dialogue system to manage conversations and dialogue display

namespace DIALOGUE
{
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField] private DialogueSystemConfigurationSO _config;
        public DialogueSystemConfigurationSO config => _config;

        public DialogueContainer dialogueContainer = new DialogueContainer();

        public ConversationManager conversationManager;

        private TextArchitect architect;

        private AutoReader autoReader;

        public bool isRunningConversation => conversationManager.isRunning;

        public static DialogueSystem instance { get; private set; }

        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserPrompt_Next;

        public DialogueContinuePrompt prompt;

        //used to hide and show the entire dialog layers
        [SerializeField] private CanvasGroup mainCanvas;
        private CanvasGroupController cgController;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
            else
            {
                DestroyImmediate(gameObject);
            }

        }

        bool _initialized = false;
        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            architect = new TextArchitect(dialogueContainer.dialogueText);
            conversationManager = new ConversationManager(architect);

            cgController = new CanvasGroupController(this, mainCanvas);

            dialogueContainer.Initialize();

            if(TryGetComponent(out autoReader))
            {
                autoReader.Initialize(conversationManager);
            }
        }

        public void OnUserPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();

            if(autoReader != null && autoReader.isOn)
            {
                autoReader.Disable();
            }
        }


        public void OnSystemPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();
        }

        public void ApplySpeakerDataToDialogueContainer(string speakerName)
        {
            Character character = CharacterManager.instance.GetCharacter(speakerName);
            CharacterConfigData config = character != null ? character.config : CharacterManager.instance.GetCharacterConfig(speakerName);

            ApplySpeakerDataToDialogueContainer(config);
        }

        public void ApplySpeakerDataToDialogueContainer(CharacterConfigData config)
        {
            dialogueContainer.SetDialogueFont(config.dialogueFont);
            dialogueContainer.SetDialogueColor(config.dialogueColor);
            dialogueContainer.SetDialogueFontSize(config.dialoguefontSize * this.config.dialogueFontScale);
            dialogueContainer.nameContainer.SetNameFont(config.nameFont);
            dialogueContainer.nameContainer.SetNameColor(config.nameColor);
            dialogueContainer.nameContainer.SetNameFontSize(config.namefontSize);
        }


        public void ShowSpeakerName(string speakerName = "")
        {
            if (speakerName.ToLower() != "narrator")
                dialogueContainer.nameContainer.Show(speakerName);

            else HideSpeakerName();
        }

        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();

        public Coroutine Say(string speaker, string dialogue)
        {
            List<string> conversation = new List<string>() { $"{speaker}\"{dialogue}\""};

           return Say(conversation);

        }

        public Coroutine Say(List<string> conversation)
        {
           return conversationManager.StartConversation(conversation);
        }

        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);

        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed, immediate);

        public bool isVisible => cgController.isVisible;

    }

}