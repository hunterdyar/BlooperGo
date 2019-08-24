using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnResponseErrorExamples : MonoBehaviour
{
    public void TurnResponse(int response){
        switch(response){
            case 0:
               Debug.LogError("Error!?");
                break;
            case 1:
                //Success!
                break;
            case 2:
                Debug.Log("Can't play. Game is over.");
                break;
            case 3:
                Debug.Log("Not this color's turn to play");
                break;
            case 4:
                Debug.Log("Cant play, Stone already at point");
                break;
            case 5:
                Debug.Log("Cant play, this play would be suicide");
                break;
            case 6:
                Debug.Log("Cant play. This play violates Ko Rule. (Returns game to previous state");
                break;
            case 7:
                Debug.Log("Invalid Position. There is no point at given location");
                break;
        }
    }
}
