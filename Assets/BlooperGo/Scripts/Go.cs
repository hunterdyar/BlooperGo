using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Blooper.SOA;
namespace Blooper.Go{
public enum StoneColor{
    black,
    white,
    none
}
public class Go : MonoBehaviour
{


    [HideInInspector]
    public List<Stone> allStones;
    [HideInInspector]
    public List<Stone> blackStones;
    [HideInInspector]
    public List<Stone> whiteStones;
    [HideInInspector]
    public List<Chain> chains;
    [HideInInspector]
    public List<Territory> territories;
    [Header("Settings")]

    [Tooltip("Initiate on awake? Leave checked if you don't know")]
    public bool initiateOnAwake = true;

    [Tooltip("see boardSetup for settings. Tells the boardSetup to generate points before calling the pre-initiate function that you can use to give those points worldPos's or whatever.")]
    public bool generatePoints;
    [Header("Game Data Holders")]
    [Tooltip("Contains the dictionary of all of the points.")]
    public BoardSetup boardSetup;
    [Tooltip("Contains a running list of moves")]
    public GameHistory history;
    [Tooltip("Contains status about the game.")]
    public GameStateInfo gsi;
    [Header("Events")]

    [Tooltip("Before any of our initiation or board setup. Useful for assigning points world positions")]
    public GameEvent PreInitiateEvent;

    [Tooltip("Fires out the newly created Stone object in need or visual representation.")]
    public StoneGameEvent NewStonePlayedEvent;
    [Tooltip("Required. Fires out whenever you try to move. True (also the above event fires), is false when move was invalid")]
    public BoolGameEvent TurnResponseEvent;
    
    [Tooltip("Fires out whenever you try a move, with an int that tells you error code of the move.")]
    public IntGameEvent TurnResponseCodeEvent;

    
    [Tooltip("The following stones need to be removed for some reason (likely: captured)")]

    public StoneGameEvent RemovedStoneEvent;
    [Tooltip("Whatever stones is at this position needs to be removed. Fired same as above. Use whichever suites you or not at all. ")]
    
    public Vector2GameEvent RemoveStoneAtPositionEvent;

    [Tooltip("Fires when the game is reset, (and on awake) so you know when to reset all the visuals.")]

    public GameEvent ResetGameBoardEvent;

    [Tooltip("Fires when the game is over. A bool in the game state gets flipped too you can check")]
    public GameEvent GameEndedEvent;

    [Header("Other")]
    public StoneColor previouslyPlayedColor = StoneColor.white;

    void Awake(){
        if(initiateOnAwake){
            Init();
        }
    }
    public void Init(){
        StartCoroutine(Initiate());
    }

    public IEnumerator Initiate(){
        Debug.Log("initiating game.");
        if(generatePoints){
            //Debug.Log("calling board generation directly");
            boardSetup.GenerateBoard(boardSetup.boardSize);
        }
        yield return new WaitForEndOfFrame();
        //Debug.Log("calling pre-initiation event");
        if(PreInitiateEvent != null){
            PreInitiateEvent.Raise();
        }//Here I am generating the points by inferring where they should be.
            //It would also be easy to make some "point location" gameobject at the location of every point, and have that one create a point object and add it to the list in boardSetup.
        //After boardSetup has all the points, we can go through all of the points in the list and do setup: register neighbors, create a lookup dictionary.
        yield return new WaitForEndOfFrame();

        //Debug.Log("initiating board");
        boardSetup.InitiateBoard();
        yield return new WaitForEndOfFrame();

        //Reset local whatswhat.
        allStones = new List<Stone>();
        blackStones = new List<Stone>();
        whiteStones = new List<Stone>();
        chains = new List<Chain>();
        territories = new List<Territory>();
        //Reset info-storing game objects.
        history.ResetHistory();
        gsi.ResetGameState();
        if(ResetGameBoardEvent != null){
            ResetGameBoardEvent.Raise();
        }
        yield return null;
    }

