using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PatchNoteMenu : MonoBehaviour {

    public Button backButton;
    // Start is called before the first frame update
    public GameObject defaultUI;

    public TMP_Text patchNotes;

    private void BackButtonEvent() {
        defaultUI.SetActive(true);
        this.gameObject.SetActive(false);
    }

    void Start() {
        backButton.onClick.AddListener(BackButtonEvent);
        patchNotes.text = @"
v0.8.1 (Started 9/13/2025):

Changes:
-Updated stats menu
-Updated achievements menu
-Updates relics menu
-Removed aspect ratio as an option on the graphics menu
-Changed some lighting on Cabin

Additions:
-Credits on main menu
-Added a spot on the reporting UI that shows how much a report will cost
-Added some additional stats
-Two new divergence types, Extra Object and Glitch
-New look for Cabin

Bugfixes:
-Modifiers should no longer work in the tutorial
-The time in level stat should now correctly be tracked when a player uses the escape menu to exit the level
	-Some stats will still be lost however if the player quits the application through other means (ALT+F4, task manager, crash, etc.)
-Fixed an issue on Cabin where Zombies wouldn't teleport the player upon contact
-Fixed random teleporting modifier
-Fixed steam achievements not unlocking (again)
-Creatures should no longer spawn in a non intended room
-Fixed an invisible Puncture orb on Cabin
-Fixed some items not moving at all when a divergence activates


v0.8.0.1:

Bugfixes:
-Cabin level timer no longer set to 60 seconds
-Puncture Collapse now correctly starts when all rooms have a divergence, and not when all rooms have a creature.



v0.8.0 (Started 8/4/2025, Finished 9/10/2025) - The Replayability Update:

Changes:
-Changed the logic of choosing the room a creature will spawn in. The spawn will no longer fail if a room with a creature in it is chosen.
-Creatures now refund 3 less energy when reported.
-Updated the modifier menu
-Removed the function where lights would dim if the level was maxed out on divergences.

Additions:
-Added the Creature Overrun modifier. This will cause the creature spawn chance to be 100%.
-Added the Speed Boost modifier
-Added a short intro sequence when starting a level. Pause and unpause to skip it.
-Added the No Warnings modifier
-Added the Darkness modifier
-Added the Random Telport modifier
-Added a patch notes section to the main menu
-Added a warning to the graveyard level

Bugfixes:
-Fixed an issue where divergences around for longer increased score instead of decreased.
-Fixed an issue where ambience noises were too loud
-Fixed teleporting not working 100% of the time
-Fixed a bug where the game over screen would not show the correct text
-Fixed a bug where achievements would not unlock at all
-Toilet divergence on cabin had no type



v0.7.3 (Started 8/1/2025, Finished 8/3/2025):

Changes:
-The flashlight is now more powerful, but covers a smaller section of the screen. 
-The flashlight will no longer drain power, but cause it to generate slower. Current causes power to generate at 95% speed.
-The Hider is now just a floating head instead of a whole body.
-Updated Graveyard map.

Additions:
-Added a physical flashlight. Only appears when flashlight is turned on.
-Added some additional ambient sounds to Cabin.
-Added more divergences to Cabin.
-Added a modifier system. This can be used to add certain effects onto a level that make it harder in exchange for more score.
-Added a scoring system. Score is based on divergences reported, creatures reported, and the length of time divergences were active in the level.


Bug Fixes:
-The Zombie now correctly teleports the player upon contact
-The flashlight no longer drains energy while the game is paused.



v0.7.2 (Dec 2024):

Changes:
-Float values in bar sliders are now clamped to the first decimal
-Updated the visuals for The Puncture
-The chance for the endgame collapse to start even if not at max divergences has been dramatically decreased

Additions:
-The reporting UI now becomes 'scrambled' when certain events occur (end game collapse, hider, lurker), making it impossible to report for a duration
-Added a small chance for the endgame collapse to not start even if at max divergences

Bug Fixes:
-Fixed issue where user would not be able to hear or change settings on first launch
-Fixed an unreportable divergence in the Cellar of Cabin
-Fixed an issue where being seen by a Hider would prevent reporting for the rest of the level
-Fixed an issue where the Eyes Peeled Achievement would trigger on the tutorial.
-The Hider is now affected by volume options
-If you report a Hider while it looks at you, it no longer permanently scrambles your reporting UI
-Achievements earned before steam integration should be unlocked on steam once finishing a level

Known Issues:
-You can change menus while typing in a new profile name



v0.7.1 (Aug 18, 2024) - Endgame Rework/Finished Settings/Added Achievements

Changes:
-Instead of an Ender spawning and killing the player, an unreportable expanding sphere spawns in each room when the player is meant to lose.
-Zombies no longer kill the player, they instead teleport the player to a random room
-Door blockers no longer kill the player, they teleport the player to a random room
-Removed Ender
-The Puncture sphere divergence now has a different texture
-Updated the UI of dropdowns and selections on the main menu
-Updated UI of the play screen and settings screen
-Updated some areas of the Graveyard, and added the reporting UI map
-The endgame collapse spheres now expand slightly faster, and for a longer time (there were safe spots on larger maps)

Additions:
-Added a particle effect and delay to expanding spheres when they spawn, so the player has some warning and doesn't instantly walk into it
-Added the Hider to Cabin
-Resolution and display mode settings are now working
-Each setting now has a description that describes what it controls
-The user can now change the quality settings of the game. The lowest setting nearly doubles FPS.
-More volume settings have been added
-Added Steam achievements

Bug Fixes:
-Fixed an issue where certain checkboxes had a weird hitbox
-The compendium menu now displays the creatures in the correct order, and changed the description for the Zombie
-Fixed some more weird UI issues on lower resolutions
-Fixed an issue where the objects in replacement divergences were randomly appearing in other parts of the map



v0.7.0 (Aug 5, 2024) - Cabin Revamp/Audio Revamp:
Changes: 
-Completely reworked the Cabin map.
-Major changes include removal of Storage, addition of Cellar Stairs and Cellar, and map was made generally smaller.
-Completely reworked audio scripts to make them more consistent
-Changed the sounds for reports
-Updated the preview images for each level

Additions:
-Added many new divergences to Cabin
-Added a new type of Divergence that can manifest in multiple ways
-Added a system where certain divergences can be made to only appear on certain difficulties
-A sound effect now plays when selecting rooms/types in the report menu
-Added some additional reminders to the tutorial
-A prompt will appear when hovering over relics in addition to their normal outline
-A new sound effect plays when a report is pending
-Keybind menu is now functional (for most buttons)
-Added a system for switching profiles. This will allow the player to reset stats/achievements/relics and certain settings (keybinds)

Bug Fixes:
-Fixed an issue where creatures could spawn in the wrong room on Cabin
-Creature and divergence audio should now pause and play correctly
-Creature sound effects now follow the creature correctly, instead of staying at the location where it first played
-Audio divergences now stop playing when deactivated after a report
-When a creature is reported, its audio correctly stops playing
-Fixed an issue where items in the background of the main menu could be interacted with



v0.6.3 (Jul 27, 2024):

Changes:
-Some divergences in the Apartment have changed
-If the player pauses while the report menu is open, it is now closed. Same for popups in the tutorial
-The flashlight now takes 30% less energy per second to use
-Normal difficulty was made slightly easier (divergence rate + creature spawn rate increased by 1 second)
-The menu/level selection for Levels, Options, and Extras are now all clickable instead of only being navigatable with Q and E

Additions:
-Enders now have a higher chance of spawning early if you have divergences that have been activated for a long time. This chance goes higher the longer the divergence has been activated (90s, 150s, 200s)
-A small particle effect now plays when a divergence is deactivated
-Added rain effects and sounds to the Apartment map
-Footstep sounds now sound faster if the player is sprinting
-Added a new carpet walking sound for Cabin
-Added warning indicator/sound effect depending on how many divergences are activated - by default they only play once
-Added a status indicator to the report menu. Gives the player some information based on how many divergences are activated
-Enabled the 'Relics' menu. Relics can now be found in-game by interacting with objects that have a white outline when looked at

Bug Fixes:
-Fixed a bug where certain spawn points would be real world colliders



v0.6.2 (Jul 21, 2024) - Creature Refactor/Animations

Changes:
-Changed how animations are controlled for each creature
-Changed the walking and running animation for the Zombie
-Made the Apartment slightly brighter
-Made disorienting effect on the Chaser stronger and apply over time
-Removed the ability to jump
-Updated the in-game UI
-Changed the position of the battery icon to the top right

Additions:
-Added a new model for Chasers
-Created fixed positions for creature spawns on certain maps where the terrain was too small/jagged for consistent creature spawns
-Added a new footstep sound for Graveyard
-Added a new disorienting effect for the Lurker
-The report button now changes color if you cannot send a report

Bug Fixes:
-Creature spawning on Apartment fixed due to fixed spawn positions being added
-Fixed an issue causing creatures to spawn incorrect (creature would warp before navmeshagent variable was initialized, causing creature to go to (0,0,0))
-Fixed an issue where creatures could track through walls on Apartment
-Fixed an issue where creatures would cost too much energy
-Can no longer use flashlight or crouch while the reporting menu is open



v0.6.1 (Jul 18, 2024) - Refactor

Changes:
-Changed how pausing works in the backend to give other parts of the UI the ability to pause the game in the same way
-Changed debug menu in the backend to be easier to add to
-Rewrote all code for divergences
-Made the duration of the battery icon turning red shorter
-Readded the Apartment level
-Made Normal and Easy slightly easier to account for the new creature effects added
-Lowered special Creature (Chaser, Lurker, Hider) spawn chances by half
-There is now a slight delay when an audio divergence is activated before the sound plays

Additions:
-Chasers now create a disorienting effect in addition to their normal slowing effect
-Enders now have a small chance to spawn even when each room does not have an anomaly. The chance is higher when you are closer to the maximum amount of anomalies.
-Apartment now shows up in the background of the main menu
-Lurkers now show up on Apartment
-A chromatic abberation effect is now applied when you go over a certain amount of divergences

Bug Fixes:
-Fixed a hamper divergence spinning way too fast in Cabin bathroom
-Fixed an issue with objects that moved where they could end up in the wrong position when reported
-Fixed an issue where Lurkers would not spawn on Cabin
-Fixed a bug where the stats page would not show more than 9 stats
-Fixed an issue with creature spawns on Apartment

Known Issues:
-File saving doesn't work on certain machines (windows 11?)
-Apartment creature spawning no worky



v0.6 (Started 6/28/24, finished 7/6) - UI Overhaul

Changes:
-Main menu UI overhauled
-Made loading screen a little nicer
-Remade stat tracking, again. Now allows for any stat to be added without much extra work
-Flashlight now follows where the player is looking
-The battery icon that represents energy now turns red when you lose a significant amount
-All data saved on file is now encrypted
-Remade certain in game screens (Game over, pause, reporting UI)

Additions:
-Added the ability to disable or enable visual hints (lights flickering)
-Added brightness settings
-Added audio settings
-Expanded stats
-Compendium describing each creature
-Added achievements
-Added WIP Graveyard map
-Added some post-processing effects to the game

Bug Fixes:
-Fixed a divergence on Cabin that was considered to not be in any room
-Fixed an issue where a door sounds would play on doors it shouldn't
-Made the battery icon more reliable
-Fixed a small overlapping ceiling on Cabin
-Temporarily removed the apartment level



v0.5.1 ():

Changes:
-Changed 'Apartment' to 'Cabin'
-Skybox in Cabin
-Remade tutorial
-Player data is tracked correctly, and new data is tracked

Additions:
-You can now see your energy outside of the report UI
-Added dynamic difficulty, which will make the game easier if the player is doing bad (turned off by default)
-Added UI for flashlight
-If the player falls out of the map, they are teleported back inside the map
-Added sensitivity options
-Added mouse acceleration (currently no option to turn it on/off however. off by default)
-Added the ability to zoom in with right click
-Added Collectibles
-Added sounds for doors

Bug Fixes:
-Fixed issue where room selection would need to be clicked twice on first open
-Fixed cabin computer monitor showing a blank image
-Fixed Cabin issue where reports would not count after reporting a specific divergence (kitchen bottle)
-Fixed UI issue in Maria's Apartment
-Report UI now correctly pauses
-Fixed an issue in cabin where an object in the storage room had multiple types of movement attached to it



v0.5.0 (Apr 1, 2024) - Overhauling the look of the game:

Changes:
-Lighting Overhaul

Additions:
-Flashlight
-Maria's Apartment
-Lurker
-Reporting UI Overhaul
-You can now see where you are on the report UI
-New floor and wall textures

Bug Fixes:
-Fixed hints not working on certain lights
-Fixed flickering lights
-Fixed camera clipping on higher FOVs
-Flashlight can no longer cause energy to go below 0



v0.4.1 (Mar 27, 2024) - Accessibility update:

Changes:
-Made game settings generally easier across the board
-Increased game length by 2 minutes on apartment
-Overhauled many apartment divergences
-Removed the randomly flickering lights and moved it to the hint system instead

Additions:
-Added 'Spin' divergence
-Added the 'MoveContinuous' divergence, which causes an object to continuously move instead of just once
-Added FOV settings
-Added Devan
-Added a 'hint' system
-Added text to let user know settings have been applied
-Added a loading screen when the player selects a level

Bug Fixes:
-Fixed player being able to move when the game is over/jumpscare is playing
-Fixed error sound not playing when the player hasn't selected a divergence type or room



v0.4.0 (Mar 23, 2024) - More Creatures Update:

Changes:
-Changed some divergences in the apartment
-Creatures now have a slightly higher chance to spawn
-Moved the players UI controls to a different class

Additions:
-Added 'Chaser' as a special creature
-Chaser is smaller and faster, but cannot kill the player
-Special creatures spawn earlier in the match than zombies, but cannot kill
-Added creature spawn rate to settings
-Debug menu
-Added crouching

Bug Fixes:
-Fixed missed divergence count number

Known issues:
-Sounds that are playing when paused, don't continue playing when unpaused



v0.3.4 (Mar 15, 2024):

Changes:
-Creatures now cost 7 energy instead of 8
-Refactored energy calculation code
-Removed 'bedroom' toggle from settings
-Completely overhauled creature spawning, now separated from divergence spawning
-Creatures spawn more rarely, but are more deadly
-Guaranteed ender to spawn when each room has a divergence

Additions:
-Added jumpscare when dying to zombie or ender
-Each divergence can now have its own energy cost
-A sound plays when you try to report without enough energy
-The type of divergence you have selected is now highlighted

Bug Fixes:
-Fixed grace period game setting not being applied




v0.3.3 (Mar 11, 2024):

Changes:
-Removed light divergence

Additions:
-Created an object lockout system, where the same dynamic object cannot become a divergence before a certain number of other divergences have occurred
-Apartment updates
-Lights now flicker when a certain amount of divergences are active
-Some lights have a chance to turn off if each room has a divergence active
-Improved tutorial
-Main menu music

Bug fixes:
-Refactored selection code
-Re-added code that was accidentally removed, causing audio divergences to no longer function
-Fixed bug that sometimes caused player to not be able to report
-Fixed sound not playing in tutorial
-Deselecting a room no longer saves your last selection
-Fixed bug for zombies spawning in the wrong room




v0.3.2 (Mar 10, 2024):

Additions:
-Creatures can now spawn in the same room as the player
-Creatures now spawn in the furthest corner of a room from the player
-Slightly increased creature detection range
-Divergences now have some randomness to their timing
-Added sound feedback for reporting

Bug Fixes:
-Allowed for guess lockout to be changed, value no longer hard coded
-Fixed bug that caused wrong guess even when guessing correctly after wrong guess




v0.3.1 (Mar 9, 2024)

Additions:
-Added more difficulty settings
-You now get 2 more energy back when correctly reporting a creature

Bug fixes:
-Refactored many classes
-Energy no longer shows as a decimal



v0.3.0 (Mar 9, 2024) - The Difficulty update:

Additions:
-Added difficulty options accessible before choosing a level
-Added more divergences to apartment
-Lights now randomly get dimmer OR brighter
-Added volume slider

Bug Fixes:
-Fixed stack overflow when each room had a divergence



V0.2.1 (Mar 8, 2024):

Additions:
-Added sounds for creatures
-Can now press E interact with doors and other openable objects

Bug Fixes:
-Can no longer interact while in menu
-Reporting a creature in a room that has a divergence in it will no longer cause the count of divergences in that room to go below 0, unintentionally allowing an additional divergence to spawn



v0.2.0 (Mar 7, 2024) - Yippie Update

Additions:
-Ender creature
-Object movement now applicable to any object
-Changed some divergence names for clarity
-Room+Timer text no longer disable when opening report menu
-Added background to report menu and room+timer to make it easier to read in certain areas
-Allowed reporting earlier after reporting

Bug fixes:
-Chess board divergence now correctly returns to original positions if reported mid-animation
-Fixed apartment toilet audio not playing
-fixed audio never disabling

Known Issues:
-Audio stops playing if paused for a certain amount of time



v0.1.4 (Mar 6, 2024):

Additions:
-Added audio divergence
-Added chess board divergence
-Added widescreen support

Bug Fixes:
-Fixed incorrect check that would cause report text to always show as correct even if no matching divergences were found
-Fixed 'retry' button on end game screen always loading apartment level
-Creatures now stop moving when game is paused
-Fixed end game screen not scaling correctly
-Fixed type selection list never being cleared upon submitting
-Fixed main menu buttons
-Fixed audio and movement divergences being unreportable



v0.1.3 (Mar 4, 2024):

Additions:
-Added tutorial to main menu
-Added door controls to tutorial
-Added creature mechanics to tutorial
-Allowed changing of game time to any number of seconds

Bug Fixes:
-Made popups easier to read by adding a background
-Fixed list of correct objects being set to null instead of a new list after report
-Fixed report text being updated at incorrect times



v0.1.2 (Mar 4, 2024) - First release/Tutorial update:

Additions:
-Added 'Popups' that pause the game and show a message
-Added a basic tutorial
-Added a way to create creatures/divergences programmatically




Bug Fixes:
-Fixed bug that caused too much or no energy to be consumed on reports";
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            BackButtonEvent();
        }
    }
}
