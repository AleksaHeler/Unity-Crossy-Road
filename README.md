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
  - [ ] Player
  - [ ] Environment
  - [ ] Obstacles
  - [ ] Collectables
  - [ ] UI
  - [ ] SFX

### Player
- [ ] Controls: up-down arrow to move forward and backward by 1 step (move can be manifested as a small jump), and left-right arrow to move player along left and right axes.
- [ ] Can collect collectables (only a number increases upon collection)
- [ ] Can die if hit by obstacle or touched by water
- [ ] Has death particle system
- [ ] Has movement particle system


### Environment
- [ ] It is realised in lanes. Each lane is a long horizontal line on which player can jump and move. There are 3 types of lanes: 
  - [ ] Safe lane - it can only contain trees or unmovable objects
  - [ ] Road - it has running vehicles on it
  - [ ] Water - kills player, but it has moving longs on which player can jump
- [ ] There is no requirement to other features of the environment, like particles, clouds, bushes, animals - feel free to add what you like, or to add nothing more than specified :)


### Obstacles
- [ ] There are 2 types of obstacles:
  - [ ] Vehicles 
    - [ ] Can kill player on hit
    - [ ] Spawn randomly
    - [ ] Move across the lines
    - [ ] Have smoke particle systems
    - [ ] Have vehicle SFX
  - [ ] Water 
    - [ ] Can kill player on hit
    - [ ] Has wooden logs on it that player can jump on to cross the water

### Collectables
- [ ] Randomly spawned coins that are collected on trigger enter

### UI
- [ ] Collectable counter 
- [ ] Steps counter (increases every time player moves one lane forward) 
- [ ] Death screen
  - [ ] Contains “Press any key to restart”, and when the player presses any key the game should restart. Key pressing should not be registered for 3 seconds after death, so that the player has time to read his stats before resetting.
  - [ ] Contains number of collectables.
  - [ ] Contains a number of steps - step is each player’s movement forward
  - [ ] Contains previous maximum number of steps (high score)

### SFX
- [ ] Player move sound
- [ ] Player death sound 
- [ ] Vehicle movement ambient sound

### Other
- [ ] Saving data to local storage and loading data from local storage. This data should contain: 
  - [ ] Total number of collectables (collectables are always loaded and each collected adds to this number, so it never resets to 0)
  - [ ] High score of steps