    public void Undo(){
        Debug.Log("undo to "+(gsi.turnNumber-2));
        if(gsi.turnNumber > 1){//1 is when the first turn. 
            TurnDetails turn = history.GetTurnDetials(gsi.turnNumber -2);
                gsi.currentTurn = OtherColor(turn.player);
                RevertToPreviousTurn(gsi.turnNumber - 2);
                gsi.turnNumber = gsi.turnNumber-1;
        }
    }
    public void CurrentPlayerResigns(){
        gsi.gameOver = true;
        //gsi.Tally();//This currently only sets the winner, so not needed. But with scoring math, if I update that... we may want to call it then override the winner
        gsi.winner = OtherColor(gsi.currentTurn);
    }

    public void PassTurn(){
        if(gsi.passedLastTurn != StoneColor.none){
            gsi.gameOver = true;
            //UpdateTerritoryCounts();//We shouldnt have to do this here since it was done after the last stone was played, and its passed?
            gsi.Tally();
            if(GameEndedEvent != null){
                GameEndedEvent.Raise();
            }
        }
        gsi.passedLastTurn = gsi.currentTurn;

        //AGA rules have you give your opponent a prisoner when you pass. 
        //we store it as a separate int so we can implemenet different scoring system rulesets and compare.
        if(gsi.currentTurn == StoneColor.black){
            gsi.timesBlackPassed++;
        }else if(gsi.currentTurn == StoneColor.white){
            gsi.timesWhitePassed++;
        }  

        gsi.currentTurn = OtherColor(gsi.currentTurn);
        if(TurnResponseEvent != null){
            TurnResponseEvent.Raise(true);
        }
        if(TurnResponseCodeEvent != null){
            TurnResponseCodeEvent.Raise(1);
        }
    }

