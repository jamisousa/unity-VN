using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //Character Stella2 = CharacterManager.instance.CreateCharacter("Stella");

            StartCoroutine(Test());
        }


        /*******************************************************
        * Say lines independent from dialogue system and files
        ********************************************************/
        IEnumerator Test()
        {
            Character Elen = CharacterManager.instance.CreateCharacter("Elen");
            Character Stella = CharacterManager.instance.CreateCharacter("Stella");
            Character Adam = CharacterManager.instance.CreateCharacter("Adam");


            List<string> lines = new List<string>()
            {
                "Hi",
                "This is a line.",
            };

            //yield return DialogueSystem.instance.Say(lines);

            yield return Elen.Say(lines);

            yield return Stella.Say(lines);


            Debug.Log("Done");
        }

        /*******************************************************
         * End Say lines independent from dialogue system and files
         ********************************************************/

        // Update is called once per frame
        void Update()
        {

        }
    }
}