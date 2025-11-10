using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace DIALOGUE.LogicalLines
{

    //show a prompt at the end of dialogue to tell the user when input is expected
    public class LogicalLineManager : MonoBehaviour
    {

        private List<ILogicalLine> logicalLines = new List<ILogicalLine>();
        private DialogueSystem dialogueSystem => DialogueSystem.instance;


        public LogicalLineManager() => LoadLogicalLines();

        private void LoadLogicalLines()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            Type[] lineTypes = assembly.GetTypes()
                .Where(t => typeof(ILogicalLine).IsAssignableFrom(t) && !t.IsInterface)
                .ToArray();

            foreach (Type lineType in lineTypes)
            {
                ILogicalLine line = (ILogicalLine)Activator.CreateInstance(lineType);
                logicalLines.Add(line);
            }
        }

        public bool TryGetLogic(DIALOGUE_LINES line, out Coroutine logic)
        {
            foreach(var logicalLine in logicalLines)
            {
                if (logicalLine.Matches(line))
                {
                    logic = dialogueSystem.StartCoroutine(logicalLine.Execute(line));
                    return true;
                }
            }

            logic = null;
            return false;
        }

    }
}