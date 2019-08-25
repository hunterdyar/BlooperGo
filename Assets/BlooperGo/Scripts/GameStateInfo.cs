using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Blooper.Go{
    public class GameStateInfo : ScriptableObject
    {
        public int turnNumber;
        public StoneColor currentTurn;
        public StoneColor passedLastTurn = StoneColor.none;
        public bool gameOver = false;
        public int blackPrisonersCapturedByWhite = 0;
        public int whitePrisonersCapturedByBlack = 0;
        public int whiteStonesOnBoard = 0;
        public int blackStonesOnBoard = 0;
        public int timesWhitePassed = 0;
        public int timesBlackPassed = 0;
        public int blackTerritoryTotal = 0;
        public int whiteTerritoryTotal = 0;
        public StoneColor winner = StoneColor.none;
        public int whitePointsAreaWithPasses{get{return timesBlackPassed+whiteStonesOnBoard+whiteTerritoryTotal;}}
        public int blackPointsAreaWithPasses{get{return timesWhitePassed+blackStonesOnBoard+blackTerritoryTotal;}}
        public int whitePointsArea{get{return whiteStonesOnBoard+whiteTerritoryTotal;}}
        public int blackPointsArea{get{return blackStonesOnBoard+blackTerritoryTotal;}}

        //Territory scoring I am not sure how to implement, because I would have to figure out seki.
        //uhg
        public void ResetGameState(){
            passedLastTurn = StoneColor.none;
            turnNumber = 1;
            currentTurn = StoneColor.black;
            gameOver = false;
            blackPrisonersCapturedByWhite = 0;
            whitePrisonersCapturedByBlack = 0;
            whiteStonesOnBoard = 0;
            blackStonesOnBoard = 0;
            timesWhitePassed = 0;
            timesBlackPassed = 0;
            blackTerritoryTotal = 0;
            whiteTerritoryTotal = 0;
            winner = StoneColor.none;
        }

        public void Tally(){//Figure out the winner.
            if(blackPointsArea>whitePointsArea){
                winner = StoneColor.black;
            }else if(whitePointsArea>blackPointsArea){
                winner = StoneColor.white;
            }else{
                winner = StoneColor.none;
            }
        }
    }
}