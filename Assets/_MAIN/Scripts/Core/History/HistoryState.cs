using System.Collections.Generic;

namespace History
{

    [System.Serializable]
    public class HistoryState 
    {
        public DialogueData dialogue;
        public List<CharacterData> characters;
        public List<AudioData> audio;
        public List<GraphicData> graphics;
        public HeartsData hearts;

        public static HistoryState Capture()
        {
            HistoryState state = new HistoryState();
            state.dialogue = DialogueData.Capture();
            state.characters = CharacterData.Capture();
            state.audio = AudioData.Capture();
            state.graphics = GraphicData.Capture();
            state.hearts = HeartsData.Capture();

            return state;
        }

        public void Load()
        {
            DialogueData.Apply(dialogue);
            CharacterData.Apply(characters);
            AudioData.Apply(audio);
            GraphicData.Apply(graphics);
            HeartsData.Apply(hearts);
        }
    }
}