    public void PlaceStone(Vector2 position,StoneColor color){
        
        if(gsi.gameOver){
           // Debug.Log("sorry, can't make a move, the game is over!");
            if(TurnResponseEvent != null){
                TurnResponseEvent.Raise(false);
            }
            if(TurnResponseCodeEvent != null){
                TurnResponseCodeEvent.Raise(2);
            }
        }

        if(color != gsi.currentTurn){
          //  Debug.Log("Not your turn to play!");
            if(TurnResponseEvent != null){
                TurnResponseEvent.Raise(false);
            }
            if(TurnResponseCodeEvent != null){
                TurnResponseCodeEvent.Raise(3);
            }
            return;
        }

        Point point = null;
        point = boardSetup.GetPoint(position);
        if(point != null){
            if(point.stoneHere != null){
               // Debug.Log("Can't Place stone here!");
                if(TurnResponseEvent != null){
                TurnResponseEvent.Raise(false);
            }
            if(TurnResponseCodeEvent != null){
                TurnResponseCodeEvent.Raise(4);
            }
                return;
            }else{
                

                //I deleted the suicide check for immediate neighbors, because chain liberties should check that anywayyyy?
        

                //Start adding the stone...
                Stone s = new Stone();
                s.color = color;
                s.point = point;
                s.turnNumberPlayed = gsi.turnNumber;
                point.stoneHere = s;

                //Oop i have to do ALL OF THIS like three times for a snapback? uh oh//edit this didnt change anything but should have tho, so i'm leaving it.
                //I think it fixes a bug that i hadnt found yet, with previously defined chains boofing the addNieghborsToChain function, which checks for null chains and add those only so it isnt infinitly recursive
                //we might be able to check this not with the stone.chain property but if the list in the addneighbros list already contains the stone or not, and get rid of this foreach
                if(chains != null){//We initialize in a coroutine, so it's possible to try to play in the first few frames and boof everything up.
                    chains.Clear();
                    foreach(Stone e in allStones){
                        e.chain = null;
                    }
                }
                ///
/// 
                //need to check if the chain we place our stone in, without direct suicide, would become a suicide. That's still invalid.
                Chain c = new Chain();
                chains.Add(c);
                s.chain = c;
                c.stones.Add(s);
                c.color = s.color;
                AddNeighborsToChain(s,c);//This is recursive.

                //we delete the single suicide check because the chain can be lenght 1 and that was redundant anyway.

                //TODO snapback checks!?

                c.DefineLiberties();//get all open ajacent spots to the chain.
                if(c.liberties.Count == 0){
                    //Invalid?
                    bool snapback = false;

                        ///Check if any enemies surrounding the point WOULD be captured. 
                        List<Stone> enemyNeighbors = GetEnemyNeighbors(s);

                        //Begin the process of clearing all of our chains.
                        chains.Clear();
                        foreach(Stone e in allStones){
                            e.chain = null;
                        }
                        //We will have to do this again. it's not optimized code.

                        foreach(Stone e in enemyNeighbors){
                            //e is the enemy surrounding the stone we just placed.

                            //First we get the surrounding chain. 
                            Chain g = new Chain();//I picked g because it rhymes with c dont @ me.
                            //chains.Add(g);//not neccesary, since it will all get cleared before we use this? but it feels like i should have it in the list anyway?

                            if(e.chain == null){
                                e.chain = g;
                                g.stones.Add(e);
                                g.color = e.color;
                                AddNeighborsToChain(e,g);//This is recursive.
                            }

                            if(g.stones.Count != 0){
                                g.DefineLiberties();
                            }

                            if(g.liberties.Count == 0 && g.stones.Count > 0){
                                //okay, so we CAN place this point!
                                // Debug.Log("Snapback?");
                                snapback = true;
                                Capture(g);//capture this now because otherwise when we do it later it wont know which chain to capture, and probably capture both?
                                //We can do the rest from below, let it do the rest of the chains (i mean, does it need to?) and calculate points and all that.
                                break;//if we break ... is it possible to snapback two chains at once?
                            }
                        }
                        
                    ///     
/// 
                    if(!snapback){
                    ///
                        point.stoneHere = null;//undo
                        s = null;//get ready to garbage collect.
                       // Debug.Log("Cant place stone here, chain suicide!");
                        if(TurnResponseEvent != null){
                            TurnResponseEvent.Raise(false);
                        }
                        if(TurnResponseCodeEvent != null){
                            TurnResponseCodeEvent.Raise(5);
                        }
                        return;
                    }
                }
                //So we have to build a one of chain from this location?
                
                //OR do we just do it, check that something beefed, and then do a history state revert? That feels clumsy.
                //EXCEPT we might have to do a history state storing and comparing thing ANYWAY because of ko rule.

                //end suicide and snapbacks.

                ///KO rule
                /// 
                
                
                List<Stone> possibleAllStones = new List<Stone>(allStones);
                possibleAllStones.Add(s);
                int[] possibleHistory = GameStateFromStonesList(possibleAllStones);
                
                if(gsi.turnNumber-2 >= 0){                    
                    IStructuralEquatable se1 = possibleHistory;
//Next returns True
                    if(se1.Equals (history.GetGameStateByTurn(gsi.turnNumber-2), StructuralComparisons.StructuralEqualityComparer)){
                        //Debug.Log("Ko rule! Cant return game to previous state.");
                        if(TurnResponseEvent != null){
                            TurnResponseEvent.Raise(false);
                        }
                        if(TurnResponseCodeEvent != null){
                            TurnResponseCodeEvent.Raise(6);
                        }
                        return;
                    } 
                }
                //place stone

                
                allStones.Add(s);
                //
                if(color == StoneColor.black){
                    blackStones.Add(s);
                }else{
                    whiteStones.Add(s);
                }
                previouslyPlayedColor = color;
                //end placing stone

                DefineChains();//Defining chains starts the ball rolling on our capture logic too.


                UpdateTerritoryCounts();//Does a number of recursive things, including going off and tallying the points.

                //Increment the turn counter, switch the player turns.

                history.AddToHistory(gsi.turnNumber,s,GameStateFromStonesList(allStones),boardSetup);
                gsi.turnNumber++;
                gsi.currentTurn = OtherColor(gsi.currentTurn);
                //Call Stone event to visually place a stone.
               
                NewStonePlayedEvent.Raise(s);
                if(TurnResponseEvent != null){
                    TurnResponseEvent.Raise(true);
                }
                if(TurnResponseCodeEvent != null){
                    TurnResponseCodeEvent.Raise(1);
                }
                history.AddToHistory(gsi.turnNumber,s);
            }
        }else{
           // Debug.Log("Invalid Position, no point at: "+ position);
            if(TurnResponseEvent != null){
                TurnResponseEvent.Raise(false);
            }
            if(TurnResponseCodeEvent != null){
                TurnResponseCodeEvent.Raise(7);
            }
            return;
        }
    }
    void RevertToPreviousTurn(int turnNumber){
        Debug.Log("reverting");
        int[] newStonesInt = history.GetGameStateByTurn(turnNumber);
        List<Stone> newStones = StonesFromGameState(newStonesInt,boardSetup);

        Debug.Log(newStones.Count + " - "+ allStones.Count);
        List<Stone> stonesToAdd = new List<Stone>();
        List<Stone> stonesToRemove = new List<Stone>();
        foreach(Stone s in newStones){
            bool addThisOne = true;

            foreach(Stone z in allStones){

                if(CompareStones(s,z)){
                    //okay, so a stone with the same color and position is already in this list. we dont need to add our stone.
                    //all stones does have the stone we are checking, so we do not need to add it.
                    addThisOne = false;
                    break;
                }
            }
            if(addThisOne){
                stonesToAdd.Add(s);
            }
        }
        foreach(Stone s in allStones){
            bool removeThisOne = true;
            foreach(Stone z in newStones){
                if(CompareStones(s,z)){
                    //okay, so a stone with the same color and position is already in this list. we dont need to add our stone.
                    //new stones does not have the stone we are checking, thus we need to remove it.
                    removeThisOne = false;
                    break;
                }
            }
            if(removeThisOne){
                stonesToRemove.Add(s);
            }
        }
        foreach(Stone s in stonesToAdd){
            Debug.Log("add a stone");
            allStones.Add(s);
            NewStonePlayedEvent.Raise(s);
            if(s.color == StoneColor.black){
                blackStones.Add(s);
            }else if(s.color == StoneColor.white){
                whiteStones.Add(s);
            }
        }
        foreach(Stone s in stonesToRemove){
            Debug.Log("remove a stone");
            Stone x = FindEquivalentStoneInList(s,allStones);
            allStones.Remove(x); 
            x.point.stoneHere = null;

            //these lists may be borked.
            if(blackStones.Contains(s)){
                blackStones.Remove(s);
            }
            if(whiteStones.Contains(s)){
                whiteStones.Remove(s);
            }
            if(RemovedStoneEvent != null){
                RemovedStoneEvent.Raise(s);
            }else if(RemoveStoneAtPositionEvent != null){
                RemoveStoneAtPositionEvent.Raise(s.point.position);
            }else{
                Debug.LogError("You need at least one remove stone event defined");
            }
        }

    }
    public void DefineChains(){
        chains.Clear();
        foreach(Stone s in allStones){
            s.chain = null;
        }
        //lol how does garbage collection work? do i need to... delete those chains?
        foreach(Stone s in allStones){
            if(s.chain == null){//at the end of this, every stone should be part of a chain.
                Chain c = new Chain();
                chains.Add(c);
                s.chain = c;
                c.stones.Add(s);
                c.color = s.color;
                AddNeighborsToChain(s,c);
            }
        }
        foreach(Chain c in chains){
            c.DefineLiberties();
            if(c.liberties.Count == 0){
                Capture(c);
            }
        }
    }

