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
            int valueChange = 0;

            if (data.StartsWith("+") || data.StartsWith("-"))
            {
                if (!int.TryParse(data, out valueChange))
                {
                    UnityEngine.Debug.LogError($"[SetAffinity] Invalid value: {data}");
                    return;
                }

                VNGameSave.activeFile.affinity += valueChange; 
            }
            else
            {
                if (!int.TryParse(data, out valueChange))
                {
                    UnityEngine.Debug.LogError($"[SetAffinity] Invalid value: {data}");
                    return;
                }

                VNGameSave.activeFile.affinity = valueChange;
            }

            if (VNGameSave.activeFile.affinity < 0)
                VNGameSave.activeFile.affinity = 0;

            HeartsManager.instance.SetHearts(VNGameSave.activeFile.affinity);
        }

    }
}
