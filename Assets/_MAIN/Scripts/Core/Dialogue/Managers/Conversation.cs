using System.Collections.Generic;

namespace DIALOGUE
{

    //data container for a conversation including its lines and progress
    public class Conversation
    {
        private List<string> lines = new List<string>();
        private int progress = 0;

        public Conversation(List<string> lines, int progress = 0)
        {
            this.lines = lines;
            this.progress = progress;
        }

        public int GetProgress() => progress;
        public void SetProcess(int value) => progress = value;
        public void IncrementProgress() => progress++;
        public int Count => lines.Count;
        public List<string> GetLines() => lines;
        public string CurrentLine() => lines[progress];
        public bool HasReachedEnd() => progress >= lines.Count;
    }
}