    public void AddNeighborsToChain(Stone s, Chain c){
        List<Stone> sn = GetSameColoredNeighbors(s);
        foreach(Stone news in sn){
            if(news.chain == null){
                news.chain = c;
                c.stones.Add(news);
                AddNeighborsToChain(news,c);
            }
        }
    }
    public void AddNeighborsToTerritory(Territory t, Point p){
        List<Point> pn = p.neighbors;
        foreach(Point newp in p.neighbors){
            if(newp.territory == null){//we dont want to go back and forth between 2 neighbors.
                if(newp.stoneHere != null){//we hit some kind of wall!
                    if(!t.isOwnerDefined){//first stone we hit in the floodFill.
                        t.isOwnerDefined = true;//We will say that the territory is defined and owned by this color.
                        t.territoryOwner = newp.stoneHere.color;
                    }else{
                        if(t.territoryOwner != newp.stoneHere.color){
                            t.territoryOwner = StoneColor.none;
                        }
                    }
                }else{
                    newp.territory = t;
                    t.points.Add(newp);
                    AddNeighborsToTerritory(t,newp);//continue the recursive floodfill.
                }
            }
        }
    }
    public void DefineTerritories(){
        territories.Clear();
        foreach(Point p in boardSetup.points.Values){
            p.territory = null;
        }
        //lol how does garbage collection work? do i need to... delete those chains?
        foreach(Point p in boardSetup.points.Values){
            if(p.stoneHere == null && p.territory == null){//at the end of this, every point not occupied by a stone should be part of a territory.
                Territory t = new Territory();
                t.isOwnerDefined = false;//we dont know who owns this territory yet.
                territories.Add(t);
                p.territory = t;
                t.points.Add(p);
                t.territoryOwner = StoneColor.none;//as of yet, I guess we assume its not owned by anyone.
                AddNeighborsToTerritory(t,p);
            }
        }
        List<Territory> nullTerritories = new List<Territory>();
        foreach(Territory t in territories){
            if(t.territoryOwner == StoneColor.none){
                nullTerritories.Add(t);
            }
        }
        foreach(Territory t in nullTerritories){
            territories.Remove(t);
        }
        
    }
    public static List<Stone> GetSameColoredNeighbors(Stone s){
        List<Stone> sameColoredNeighbors = new List<Stone>();
        foreach(Point p in s.point.neighbors){
            if(p.stoneHere != null){
                if(p.stoneHere.color == s.color && !s.captured){
                    sameColoredNeighbors.Add(p.stoneHere);
                }
            }
        }
        return sameColoredNeighbors;
    }
    public static List<Stone> GetEnemyNeighbors(Stone s){
        List<Stone> enemyNeighbors = new List<Stone>();
        foreach(Point p in s.point.neighbors){
            if(p.stoneHere != null){
                if(p.stoneHere.color == OtherColor(s.color) && !s.captured){
                    enemyNeighbors.Add(p.stoneHere);
                }
            }
        }
        return enemyNeighbors;
    }
    public static List<Point> GetEmptyNeighbors(Point point){
        List<Point> emptyNeighbors = new List<Point>();
        foreach(Point p in point.neighbors){
            if(p.stoneHere == null){//if the nighbor is a liberty.
                if(!emptyNeighbors.Contains(p)){
                    emptyNeighbors.Add(p);
                }
            }
        }
        return emptyNeighbors;
    }

