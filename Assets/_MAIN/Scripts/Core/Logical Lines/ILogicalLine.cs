using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE.LogicalLines
{

    //interface for all logical lines
    public interface ILogicalLine 
    {
        string keyword { get; }
        bool Matches(DIALOGUE_LINES line);
        IEnumerator Execute(DIALOGUE_LINES line);

    }
}