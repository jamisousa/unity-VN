using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

public class TestingConversationQueue : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Running()); 
    }

    IEnumerator Running()
    {
        List<string> lines = new List<string>()
        {
            "This is line 1 from the original conversation",
            "This is line 2 from the original conversation",
            "This is line 3 from the original conversation",
        };

        yield return DialogueSystem.instance.Say(lines);

        //DialogueSystem.instance.Hide();

    }

    void Update()
    {
        List<string> lines = new List<string>();
        Conversation conversation = null;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            lines = new List<string>()
            {
                "hi",
                "hello",
                "hey",
            };

            conversation = new Conversation(lines);
            DialogueSystem.instance.conversationManager.Enqueue(conversation);

        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            lines = new List<string>()
            {
                "hiiiiiiiiiii",
                "hellooooooooooo",
                "heyyyyyyyyyyyyyyyyyy",
            };

            conversation = new Conversation(lines);
            DialogueSystem.instance.conversationManager.EnqueuePriority(conversation);

        }
    }


}
