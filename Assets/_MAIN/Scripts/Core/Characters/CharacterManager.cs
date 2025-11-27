using System.Collections.Generic;
using System.Linq;
using DIALOGUE;
using UnityEngine;

namespace CHARACTERS
{

    //central hub for creating, retrieving and managing characters in a scene

    public class CharacterManager : MonoBehaviour
    {

        public static CharacterManager instance { get; private set; }
        private Dictionary<string, Character> characters = new Dictionary<string, Character>();

        private CharacterConfigSO config => DialogueSystem.instance.config.characterConfigurationAsset;

        //config casting for more than one character with the same properties (eg. generic chars)
        //Example: CreateCharacter(Guard as Generic) -> "Guard" will use the config for "Generic"
        public const string CHARACTER_CASTING_ID = " as ";

        //paths for prefabs
        private const string CHARACTER_NAME_ID = "<characterName>";
        public string characterRootPathFormat => $"Characters/{CHARACTER_NAME_ID}";

        public string characterPrefabNameFormat => $"Character - [{CHARACTER_NAME_ID}]";
 
        public string characterPrefabPathFormat => $"{characterRootPathFormat}/{characterPrefabNameFormat}";

        public bool HasCharacter(string characterName) => characters.ContainsKey(characterName.ToLower());


        [SerializeField] private RectTransform _characterPanel =  null;
        public RectTransform characterPanel => _characterPanel;
        public Character[] allCharacters => characters.Values.ToArray();


        private void Awake()
        {
            instance = this;
        }

        public CharacterConfigData GetCharacterConfig(string characterName)
        {
           return config.GetConfig(characterName);
        }



        public Character GetCharacter(string characterName, bool createIfDoesNotExist = false)
        {
            if (characters.ContainsKey(characterName.ToLower()))
            {
                return characters[characterName.ToLower()];
            }else if (createIfDoesNotExist)
            {
                return CreateCharacter(characterName);
            }

            return null;
        }

        public Character CreateCharacter(string characterName, bool revealAfterCreation = false)
        {
            if(characters.ContainsKey(characterName.ToLower()))
            {
                Debug.LogWarning("Character with name " + characterName + " already exists!");
                return null;
            }

            CHARACTER_INFO info = GetCharacterInfo(characterName);

            Character character = CreateCharacterFromInfo(info);

            if(info.castingName != info.name)
            {
              character.castingName = info.castingName;
            }

            characters.Add(info.name.ToLower(), character);

            if (revealAfterCreation)
            {
                character.Show();
            }

            return character;
        }

        private Character CreateCharacterFromInfo(CHARACTER_INFO info)
        {
            CharacterConfigData config = info.config;

            switch (config.characterType)
            {
                case Character.CharacterType.Text:
                    return new Character_Text(info.name, config);
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    return new Character_Sprite(info.name, config, info.prefab, info.rootCharacterFolder);
                case Character.CharacterType.Live2D:
                    return new Character_Live2D(info.name, config, info.prefab, info.rootCharacterFolder);
                case Character.CharacterType.Model3D:
                    return new Character_Model3D(info.name, config, info.prefab, info.rootCharacterFolder);
                default:
                    return null;
            }
        }


        private CHARACTER_INFO GetCharacterInfo(string characterName)
        {
            CHARACTER_INFO result = new CHARACTER_INFO();

            string[] nameData = characterName.Split(new string[] { CHARACTER_CASTING_ID }, System.StringSplitOptions.RemoveEmptyEntries);

            result.name = nameData[0];

            result.castingName = nameData.Length > 1 ? nameData[1] : result.name;

            Debug.Log("casting name" + result.castingName);

            result.config = config.GetConfig(result.castingName);

            result.prefab = GetPrefabForCharacter(result.castingName);

            result.rootCharacterFolder = FormatCharacterPath(characterRootPathFormat, result.castingName);

            return result;
        }

        private class CHARACTER_INFO
        {
            public string name = "";
            public string castingName = "";

            public string rootCharacterFolder = "";

            public CharacterConfigData config = null;

            public GameObject prefab = null;
        }

        public string FormatCharacterPath(string path, string characterName) => path.Replace(CHARACTER_NAME_ID, characterName);

        private GameObject GetPrefabForCharacter(string characterName)
        {
            //use character path in resources folder
            string prefabPath = FormatCharacterPath(characterPrefabPathFormat, characterName);

            Debug.Log("path" + prefabPath + "characterPrefabPath" + characterPrefabPathFormat);

            return Resources.Load<GameObject>(prefabPath);
        }

        /*
         * below functions wil sort characters in the UI hierarchy based on priority and visibility
         * can be called without parameters (auto-sorting by priority) or with a list of names (manual sorting)
         * ensures active characters appear on top and the visual order matches their defined priority
        */

        public void SortCharacters()
        {

            /*
             * sorts all characters in the scene based on their visibility and priority
             * - collects all active and visible characters
             * - separates inactive ones to keep their positions unchanged
             * - sorts active characters by their priority value (lowest to highest)
             * - reapplies the sorted order to the UI hierarchy so that characters render in the correct visual order
             */


            List<Character> activeCharacters = characters.Values.Where(c=> c.root.gameObject.activeInHierarchy && c.isVisible).ToList();
            List<Character> inactiveCharacters = characters.Values.Except(activeCharacters).ToList();

            activeCharacters.Sort((a,b) => a.priority.CompareTo(b.priority));

            activeCharacters.Concat(inactiveCharacters);

            SortCharacters(activeCharacters);

        }

        private void SortCharacters(List<Character> charactersSortingOrder)
        {

            /*
             * applies the given sorting order to the UI hierarchy
             * iterates through the provided character list and updates each character's sibling index,
             * changing their render order in the scene to match the specified sequence
             */

            int i = 0;

            foreach(Character character in charactersSortingOrder)
            {
                character.root.SetSiblingIndex(i++);
            }
        }


        public void SortCharacters(string[] characterNames)
        {

            /*
             *  manually sorts characters based on a specified list of names:
             *  retrieves the characters matching the given names and filters out any that don't exist;
             *  keeps the remaining characters ordered by their current priority;
             *  reverses the provided list to ensure the last name appears on top in the UI;
             *  reassigns priorities to the specified characters so they appear above the others;
             *  merges all characters (remaining + reordered) and applies the final sorting to the UI hierarchy!
             */


            List<Character> sortedCharacters = new List<Character>();

            sortedCharacters = characterNames
                .Select(name => GetCharacter(name))
                .Where(c => c != null)
                .ToList();


            List<Character> remainingCharacters = characters.Values.Except(sortedCharacters).OrderBy(character => character.priority).ToList();

            sortedCharacters.Reverse();


            int startingPriority = remainingCharacters.Count > 0 ? remainingCharacters.Max(c=> c.priority) : 0;

            for(int i = 0; i < sortedCharacters.Count; i++)
            {
                Character character = sortedCharacters[i];
                character.SetPriority(startingPriority + i + 1, autoSortCharactersOnUi: false);
            }


            List<Character> allCharacters = remainingCharacters.Concat(sortedCharacters).ToList();
            SortCharacters(allCharacters);
        }

    }
}