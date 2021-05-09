# Crossy Road
[Hours spent: 15]

## About
This is a game similar to [Crossy Road](https://play.google.com/store/apps/details?id=com.yodo1.crossyroad&hl=en&gl=US) and [Frogger](https://en.wikipedia.org/wiki/Frogger) made for an internship application.

It is made using **Unity Game Engine version 2020.3.7f1 (LTS)** with *Universal Render Pipeline* (URP).

Most 3D assets come from [kenney.nl](https://kenney.nl/)

Main colors:
- #DE8155 - orange
- #3E99DE - blue 
- #DE4D49 - red
- #6ADE91 - green
- #DEAB5F - light orange


## Tasks

### Scenes
- [X] Main menu 
  - [X] 2 buttons:
    - [X] Play button which leads to Game scene
    - [X] Button that toggles music and sound effects in whole game
    - [x] (optional) Quit button
  - [X] Background music
- [ ] Game
  - [X] Player
  - [X] Environment
  - [X] Obstacles
  - [X] Collectables
  - [ ] UI
  - [ ] SFX

### Player
- [X] Controls: up-down arrow to move forward and backward by 1 step (move can be manifested as a small jump), and left-right arrow to move player along left and right axes.
- [X] Can collect collectables (only a number increases upon collection)
- [X] Can die if hit by obstacle or touched by water
- [X] Has death particle system
- [X] Has movement particle system


### Environment
- [X] It is realised in lanes. Each lane is a long horizontal line on which player can jump and move. There are 3 types of lanes: 
  - [X] Safe lane - it can only contain trees or unmovable objects
  - [X] Road - it has running vehicles on it
  - [X] Water - kills player, but it has moving logs on which player can jump
- [X] There is no requirement to other features of the environment, like particles, clouds, bushes, animals - feel free to add what you like, or to add nothing more than specified :)


### Obstacles
- [X] There are 2 types of obstacles:
  - [X] Vehicles 
    - [X] Can kill player on hit
    - [X] Spawn randomly
    - [X] Move across the lines
    - [X] Have smoke particle systems
    - [X] Have vehicle SFX
  - [X] Water 
    - [X] Can kill player on hit
    - [X] Has wooden logs on it that player can jump on to cross the water

### Collectables
- [X] Randomly spawned coins that are collected on trigger enter

### UI
- [X] Collectable counter 
- [X] Steps counter (increases every time player moves one lane forward) 
- [X] Death screen
  - [X] Contains “Press any key to restart”, and when the player presses any key the game should restart. Key pressing should not be registered for 3 seconds after death, so that the player has time to read his stats before resetting.
  - [X] Contains number of collectables.
  - [X] Contains a number of steps - step is each player’s movement forward
  - [X] Contains previous maximum number of steps (high score)

### SFX
- [X] Player move sound
- [X] Player death sound 
- [X] Vehicle movement ambient sound

### Other
- [X] Saving data to local storage and loading data from local storage. This data should contain: 
  - [X] Total number of collectables (collectables are always loaded and each collected adds to this number, so it never resets to 0)
  - [X] High score of steps

