using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blooper.Go{

[System.Serializable]
public struct TurnDetails{
    public int turnNumber;
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
            turnNumber = 0;
        }
        public void AddToHistory(int turnNumber, Stone s){
            TurnDetails turnDetails = new TurnDetails();
            turnDetails.turnNumber = turnNumber;
            turnDetails.player = s.color;
            turnDetails.playPosition = s.point.position;
            turnDetails.columnAsLetter = GetColumnLetter((int)s.point.position.x);
            turnDetails.rowAsNumber = (int)s.point.position.x+1;
            gameHistory[turnNumber] = turnDetails;


        }

        public void AddToHistory(int turnNumber, Stone s, int[] gameState){
            TurnDetails turnDetails = new TurnDetails();
            turnDetails.turnNumber = turnNumber;
            turnDetails.player = s.color;
            turnDetails.playPosition = s.point.position;
            turnDetails.columnAsLetter = GetColumnLetter((int)s.point.position.x);
            turnDetails.rowAsNumber = (int)s.point.position.x+1;
            turnDetails.gameState = gameState;

            gameHistory[turnNumber] = turnDetails;
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