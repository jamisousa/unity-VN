using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_Audio : CMD_DatabaseExtension
    {
        private static string[] PARAM_SFX = new string[] { "-s", "-sfx" };
        private static string[] PARAM_VOLUME = new string[] { "-v", "-vol", "-volume" };
        private static string[] PARAM_PITCH = new string[] { "-p", "-pitch" };
        private static string[] PARAM_LOOP = new string[] { "-l", "-loop" };

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("playsfx", new Action<string[]>(PlaySFX));
            database.AddCommand("stopsfx", new Action<string>(StopSFX));

        }

        private static void PlaySFX(string[] data) {

            string filepath;
            float volume, pitch;
            bool loop;

            var parameters = ConvertDataToParameters(data);

            //try to get the name or path to the sound effect
            parameters.TryGetValue(PARAM_SFX, out filepath);

            //get volume
            parameters.TryGetValue(PARAM_VOLUME, out volume);

            //pitch
            parameters.TryGetValue(PARAM_PITCH, out pitch);

            //sound loops
            parameters.TryGetValue<bool>(PARAM_LOOP, out loop);

            //run logic
            AudioClip sound = Resources.Load<AudioClip>(FilePaths.GetPathToResource(FilePaths.resources_sfx, filepath));


            if(sound == null)
            {
                return;
            }


            AudioManager.instance.PlaySoundEffect(sound, volume: volume, pitch: pitch, loop: loop);
        }

        private static void StopSFX(string data)
        {

            AudioManager.instance.StopSoundEffect(data);

        }
    }
}