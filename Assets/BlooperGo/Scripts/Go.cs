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
    public List<Point> allStones;
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

    private int stonesCapturedThisTurn = 0;
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
        allStones = new List<Point>();
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
    void UpdateStoneCounts(){
        gsi.whiteStonesOnBoard = 0;
        gsi.blackStonesOnBoard = 0;
        foreach(Point p in allStones){
            if(p.stone.color == StoneColor.black){
                gsi.blackStonesOnBoard++;
            }else if(p.stone.color == StoneColor.white){
                gsi.whiteStonesOnBoard++;
            }
        }
        //gsi.Tally();
    }
    public void CurrentPlayerResigns(){
        gsi.gameOver = true;
        gsi.Tally();//This currently only sets the winner, so not needed. But with scoring math, if I update that... we may want to call it then override the winner
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
        Point point = boardSetup.GetPoint(position);
        if(point != null){
            if(point.stone.color != StoneColor.none){
               // Debug.Log("Can't Place stone here!");
                if(TurnResponseEvent != null){
                TurnResponseEvent.Raise(false);
            }
            if(TurnResponseCodeEvent != null){
                TurnResponseCodeEvent.Raise(4);
            }
                return;
            }else{
                
            
            
            stonesCapturedThisTurn = 0;
            //Suicide and snapback checks
                Chain g = null; 
                //I deleted the suicide check for immediate neighbors, because chain liberties should check that anywayyyy?
        
                //this is a soft "add" while we do out checks, and we will need to undo them on failures.
                point.stone.color = color;
                point.stone.turnNumberPlayed = gsi.turnNumber;

                //Oop i have to do ALL OF THIS like three times for a snapback? uh oh//edit this didnt change anything but should have tho, so i'm leaving it.
                //I think it fixes a bug that i hadnt found yet, with previously defined chains boofing the addNieghborsToChain function, which checks for null chains and add those only so it isnt infinitly recursive
                //we might be able to check this not with the stone.chain property but if the list in the addneighbros list already contains the stone or not, and get rid of this foreach
                if(chains != null){//We initialize in a coroutine, so it's possible to try to play in the first few frames and boof everything up.
                    chains.Clear();

                    for(int i =0;i<allStones.Count;i++){
                        allStones[i].stone.chain = null;
                    }
                }

                //need to check if the chain we place our stone in, without direct suicide, would become a suicide. That's still invalid.
                Chain c = new Chain();
                chains.Add(c);
                point.stone.chain = c;
                point.stone.chain.stones.Add(point);
                c.color = point.stone.color;
                AddNeighborsToChain(point,c);//This is recursive.

                //snapback checks
                bool snapback = false;

                c.DefineLiberties();//get all open ajacent spots to the chain.
                if(c.liberties.Count == 0){
                    //Invalid?

                        //Check if any enemies surrounding the point WOULD be captured. 
                        
                                //Begin the process of clearing all of our chains.
                                chains.Clear();
                                for(int i =0;i<allStones.Count;i++){
                                    allStones[i].stone.chain = null;
                                }
                                //We will have to do this clearing again. it's not optimized code.

                        //First we get the enemies surrounding the point.
                        List<Point> enemyNeighbors = GetEnemyNeighbors(point, color);
                        for(int i = 0;i<enemyNeighbors.Count;i++){
                            //e is the enemy surrounding the stone we just placed.

                            //First we get the surrounding chain. 
                            g = new Chain();                            

                            if(enemyNeighbors[i].stone.chain == null){
                                enemyNeighbors[i].stone.chain = g;
                                g.stones.Add(enemyNeighbors[i]);
                                g.color = enemyNeighbors[i].stone.color;
                                AddNeighborsToChain(enemyNeighbors[i],g);//This is recursive.
                            }
                        //So we check if the surrounding enemy neighbor chain has no more liberties now.
                            if(g.stones.Count != 0){
                                g.DefineLiberties();
                            }

                            if(g.liberties.Count == 0 && g.stones.Count > 0){
                                //okay, so we CAN place this point.... MAYBE

                                // Debug.Log("Snapback?");
                                snapback = true;
                            
                                //KO rule
                                    // One may not capture just one stone if that stone was played on the previous move and that move also captured just one stone.
                                    if(snapback && g.stones.Count == 1){
                                    //ko rule always happens during snapback (???) 
                                        if(history.gameHistory[gsi.turnNumber-1].stonesCapturedThisTurn == 1){
                                            if(g.stones[0].stone.turnNumberPlayed == gsi.turnNumber-1){
                                                
                                                //undo our placement
                                                point.stone.color = StoneColor.none;//undo our placement 
                                                point.stone.chain = null;
                                                point.stone.turnNumberPlayed = -1;
                                                if(TurnResponseEvent != null){
                                                    TurnResponseEvent.Raise(false);
                                                }
                                                if(TurnResponseCodeEvent != null){
                                                    TurnResponseCodeEvent.Raise(6);
                                                }
                                                return;
                                            }
                                        }
                                    }


                               Capture(g);//capture this on before we run the chain iteration but after ko rule because otherwise when we do it later it wont know which chain to capture, and probably capture both?
                                //We can do the rest from below, let it do the rest of the chains (i mean, does it need to?) and calculate points and all that.
                                break;//if we break ... is it possible to snapback two chains at once?
                            }
                        }
                        
                    ///     
/// 
                    if(!snapback){
                    ///
                        point.stone.color = StoneColor.none;//undo our placement 
                        point.stone.chain = null;
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
                
                  
                    //which feels more ... systematicc



                //place stone
                allStones.Add(point);
                //end placing stone

                DefineChains();//Defining chains starts the ball rolling on our capture logic too.


                UpdateTerritoryCounts();//Does a number of recursive things, including going off and tallying the points.


                history.AddToHistory(gsi.turnNumber,point.stone,stonesCapturedThisTurn,GameStateFromStonesList(allStones),boardSetup);
                stonesCapturedThisTurn = 0;
                //Increment the turn counter, switch the player turns.
                gsi.turnNumber++;
                gsi.currentTurn = OtherColor(gsi.currentTurn);
                //Call Stone event to visually place a stone.
               
                NewStonePlayedEvent.Raise(point.stone);
                if(TurnResponseEvent != null){
                    TurnResponseEvent.Raise(true);
                }
                if(TurnResponseCodeEvent != null){
                    TurnResponseCodeEvent.Raise(1);
                }
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
        int[] newStonesInt = history.GetGameStateByTurn(turnNumber);
        List<Point> newStones = StonesFromGameState(newStonesInt,boardSetup);
        List<Point> stonesToAdd = new List<Point>();
        List<Point> stonesToRemove = new List<Point>();
        foreach(Point s in newStones){
            bool addThisOne = true;
            foreach(Point z in allStones){
                if(CompareStones(s.stone,z.stone,false)){
                    addThisOne = false;
                    break;
                }
            }
            if(addThisOne){
                stonesToAdd.Add(s);
            }
        }
        foreach(Point s in allStones){
            bool removeThisOne = true;
            foreach(Point z in newStones){
                if(CompareStones(s.stone,z.stone,false)){
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
        foreach(Point s in stonesToAdd){
            Point p = boardSetup.GetPoint(s.position);
            p = s;
            allStones.Add(p);
            NewStonePlayedEvent.Raise(s.stone);
        }
        foreach(Point p in stonesToRemove){
            allStones.Remove(p); 
            p.stone.color = StoneColor.none;

            if(RemovedStoneEvent != null){
                RemovedStoneEvent.Raise(p.stone);
            }else if(RemoveStoneAtPositionEvent != null){
                RemoveStoneAtPositionEvent.Raise(p.position);
            }else{
                Debug.LogError("You need at least one remove stone event defined");
            }
        }

    }
    public void DefineChains(){
        chains.Clear();
        for(int i =0;i<allStones.Count;i++){
            allStones[i].stone.chain = null;
        }
        for(int i =0;i<allStones.Count;i++){
            
            if(allStones[i].stone.chain == null){//at the end of this, every stone should be part of a chain.
                Chain c = new Chain();
                chains.Add(c);
                allStones[i].stone.chain = c;
                c.stones.Add(allStones[i]);
                c.color = allStones[i].stone.color;
                AddNeighborsToChain(allStones[i],c);
            }
        }
        foreach(Chain c in chains){
            c.DefineLiberties();
            if(c.liberties.Count == 0){
                Capture(c);
            }
        }
    }

    public void AddNeighborsToChain(Point p, Chain c){
        List<Point> pl = GetSameColoredNeighbors(p);
        for(int i =0;i<pl.Count;i++){
            if(pl[i].stone.chain != c){
                pl[i].stone.chain = c;
                pl[i].stone=pl[i].stone;//probably not needed.
                c.stones.Add(pl[i]);
                AddNeighborsToChain(pl[i],c);
            }
        }
    }
    public void AddNeighborsToTerritory(Territory t, Point p){
        List<Point> pn = p.neighbors;
        foreach(Point newp in p.neighbors){
            if(newp.territory != t){//we dont want to go back and forth between 2 neighbors.
                if(newp.stone.color != StoneColor.none){//we hit some kind of wall!
                    if(!t.isOwnerDefined){//first stone we hit in the floodFill.
                        t.isOwnerDefined = true;//We will say that the territory is defined and owned by this color.
                        t.territoryOwner = newp.stone.color;
                    }else{
                        if(t.territoryOwner != newp.stone.color){
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
            if(p.stone.color == StoneColor.none && p.territory == null){//at the end of this, every point not occupied by a stone should be part of a territory.
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
    public static List<Point> GetSameColoredNeighbors(Point s){
        List<Point> sameColoredNeighbors = new List<Point>();
        foreach(Point p in s.neighbors){
            if(p.stone.color != StoneColor.none){
                if(p.stone.color == s.stone.color){
                    sameColoredNeighbors.Add(p);
                }
            }
        }
        return sameColoredNeighbors;
    }
    public static List<Point> GetEnemyNeighbors(Point point, StoneColor c){
        List<Point> enemyNeighbors = new List<Point>();
        foreach(Point p in point.neighbors){
            if(p.stone.color != StoneColor.none){
                if(p.stone.color == OtherColor(c)){
                    enemyNeighbors.Add(p);
                }
            }
        }
        return enemyNeighbors;
    }
    public static List<Point> GetEmptyNeighbors(Point point){
        List<Point> emptyNeighbors = new List<Point>();
        foreach(Point p in point.neighbors){
            if(p.stone.color == StoneColor.none){//if the nighbor is a liberty.
                if(!emptyNeighbors.Contains(p)){
                    emptyNeighbors.Add(p);
                }
            }
        }
        return emptyNeighbors;
    }

    public void Capture(Chain c){
        for(int i = 0;i<c.stones.Count;i++){
            stonesCapturedThisTurn++;
            c.stones[i].stone.color = StoneColor.none;
            c.stones[i].stone.chain = null;
            allStones.Remove(c.stones[i]);
            RemovedStoneEvent.Raise(c.stones[i].stone);
        }
    }

    

    public void UpdateTerritoryCounts(){
        UpdateStoneCounts();
        
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

    public static int[] GameStateFromStonesList(List<Point> stones){
        //the game state is stored as an int[] that is like a triangle vertices array, but a quad.
        //its turn,x,y,v,turn2,x2,y2,v2,turn3,x3,y3,v3, etc.
        //This way it stays small with small boards: it doesnt count empty points. Which makes comparators and operations quicker for most of the game.
        //The current system deletes captured stones. I am not sure that it's important to keep those, because I've got turn numbers here.
        //Originally I assumed turn numbers by sorting the list, before remembering that some stones were just gone.
        int[] gsint = new int[stones.Count*4];
        for(int i = 0;i < stones.Count;i++){
            gsint[i*4] =  stones[i].stone.turnNumberPlayed;
            gsint[i*4+1]= (int)stones[i].position.x;
            gsint[i*4+2]= (int)stones[i].position.y;
            gsint[i*4+3]= StoneColorToInt(stones[i].stone.color);
        }

        return gsint;
    }

    //Some explanation here: 
    public static List<Point> StonesFromGameState(int[] gs, BoardSetup board){
        List<Point> stones = new List<Point>();
        for(int i = 0; i<gs.Length;i = i+4){
            Point p = board.GetPoint(new Vector2(gs[i+1],gs[i+2]));
            p.stone.turnNumberPlayed = gs[i];
            p.stone.color = IntToStoneColor(gs[i+3]);
            stones.Add(p);
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
    public static bool CompareStones(Stone a,Stone b, bool considerTurnNumber){
        return (!considerTurnNumber || a.turnNumberPlayed == b.turnNumberPlayed) && (a.color == b.color) && ((int)a.position.x == (int)b.position.x) && ((int)a.position.y == (int)b.position.y);
    }
    public static bool CompareGameStates(int[] a, int[] b, bool considerTurnNumber){
        if(a.Length != b.Length){
            //okay first the easy one. 
            //multiple loops below is kind of fine because like 99% of the time it will be this
            return false;
        }
        //The other way to do this is to check all points locations in a for the same point location in b and if that color matches.
        //

        int aInb = 0;
        int bIna = 0;
        for(int i = 0;i<a.Length;i= i+4){
            for(int j = 0;j<b.Length;j= j+4){
                if((!considerTurnNumber || a[i] == b[j]) && a[i+1] == b[j+1] && a[i+2] == b[j+2]&& a[i+3] == b[j+3]){
                    aInb++;
                    break;
                }
            }
        }
        for(int i = 0;i<b.Length;i= i+4){
            for(int j = 0;j<a.Length;j= j+4){
                if((!considerTurnNumber || a[i] == b[j]) && a[i+1] == b[j+1] && a[i+2] == b[j+2]&& a[i+3] == b[j+3]){
                    bIna++;
                    break;
                }
            }
        }
        //if the number of a elements found in b is the same as the number of a elements, and vice versa.
        return (aInb == a.Length/4 && bIna ==b.Length/4);
    }
    public Point FindPointFromStoneInList(Stone equivalentStone,List<Point> checkStones){
        foreach(Point p in checkStones){
            if(CompareStones(equivalentStone,p.stone,false)){
                return p;
            }
        }
        return null;
    }
}

}