using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blooper.Go{

[System.Serializable]
public struct TurnDetails{
    public int turnNumber;
    public int stonesCapturedThisTurn;
    public StoneColor player;
    public Vector2 playPosition;
    public char columnAsLetter;
    public int rowAsNumber;
    public int[] gameState;
}


    public class GameHistory : ScriptableObject
    {
        
        public Dictionary<int,TurnDetails> gameHistory = new Dictionary<int, TurnDetails>();
        public int turnNumber = 0;

        public void ResetHistory(){
            gameHistory.Clear();
            TurnDetails td = new TurnDetails();//we need to set up the BASE turn, for history reverting.
            td.player = StoneColor.white;//black plays first. Which means the "previous" turn was white. So it sets the "next" turn to black, by returning white. (its dumb, dont judge me)
            td.gameState = new int[0];//empty! But we can "check" it and count that there are 0 stones at this state, and not throw an erorr.
            gameHistory[0] = td;
            turnNumber = 1;
        }


        public void AddToHistory(int turnNumber, Stone s, int stonesCaptued,int[] gameState, BoardSetup board){
            TurnDetails turnDetails = new TurnDetails();
            turnDetails.turnNumber = turnNumber;
            turnDetails.player = s.color;
            turnDetails.stonesCapturedThisTurn = stonesCaptued;
            turnDetails.playPosition = s.position;
            turnDetails.columnAsLetter = GetColumnLetter((int)s.position.x);
            turnDetails.rowAsNumber = (int)s.position.x+1;
            turnDetails.gameState = gameState;
            gameHistory[turnNumber] = turnDetails;
        }

        public TurnDetails GetTurnDetials(int turn){
            
            TurnDetails td;
            if(gameHistory.TryGetValue(turn, out td)){
                return td;
            }else{
                Debug.Log("cant get turn details, invalid turn number");
                return new TurnDetails();
            }
        }
        public int[] GetGameStateByTurn(int turn){
            
            TurnDetails td;
            if(gameHistory.TryGetValue(turn, out td)){
                return td.gameState;
            }else{
                Debug.Log("cant get gamestate, invalid turn number");
                return null;
            }
        }
        

        public static char GetColumnLetter(int x){
            string col = "abcdefghijklmnopqrstuvwxyz";
            return col[x];
        }
    }
}