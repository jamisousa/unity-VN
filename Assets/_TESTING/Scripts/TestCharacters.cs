using System.Collections;
using UnityEngine;
using CHARACTERS;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {

        private Character CreateCharacter(string characterName) => CharacterManager.instance.CreateCharacter(characterName);

        // Start is called before the first frame update
        void Start()
        {
            //Character Stella2 = CharacterManager.instance.CreateCharacter("Stella");

            //Character Raelin = CharacterManager.instance.CreateCharacter("Raelin");

            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            Character guard1 = CreateCharacter("Guard1 as Generic");
            Character guard2 = CreateCharacter("Guard2 as Generic");
            Character guard3 = CreateCharacter("Guard3 as Generic");

            guard1.Show();
                        yield return new WaitForSeconds(1f);
            guard2.Show();
                        yield return new WaitForSeconds(1f);
            guard3.Show();

            yield return null;
        }



        /*******************************************************
        * Say lines independent from dialogue system and files (optional)
        ********************************************************/
        //IEnumerator Test()
        //{
        //    Character Elen = CharacterManager.instance.CreateCharacter("Elen");
        //    Character Stella = CharacterManager.instance.CreateCharacter("Stella");
        //    Character Ben = CharacterManager.instance.CreateCharacter("Benjamin");


        //    List<string> lines = new List<string>()
        //    {
        //        "Hi",
        //        "This is a line.",
        //    };

        //    //yield return DialogueSystem.instance.Say(lines);

        //    yield return Elen.Say(lines);

        //    Elen.SetNameColor(Color.red);
        //    Elen.SetDialogueColor(Color.yellow);

        //    yield return Stella.Say(lines);

        //    yield return Ben.Say(lines);


        //    Debug.Log("Done");
        //}

        /*******************************************************
         * End Say lines independent from dialogue system and files (optional)
         ********************************************************/

        // Update is called once per frame
        void Update()
        {

        }
    }
}