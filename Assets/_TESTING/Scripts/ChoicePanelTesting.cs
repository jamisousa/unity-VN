using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoicePanelTesting : MonoBehaviour
{
    ChoicePanel panel;

    void Start()
    {
        panel = ChoicePanel.instance;
        StartCoroutine(Running());
    }

    IEnumerator Running()
    {
        string[] choices = new string[]
        {
            "Hello",
            "Hi",
            "Wassup"
        };

        panel.Show("Hey there!", choices);

        while (panel.isWaitingOnUserChoice)
        {
            yield return null;
        }

        var decision = panel.lastDecision;

        Debug.Log($"Made choice {decision.answerIndex} {decision.choices[decision.answerIndex]}");
    }

}
