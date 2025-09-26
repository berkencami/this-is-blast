# this-is-blast

![ScreenShot](https://github.com/berkencami/this-is-blast/blob/main/Gameplay/Gameplay-gif.gif)

Project Overview
"This Is Blast" is a color-matching puzzle game developed in Unity, where players strategically use colored shooters to eliminate matching blocks. The objective is to clear all blocks from the board by matching shooter colors with target block colors.

Technical Architecture & Design Choices
Event-Driven Architecture
I implemented a centralized EventManager system to decouple game components and ensure clean communication between systems. This approach makes the codebase more maintainable and allows for easy feature additions without tight coupling.

Async/Await Pattern with UniTask
Used UniTask for all asynchronous operations (animations, shooting sequences, UI transitions). This provides better performance than traditional coroutines and cleaner, more readable async code.

Custom Level Editor
The project includes a custom level editor (LevelEditor.cs) that allows designers to create and modify levels directly within Unity. This tool enables rapid level prototyping and iteration, making it easy to test different block arrangements and shooter configurations without touching code.

![ScreenShot](https://github.com/berkencami/this-is-blast/blob/main/Gameplay/editor1.png)
![ScreenShot](https://github.com/berkencami/this-is-blast/blob/main/Gameplay/editor2.png)
