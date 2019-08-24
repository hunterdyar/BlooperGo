# BlooperGo
Go game implementation in Unity. Game logic abstracted away from interface. It uses Area Scoring because that was easier to implement.

[Wikipedia page on go](https://en.wikipedia.org/wiki/Go_(game))

This project is designed to do most of the "game" part of creating a Go game - leaving the interface up to others. End-goal is perhaps to use it in support of a study with VR or AR interfaces.

## Important Background
The short version is that you can add various "Game Event listeners" and "Game Events" that you give the same asset. One script can raise the event, and the other will recieve it, and thanks to UnityEvent serialization, it's trivial to make that event call a function.
This minimizes dependencies (some of the events are optional, for example), and provides a clean interface to interact with the go logic.

There are two namespaces used, Blooper.Go, where all of the go logic lives, and Blooper.SOA, which is where the scriptable object unityEvent architecture lives. 
For more info on how or what Blooper.SOA does or why, see [this talk](https://www.youtube.com/watch?v=raQ3iHhE_Kk). 

The Go Game also stores all of the game logic in Scriptable Objects. There are three that are neccesary: 
1. A BoardSetup asset, which we tell the size of the board. It creates all of the point objects that work behind the scenes.
2. A GameState asset, which holds all of the information about the current state of the game.
3. A GameHistory asset, which keeps track of every move and a copy of the game state at every turn. 

The GameHistory is neccesary because I use it to check Ko Rule, but I plan on eventually making it optional, and rolling ko rule check and 1 undo state into the go script itself. 

## Setup
Check out the example (SampleScene). Basically just add the Go component to a gameObject and populate it with various appropriate assets: the events and scriptableObjects that the go game runs with.

The gameobject with the go script needs a few listeners too, for events that you fire off to call these functions:
1. PlaceStone()  (Turn Game Event). You call this event with a function with a vector2 for the location, and a Blooper.Go.StoneColor enum.
2. Init() (GameEvent). Optional. You can check "initiate on awake" and just reload the scene, perhaps.
3. PassTurn(). (GameEvent). This function will pass whoever the current player is.
4. CurrentPlayerResigns() (GameEvent). This function will end the game, with the current player the loser.

You need to listen to the following events (assign them in the go script) and do the appopriate action:
1. PreInitiate - use this event to take the generated points, and assign them transforms or worldLocations, or whatever you need to keep track of the world position.
2. NewStonePlayed - use this to create a visual representation of a stone and place it on the board.
3. TurnResponseEvent and TurnResponseCodeEvent - use this to give the player feedback when their attempt to place a stone did/didnt work
4. RemoveStone - remove a stone from the board.
5. ResetGameBoard - This is called at the end of initiation, for you to clean whatever you need to clean up, up, or delete all sorts of leftover objects on a restart
6. GameEndedEvent - called when the game ends.

## Notes
This isn't done. It's not very clean or tidy yet, and the game doesn't check if there are no valid places to move or not. Players just have to pass twice.
The Blooper.SOA is basically just a simple copy of Ryan Hipples example code right now, but I have plans to create a fully working scriptableObject Architecture for my own use, one that is explanatory and usable in teaching environments.
For serious projects, consider replacing all that with [this excellent package](https://assetstore.unity.com/packages/tools/utilities/scriptableobject-architecture-131520).

This histoy GameState is kind of weird. Instead of just storing a list of stone objects, or copies of them, it stores an int array. The array is in triples: x,y,color,x,y,color,x,y,color, etc. 
This plus the board size is all that's needed to store the state. I do it this way because it will be, in the future, easier to write and read that from a file, even one that you parse yourself with TextAsset and string splits and so on, which would be a fun coding assignment.


