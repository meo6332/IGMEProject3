# Project Humans Vs Zombies Part 2

### Student Info

-   Name: Max O'Malley
-   Section: 05S2

### Description

This project fully implements the requirements given for the humans VS zombies project, creating a small interactive game based on the requirements. 
In this game, the player plays as a cop that is immune to the zombie virus who must protect a number of humans from an endless horde of zombies, with the objective being to keep
the humans alive for as long as possible while killing the zombies that will chase after and infect them.  The game plays like a twin stick shooter,
with the player able to move in 8 directions and shoot in any direction.  

### User Responsibilities

-   In this game the user controls the cop character, and can move around using the WASD keys, allowing the player to move in any of 8 directions.  The player
    is able to shoot their gun automatically by holding in any of 8 directions using the arrow keys.  Holding in a given direction will make the player shoot repeatedly in that
    direction.
-   The player needs to keep the humans alive as long as possible.  The humans will wander around when not near zombies, and will run away from zombies if they come
    close.  Do be aware that the humans will turn into zombies if they are caught by any zombies.
-   The zombies can be destroyed by the player by shooting them, and will die in one shot.  The player is also uneffected by the zombies, due to being immume, 
    and as such the player is neither harmed by the zombies, or chased by them.  However, the zombies will come in endless waves, and as such need to be held off as
    long as possible.
-   The game ends whenever all of the humans are infected.  The player will then need to restart the application to play again. 

### Known Issues

-   There is a known bug where, in the first few seconds where the player tries to use the arrow keys to shoot, they won't shoot anything.  After a few seconds this
    automatically resolves itself and allows the player to shoot in any direction again, but know that you need to try to fire for those first few seconds 
    for this to work.
-   There is no collision detection for the player, so nothing will stop the player from moving through trees or from going out of bounds.  
-   None of the characters have any animations beyond facing their direction of movement.
-   The timer will roll over to 0 whenever it reaches 60.

### Requirements not completed

-   All the requirements for the project are complete

### Design Details and Implementation Decisions

-   During the second version for HvZ I had to make a number of decisions related to design to make my twin stick shooter idea work properly.  I don't have a ton
    to say regarding every decision with it, but I do want to make a few notes about what I decided, that I will order according to what part of the project those
    design decisions relate to the most.
    
    Player
-   While attempting to implement the player into the project, for simplicity's sake and to be able to make it easiest to move them, I implemented the player
    without using forces given I thought it would be more easy to understand and control.  This also helped lead into my idea of protecting humans, as it allowed
    for me to keep the humans present, while giving the player a means to interact with them and the zombies.
-   I decided to make the game a twin stick shooter using the arrow keys to shoot, given that I recognized that it would be diffcult to properly handle the position
    of the mouse in 3d space.  Given the entire game is also effectively along a plane on the x-z axis as well, meant that you would only really need to shoot along
    that axis. That led me to realize that just using the arrow keys for shooting would be relatively natural and still be perfectly functional for what a twin stick
    shooter needs.
    
    Environment
-   I ended up adding a few trees around the center of the scene and the edges to help break up the zombies that spawn and to offer the humans some amount of initial
    protection.  It gives both of the agents a chance to move around in more interesting patterns, and prevenets the humans or zombies from bunching up.  It also can give
    the player extra time to kill zombies, given that they'll be slower around the trees whenever they're trying to get close to humans.
    
    Zombies
-   Whenever it came to spawning waves of zombies, I decided to put a limit upon how many spawn at once.  They'll spawn rapidly, which can put a lot of pressure on the player,
    so I prevented too many zombies from spawning to make certain that the player would be capable of dealing with them and be able to protect the humans.  I also didn't
    want too many zombies to be spawned at once, as given the player is not extremely fast, it means that they could be more likely to end up in a situation where they can't 
    protect all of the humans through no fault of their own besides there just being too many zombies.  The amount present now means that, while things are still intense, 
    the player should be able to more reasonably defend the humans and focus on the ones that have many zombies chasing them. 
-   I also made it so that the zombies have a limited range of being able to chase humans to help aleviate the player not being able to defend all the humans at once.
    This makes it so that the player can have some chance to see what humans might have more zombies coming after them soon, and move to kill those zombies and prevent this.
    It also makes it less likely that multiple humans will be pursued by multiple zombies at once, helping the player to be able to deal with major threats one at a time as
    they appear, rather than being forced to multitask in a way that's likely to make them fail. 
    
    Humans
-   The humans were made faster than the zombies to make it so that they wouldn't be likely to be caught by a single zombie unless they're cornered.  This was done this way
    to give the player ample time to save a human from a zombie, and to make singular zombies less dangerous to humans.  I wanted it to be the case that humans were only
    truly at risk if they were cornered, or being chased by multiple zombies.  Making the humans faster than the zombies make it so that they can prevent themselves from
    being infected for a brief time even without assistance from the player.  This gives the player time to see what humans are in danger, and move to help save them by
    killing any zombies that put them at risk.

### Sources

-   Human, Zombie, and Player Models: https://assetstore.unity.com/packages/3d/characters/toony-tiny-people-demo-113188
-   Tree Model: https://assetstore.unity.com/packages/3d/vegetation/trees/free-trees-103208
-   Background Building Models: https://assetstore.unity.com/packages/3d/environments/urban/russian-buildings-lowpoly-pack-80518
-   The Floor and Bullet models were both made using Unity's in engine assets.

- The below models are in the assets of the project but unused, but I still wanted to ensure they were credited
- Zombie Assets: https://assetstore.unity.com/packages/3d/characters/humanoids/zombie-30232
- Robot Hero Models: https://assetstore.unity.com/packages/3d/characters/robots/robot-hero-pbr-hp-polyart-106154
