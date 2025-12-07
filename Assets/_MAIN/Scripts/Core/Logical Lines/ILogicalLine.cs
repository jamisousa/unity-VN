using System.Collections;

namespace DIALOGUE.LogicalLines
{

    public interface ILogicalLine 
    {
        string keyword { get; }
        bool Matches(DIALOGUE_LINES line);
        IEnumerator Execute(DIALOGUE_LINES line);

    }
}