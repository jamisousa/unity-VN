using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_General: CMD_DatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("wait", (System.Func<string, IEnumerator>)(Wait));
        }
        private static IEnumerator Wait(string data)
        {
            if(float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }

        }
    }
}