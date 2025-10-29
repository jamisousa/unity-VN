using System.Collections;
using System.Collections.Generic;
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
        private const string CHARACTER_CASTING_ID = " as ";

        //paths for prefabs
        private const string CHARACTER_NAME_ID = "<characterName>";
        private string characterRootPath => $"Characters/{CHARACTER_NAME_ID}";
        private string characterPrefabPath => $"{characterRootPath}/Character - [{CHARACTER_NAME_ID}]";

       [SerializeField] private RectTransform _characterPanel =  null;
        public RectTransform characterPanel => _characterPanel;

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

        public Character CreateCharacter(string characterName)
        {
            if(characters.ContainsKey(characterName.ToLower()))
            {
                Debug.LogWarning("Character with name " + characterName + " already exists!");
                return null;
            }

            CHARACTER_INFO info = GetCharacterInfo(characterName);

            Character character = CreateCharacterFromInfo(info);

            characters.Add(characterName.ToLower(), character);

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
                    return new Character_Sprite(info.name, config, info.prefab);
                case Character.CharacterType.Live2D:
                    return new Character_Live2D(info.name, config, info.prefab);
                case Character.CharacterType.Model3D:
                    return new Character_Model3D(info.name, config, info.prefab);
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

            return result;
        }

        private class CHARACTER_INFO
        {
            public string name = "";
            public string castingName = "";

            public CharacterConfigData config = null;

            public GameObject prefab = null;
        }

        private string FormatCharacterPath(string path, string characterName) => path.Replace(CHARACTER_NAME_ID, characterName);

        private GameObject GetPrefabForCharacter(string characterName)
        {
            //use character path in resources folder
            string prefabPath = FormatCharacterPath(characterPrefabPath, characterName);

            Debug.Log("path" + prefabPath + "characterPrefabPath" + characterPrefabPath);

            return Resources.Load<GameObject>(prefabPath);
        }

    }
}