using System;
using System.Collections;
using CHARACTERS;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_Characters : CMD_DatabaseExtension
    {
        private static string[] PARAM_ENABLE => new string[] { "-e", "-enable" };
        private static string[] PARAM_IMMEDIATE => new string[] { "-i", "-immediate" };
        private static string[] PARAM_SPEED => new string[] { "-spd", "-speed" };
        private static string[] PARAM_SMOOTH => new string[] { "-sm", "-smooth" };

       
        private static string PARAM_XPOS => "-x";
        private static string PARAM_YPOS => "-y";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("createcharacter", new Action<string[]>(CreateCharacter));
            database.AddCommand("movecharacter", new Func<string[], IEnumerator>(MoveCharacter));

            CommandDatabase baseCommands = CommandManager.instance.CreateSubDatabase(CommandManager.DATABASE_CHARACTERS_BASE);
            baseCommands.AddCommand("move", new Func<string[], IEnumerator>(MoveCharacter));
            baseCommands.AddCommand("show", new Func<string[], IEnumerator>(Show));
            baseCommands.AddCommand("hide", new Func<string[], IEnumerator>(Hide));
            baseCommands.AddCommand("setpriority", new Action<string[]>(SetPriority));
            baseCommands.AddCommand("setposition", new Action<string[]>(SetPosition));
            baseCommands.AddCommand("setColor", new Func<string[], IEnumerator>(SetColor));

            CommandDatabase spriteCommands = CommandManager.instance.CreateSubDatabase(CommandManager.DATABASE_CHARACTERS_SPRITE);
            spriteCommands.AddCommand("setsprite", new Func<string[], IEnumerator>(SetSprite));
            spriteCommands.AddCommand("animate", new Func<string[], IEnumerator>(Animate));
            spriteCommands.AddCommand("stopanimation", new Action<string[]>(StopAnimation));
        }

        private static void CreateCharacter(string[] data)
        {
            string characterName = data[0];
            bool enable = false;
            bool immediate = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_ENABLE, out enable, defaultValue: false);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            Character character = CharacterManager.instance.CreateCharacter(characterName);

            if (!enable)
                return;

            if (immediate)
                character.isVisible = true;
            else
                character.Show();

        }

        private static IEnumerator MoveCharacter(string[] data)
        {
            string characterName = data[0];
            Character character = CharacterManager.instance.GetCharacter(characterName);

            if (character == null)
                yield break;

            float x = 0, y = 0;
            float speed = 1;
            bool smooth = false;
            bool immediate = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_XPOS, out x);

            parameters.TryGetValue(PARAM_YPOS, out y);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1);

            parameters.TryGetValue(PARAM_SMOOTH, out smooth, defaultValue: false);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            Vector2 position = new Vector2(x, y);

            if (immediate)
                character.SetPosition(position);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.SetPosition(position); });
                yield return character.MoveToPosition(position, speed, smooth);
            }
        }

        private static IEnumerator Show(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]);

            if (character == null)
                yield break;

            bool immediate = false;
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);

            if (immediate)
                character.isVisible = true;
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { if (character != null) character.isVisible = true; });

                yield return character.Show();
            }
        }

        private static IEnumerator Hide(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]);

            if (character == null)
                yield break;

            bool immediate = false;
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);

            if (immediate)
                character.isVisible = false;
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { if (character != null) character.isVisible = false; });

                yield return character.Hide();
            }
        }

        public static void SetPosition(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);
            float x = 0, y = 0;

            if (character == null || data.Length < 2)
                return;

            var parameters = ConvertDataToParameters(data, 1);

            parameters.TryGetValue(PARAM_XPOS, out x, defaultValue: 0);
            parameters.TryGetValue(PARAM_YPOS, out y, defaultValue: 0);

            character.SetPosition(new Vector2(x, y));
        }

        public static void SetPriority(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);
            int priority;

            if (character == null || data.Length < 2)
                return;

            if (!int.TryParse(data[1], out priority))
                priority = 0;

            character.SetPriority(priority);
        }

        public static IEnumerator SetColor(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);
            string colorName;
            float speed;
            bool immediate;

            if (character == null || data.Length < 2)
                yield break;

            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(new string[] { "-c", "-color" }, out colorName);
            bool specifiedSpeed = parameters.TryGetValue(new string[] { "-spd", "-speed" }, out speed, defaultValue: 1f);
            if (!specifiedSpeed)
                parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: true);
            else
                immediate = false;

            Color color = Color.white;
            color = color.GetColorFromName(colorName);

            if (immediate)
                character.SetColor(color);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.SetColor(color); });
                character.TransitionColor(color, speed);
            }

            yield break;
        }

        public static IEnumerator SetSprite(string[] data)
        {
            Character_Sprite character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false) as Character_Sprite;
            int layer = 0;
            string spriteName;
            bool immediate = false;
            float speed;

            if (character == null || data.Length < 2)
                yield break;

            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(new string[] { "-s", "-sprite" }, out spriteName);
            parameters.TryGetValue(new string[] { "-l", "-layer" }, out layer, defaultValue: 0);

            bool specifiedSpeed = parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 0.1f);

            if (!specifiedSpeed)
                parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: true);
            Sprite sprite = character.GetSprite(spriteName);

            if (sprite == null)
                yield break;

            if (immediate)
            {
                character.SetSprite(sprite, layer);
            }
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.SetSprite(sprite, layer); });
                yield return character.TransitionSprite(sprite, layer, speed);
            }

        }

        private static IEnumerator Animate(string[] data)
        {
            Character_Sprite character = CharacterManager.instance
                .GetCharacter(data[0], createIfDoesNotExist: false) as Character_Sprite;

            if (character == null)
                yield break;

            string animationName = data[1];

            bool refresh = false;
            var parameters = ConvertDataToParameters(data, startingIndex: 2);

            parameters.TryGetValue(new string[] { "-r", "-refresh" }, out refresh, defaultValue: true);

            character.Animate(animationName, refresh);
        }

        private static void StopAnimation(string[] data)
        {
            Character_Sprite character = CharacterManager.instance
                .GetCharacter(data[0], createIfDoesNotExist: false) as Character_Sprite;

            if (character == null || data.Length < 2)
                return;

            string animationName = data[1];

            bool refresh = false;
            var parameters = ConvertDataToParameters(data, startingIndex: 2);

            parameters.TryGetValue(new string[] { "-r", "-refresh" }, out refresh, defaultValue: false);

            character.StopAnimation(animationName, refresh);
        }

    }
}