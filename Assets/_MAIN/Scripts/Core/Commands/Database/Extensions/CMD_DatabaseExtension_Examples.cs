using System;
using COMMANDS;
using UnityEngine;

namespace TESTING
{

    //examplke for how to add future commands to the command system
    public class CMD_DatabaseExtension_Examples : CMD_DatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            //add command with no parameters
            database.AddCommand("print", new Action(PrintDefaultMesage));
        }

        private static void PrintDefaultMesage()
        {
            Debug.Log("This is a default message from the command database extension example.");
        }
    }
}