    public void Capture(Chain c){
        foreach(Stone s in c.stones){
            RemovedStoneEvent.Raise(s);
            s.point.stoneHere = null;
            s.captured = true;
            s.point = null;
            allStones.Remove(s);
            if(s.color == StoneColor.black){
                blackStones.Remove(s);
                gsi.blackPrisonersCapturedByWhite++;
            }else if(s.color == StoneColor.white){
                whiteStones.Remove(s);
                gsi.whitePrisonersCapturedByBlack++;
            }
        }

    }

    public void UpdateTerritoryCounts(){
        gsi.whiteStonesOnBoard = whiteStones.Count;
        gsi.blackStonesOnBoard = blackStones.Count;
        
        if(gsi.whiteStonesOnBoard < 2 || gsi.blackStonesOnBoard<2){
            return;
        }
        //Probably should be a coroutine yea?
        DefineTerritories();

        gsi.whiteTerritoryTotal = 0;
        gsi.blackTerritoryTotal = 0;
        foreach(Territory t in territories){
            if(t.territoryOwner == StoneColor.white){
                gsi.whiteTerritoryTotal = gsi.whiteTerritoryTotal + t.points.Count;
            }else if(t.territoryOwner == StoneColor.black){
                gsi.blackTerritoryTotal = gsi.blackTerritoryTotal + t.points.Count;
            }
        }
    }

