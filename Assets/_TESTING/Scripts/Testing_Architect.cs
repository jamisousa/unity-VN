using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class Testing_Architect : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architect;

        public TextArchitect.BuildMethod buildMethod = TextArchitect.BuildMethod.instant;

        string[] lines = new string[5]
        {
            "This is the first randomized line of text.",
            "This is the second randomized line of text.",
            "This is the third randomized line of text.",
            "This is the fourth randomized line of text.",
            "This is the fifth randomized line of text."
        };

        // Start is called before the first frame update
        void Start()
        {
            ds = DialogueSystem.instance;
            architect = new TextArchitect(ds.dialogueContainer.dialogueText);
            architect.buildMethod = TextArchitect.BuildMethod.fade;
            architect.speed = 2f;
        }

        // Update is called once per frame
        void Update()
        {
            if(buildMethod != architect.buildMethod)
            {
                architect.buildMethod = buildMethod;
                architect.StopBuilding();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
              architect.StopBuilding();
            }

            string longLine = "This is a very long line of text that is meant to test how the text architect handles longer strings of text. It should be able to manage the text without any issues, ensuring that everything displays correctly and in a readable manner.";   
           
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (architect.isBuilding)
                {
                    if(!architect.hurryText)
                    {
                        architect.hurryText = true;
                    }
                    else
                    {
                        architect.ForceComplete();
                    }
                }
                else
                {
                    //architect.Build(lines[Random.Range(0, lines.Length)]);
                    architect.Build(longLine);

                }

            }
            else if(Input.GetKeyDown(KeyCode.A))
            {
                //architect.Append(lines[Random.Range(0, lines.Length)]);
                architect.Append(longLine);

            }
        }
    }

}
