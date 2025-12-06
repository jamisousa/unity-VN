# Visual Novel Project

This Unity-based visual novel introduces a mysterious character whose true identity you must uncover. Engage through dialogue choices, explore his personality, and make decisions that shape the story as it unfolds. Will you take the time to get to know him and discover more?

<div align="center">
  <img alt="Unity" height="30" src="https://img.shields.io/badge/unity-%23000000.svg?style=for-the-badge&logo=unity&logoColor=white&color=black">
  <img alt="Unity" height="30" src="https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white&color=black">
</div>

---

##  Game Features

- Interactive dialogue system with branching choices
- Character animations and expressions using sprite layers
- Save/load functionality
- Gallery system: unlock images based on story progression
- History logs: limited history displaying the most recent story text
- Auto reader and text skipping options, with the ability to continue or stop skipping after choices
- Configuration menu:
    - Audio settings: disable music, ambience, SFX, and voice
    - General settings: resolution, fullscreen/windowed mode, auto read, and text building speed
- Help menu with game controls

---

## Extra Code Features

- Text building types: typewriter, fade, or instant
- Easily extendable commands to enhance your story

### General Commands
- Wait → waits for a command to finish before advancing to the next line of dialogue
- ShowUI / HideUI → show or hide the overall visual novel UI
- ShowDialogueBox / HideDialogueBox → show or hide the dialogue box
- Load → loads a new dialogue file (useful for story branches). Reads any .txt file, allowing the visual novel to load other .txt files from the Resources folder

### Character Commands
- CreateCharacter → creates a character based on a CharacterConfigSO file
- MoveCharacter → moves a character to a specified position
- Show / Hide → show or hide characters
- SetPriority → display a character on top of another
- SetPosition → change the character’s position
- SetColor / SetSprite → change character appearance
- Animate / StopAnimation → run or stop animations
    - Current animations: Hop and Shiver

### Audio Commands
- PlaySFX / StopSFX
- PlaySong / StopSong
- PlayAmbience / StopAmbience

### Graphic Panels
- SetLayerMedia → change layers of visual novel images, including background, foreground, cinematic, and others
- ClearLayerMedia → clear media displayed on chosen layer

### Visual Novel Commands
- SetPlayerName → sets the player name based on the input on screen
- SetAffinity → sets affinity with the main character and updates hearts on screen

### Gallery Commands
- ShowGalleryImage → shows gallery image and unlocks it in the gallery menu
- HideGalleryImage → hides gallery image after being displayed

> Developers can refer to CMD_DatabaseExtension_Examples to create additional commands.

---

## Acknowledgment

A huge thanks to [Stellar Studio](https://www.youtube.com/@stellarstudio5495) for making an incredible, free [course](https://www.youtube.com/watch?v=cO6NzrvTrkY&list=PLGSox0FgA5B58Ki4t4VqAPDycEpmkBd0i) available to everyone on YouTube. His lessons were essential to creating this project.

---

## Playing the Game

The game build is located in the Build folder. Simply open the executable to play.

// TODO: add folder path

---

## Resources Credits

Credits for each image, video and sound used in the game can be found in the file below:
ResourcesCredits.md

Please refer to the [respective file](https://github.com/jamisousa/unity-VN/blob/refactor/cleanup/ResourcesCredits.md) for full attribution details and original sources.

---

## Important Notice - Cloning This Repository

If you'd like to clone this repository for reference or to create your own project:

- Images and audio files are **not included** due to licensing restrictions.
- The story content and character sprites are **not included** as they are my creations.
- To write and customize your own story, check the following files in Assets/_Main folder:
    - Character Configuration Asset
    - Dialogue System Configuration
    - Visual Novel Configuration
- To load character sprites, refer to Assets/_TESTING/SpriteLoaderEditor.cs

---

## Game Demonstrations

//TODO: add game demo