    public static int[] GameStateFromStonesList(List<Stone> stones){
        //the game state is stored as an int[] that is like a triangle vertices array, but a quad.
        //its turn,x,y,v,turn2,x2,y2,v2,turn3,x3,y3,v3, etc.
        //This way it stays small with small boards: it doesnt count empty points. Which makes comparators and operations quicker for most of the game.
        //The current system deletes captured stones. I am not sure that it's important to keep those, because I've got turn numbers here.
        //Originally I assumed turn numbers by sorting the list, before remembering that some stones were just gone.


        int[] gsint = new int[stones.Count*4];
        for(int i = 0;i < stones.Count;i++){
            gsint[i*4] =  stones[i].turnNumberPlayed;
            gsint[i*4+1]= (int)stones[i].point.position.x;
            gsint[i*4+2]= (int)stones[i].point.position.y;
            gsint[i*4+3]= StoneColorToInt(stones[i].color);
        }

        return gsint;
    }

    //Some explanation here: 
    public static List<Stone> StonesFromGameState(int[] gs, BoardSetup board){
        List<Stone> stones = new List<Stone>();
        for(int i = 0; i<gs.Length;i = i+4){
            Stone s = new Stone();
            s.turnNumberPlayed = gs[i];
            s.point = board.GetPoint(new Vector2(gs[i+1],gs[i+2]));
            board.GetPoint(new Vector2(gs[i+1],gs[i+2])).stoneHere = s;
            s.color = IntToStoneColor(gs[i+3]);
            stones.Add(s);
        }
        return stones;
    }

    public static StoneColor OtherColor(StoneColor c){
        if(c == StoneColor.black){
            return StoneColor.white;
        }else if(c == StoneColor.white){
            return StoneColor.black;
        }else{
            Debug.Log("just tried to reverse no color?");
            return StoneColor.none;
        }
    }
    public static int StoneColorToInt(StoneColor sc){
        if(sc == StoneColor.black){
            return 1;
        }else if(sc == StoneColor.white){
            return 2;
        }else{
            return 0;
        }
    }
    public static StoneColor IntToStoneColor(int x){
        if(x == 1){
            return StoneColor.black;
        }else if(x == 2){
            return StoneColor.white;
        }else{
            return StoneColor.none;
        }
    }
    public static bool CompareStones(Stone a,Stone b){
        if(a == null & b == null){
            return true;//???? I GUESS?
        }
        if(a == null){
            return false;
        }
        if(b == null){
            return false;
        }
        return (a.turnNumberPlayed == b.turnNumberPlayed) && (a.color == b.color) && ((int)a.point.position.x == (int)b.point.position.x) && ((int)a.point.position.y == (int)b.point.position.y);
    }
    public Stone FindEquivalentStoneInList(Stone equivalentStone,List<Stone> checkStones){
        foreach(Stone s in checkStones){
            if(CompareStones(equivalentStone,s)){
                return s;
            }
        }
        return null;
    }
}

}