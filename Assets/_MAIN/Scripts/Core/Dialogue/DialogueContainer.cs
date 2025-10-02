using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    [System.Serializable]
    public class DialogueContainer
    {
        public GameObject root;
        public NameContainer nameContainer = new NameContainer(); 
        public TextMeshProUGUI dialogueText;
    }

}
