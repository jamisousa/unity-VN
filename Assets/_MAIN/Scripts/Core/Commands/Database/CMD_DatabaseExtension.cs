using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{

    //a base class used to extend the available commands in the command database

    public abstract class CMD_DatabaseExtension
    {
        public static void Extend(CommandDatabase database){}

        public static CommandParameters ConvertDataToParameters(string[] data) => new CommandParameters(data);
    }
}