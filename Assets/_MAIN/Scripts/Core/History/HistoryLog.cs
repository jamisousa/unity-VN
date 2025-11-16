using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace History
{

    //a visual log of a previous point in dialogue
    public class HistoryLog 
    {
        public GameObject container;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI dialogueText;
        public float nameFontSize = 0;
        public float dialogueFontSize = 0;
    }
}