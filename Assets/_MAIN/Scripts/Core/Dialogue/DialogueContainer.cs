using TMPro;
using UnityEngine;

//the container that holds all dialogue-related UI elements
namespace DIALOGUE
{
    [System.Serializable]
    public class DialogueContainer
    {
        public GameObject root;
        public NameContainer nameContainer = new NameContainer(); 
        public TextMeshProUGUI dialogueText;

        public void SetDialogueColor(Color color)  => dialogueText.color = color;
        public void SetDialogueFont(TMP_FontAsset font) => dialogueText.font = font;

        public void SetDialogueFontSize(float size) => dialogueText.fontSize = size;
    }

}
