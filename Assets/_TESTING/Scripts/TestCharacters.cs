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
        * Say lines independent from dialogue system and files (optional)
        ********************************************************/
        IEnumerator Test()
        {
            Character Elen = CharacterManager.instance.CreateCharacter("Elen");
            Character Stella = CharacterManager.instance.CreateCharacter("Stella");
            Character Ben = CharacterManager.instance.CreateCharacter("Benjamin");


            List<string> lines = new List<string>()
            {
                "Hi",
                "This is a line.",
            };

            //yield return DialogueSystem.instance.Say(lines);

            yield return Elen.Say(lines);

            Elen.SetNameColor(Color.red);
            Elen.SetDialogueColor(Color.yellow);

            yield return Stella.Say(lines);

            yield return Ben.Say(lines);


            Debug.Log("Done");
        }

        /*******************************************************
         * End Say lines independent from dialogue system and files (optional)
         ********************************************************/

        // Update is called once per frame
        void Update()
        {

        }
    }
}