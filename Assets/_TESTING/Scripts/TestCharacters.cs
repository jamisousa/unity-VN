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

            Character_Sprite Raelin = CreateCharacter("Raelin") as Character_Sprite;
            Character_Sprite Guard = CreateCharacter("Guard as Generic") as Character_Sprite;

            Sprite RaelinBodySprite = Raelin.GetSprite("Raelin_1");
            Sprite RaelinFaceSprite = Raelin.GetSprite("Raelin_10");
            
            Sprite RaelinSecondaryBodySprite = Raelin.GetSprite("Raelin_2");
            Sprite RaelinSecondaryFaceSprite = Raelin.GetSprite("Raelin_23");

            Raelin.Show();

            yield return new WaitForSeconds(1);

            Guard.Show();

            yield return new WaitForSeconds(1);

            Raelin.Animate("Hop");

            CharacterManager.instance.SortCharacters(new string[]
            {
                "Raelin", 
                "Guard as Generic" 
            });

            yield return new WaitForSeconds(1);


            CharacterManager.instance.SortCharacters();

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