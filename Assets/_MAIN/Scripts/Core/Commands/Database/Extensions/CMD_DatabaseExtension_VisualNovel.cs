using System;
using VISUALNOVEL;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_VisualNovel : CMD_DatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("setplayername", new Action<string>(SetPlayerNameVariable));
            database.AddCommand("setaffinity", new Action<string>(SetAffinity));
        }

        private static void SetPlayerNameVariable(string data)
        {
            VNGameSave.activeFile.playerName = data;
        }

        private static void SetAffinity(string data)
        {
            if (!int.TryParse(data, out int value))
            {
                UnityEngine.Debug.LogError($"[SetAffinity] Invalid value: {data}");
                return;
            }

            VNGameSave.activeFile.affinity = value;

            HeartsManager.instance.SetHearts(value);
        }
    }
}
