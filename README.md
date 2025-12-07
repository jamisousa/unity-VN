# Visual Novel Project

This Unity-based visual novel introduces a mysterious character whose true identity you must uncover. Engage through dialogue choices, explore his personality, and make decisions that shape the story as it unfolds! Will you take the time to get to know him and discover more?

<div align="center">
  <img alt="Unity" height="30" src="https://img.shields.io/badge/unity-%23000000.svg?style=for-the-badge&logo=unity&logoColor=white&color=black">
  <img alt="Unity" height="30" src="https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white&color=black">
</div>

<br>

<div align="center">
  <img width="350" height="350" alt="VNChar3" src="https://github.com/user-attachments/assets/42d8ada9-39ad-427a-87d5-fb44a31bf520" />
  <img width="350" height="350" alt="VNChar" src="https://github.com/user-attachments/assets/a60ef101-4bcb-414f-9cdf-2ef800d27e92" />
</div>

---

## 💻 Game Features

- Interactive dialogue system with branching choices
- Character animations and expressions using sprite layers
- Save/load functionality
- Gallery system: unlock images based on story progression
- History logs: limited history displaying the most recent story text
- Auto reader and text skipping options, with the ability to continue or stop skipping after choices
- Configuration menu: Audio settings: disable music, ambience, SFX, and voice // General settings: resolution, fullscreen/windowed mode, auto read, and text building speed
- Help menu with game controls
- Input system
---

## 🔌 Extra Code Features

- Text building types: typewriter, fade, or instant
- Easily extendable commands to enhance your story
- Tag manager for saving variables during the save and load process

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

## 💗 Acknowledgment

A huge thanks to [Stellar Studio](https://www.youtube.com/@stellarstudio5495) for making an incredible, free [course](https://www.youtube.com/watch?v=cO6NzrvTrkY&list=PLGSox0FgA5B58Ki4t4VqAPDycEpmkBd0i) available to everyone on YouTube. His lessons were essential to creating this project.

---

## 🎮 Playing the Game

The game build is located in the releases page. Simply extract the zip and open the executable VisualNovel.exe to play.

To download and play the latest version of the game, please check [the release page](https://github.com/jamisousa/unity-VN/releases/tag/v1.0)

---

## 📖 Resources Credits

Credits for each image, video and sound used in the game can be found in the file below:
ResourcesCredits.md

Please refer to the [respective file](https://github.com/jamisousa/unity-VN/blob/refactor/cleanup/ResourcesCredits.md) for full attribution details and original sources.

---

## 📢 Important Notice - Cloning This Repository

If you'd like to clone this repository for reference or to create your own project:

- Images and audio files are **not included** due to licensing restrictions.
- The story content and character sprites are **not included** as they are my creations.
- To write and customize your own story, check the following files in Assets/_Main folder: Character Configuration Asset // Dialogue System Configuration // Visual Novel Configuration
- To load character sprites, refer to Assets/_TESTING/SpriteLoaderEditor.cs

---

## 👾 Game Demonstrations


<img width="800" height="607" alt="Image" src="https://github.com/user-attachments/assets/a562a371-a6b0-4aeb-aae5-8ecba1811a73" /> 
<img width="800" height="607" alt="Image" src="https://github.com/user-attachments/assets/4909a4c4-ff7d-4e35-9a73-e998bf6cf27c" /> 
<img width="800" height="607" alt="image" src="https://github.com/user-attachments/assets/38c58a66-f6e8-4d64-8142-bb36c15aa271" /> 
<img width="800" height="607" alt="Image" src="https://github.com/user-attachments/assets/12ff5e0a-d716-4d5d-bb52-f9dfbeec7764" /> 
<img width="800" height="607" alt="Image" src="https://github.com/user-attachments/assets/940d32c8-2fe2-4edd-9662-42276aab24a0" /> 
<img width="800" height="607" alt="Image" src="https://github.com/user-attachments/assets/20e83af7-1afb-4382-afe8-966aa83d702d" /> 
<img width="800" height="607" alt="image" src="https://github.com/user-attachments/assets/f1751285-6cf0-4f6f-996f-918811e348cb" /> 
<img width="800" height="607" alt="image" src="https://github.com/user-attachments/assets/5f82bc72-6692-44e7-8ce8-8b26232fe35f" />
<img width="800" height="607" alt="image" src="https://github.com/user-attachments/assets/5d5c7a4b-3fb1-419e-af5c-5d1b4f7b47d0" />
<img width="800" height="607" alt="image" src="https://github.com/user-attachments/assets/59e7f547-c58f-4f40-9aa1-e36e7b91a7f0" />

