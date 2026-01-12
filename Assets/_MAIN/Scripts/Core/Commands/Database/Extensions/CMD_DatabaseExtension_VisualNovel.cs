using System;
using UnityEngine;
using VISUALNOVEL;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_VisualNovel : CMD_DatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("setplayername", new Action<string>(SetPlayerNameVariable));
            database.AddCommand("setmaincharname", new Action<string>(SetMainCharName));
            database.AddCommand("setaffinity", new Action<string>(SetAffinity));

            database.AddCommand("lockcursor", new Action(LockCursor));
            database.AddCommand("unlockcursor", new Action(UnlockCursor));
        }

        private static void SetPlayerNameVariable(string data)
        {
            VNGameSave.activeFile.playerName = data;
        }

        private static void SetMainCharName(string data)
        {
            VNGameSave.activeFile.mainCharGuessedName = data;
        }


        private static void SetAffinity(string data)
        {
            if (VNGameSave.isLoading)
                return;

            int valueChange = 0;

            if (data.StartsWith("+") || data.StartsWith("-"))
            {
                if (!int.TryParse(data, out valueChange))
                {
                    Debug.LogError($"[SetAffinity] Invalid value: {data}");
                    return;
                }

                VNGameSave.activeFile.affinity += valueChange;
            }
            else
            {
                if (!int.TryParse(data, out valueChange))
                {
                    Debug.LogError($"[SetAffinity] Invalid value: {data}");
                    return;
                }

                VNGameSave.activeFile.affinity = valueChange;
            }

            if (VNGameSave.activeFile.affinity < 0)
                VNGameSave.activeFile.affinity = 0;

            HeartsManager.instance.SetHearts(VNGameSave.activeFile.affinity);
        }


        private static void LockCursor()
        {
            //not actually locking since it needs to progress story
            Cursor.visible = false;
        }

        private static void UnlockCursor()
        {
            Cursor.visible = true;
        }
    }
}
