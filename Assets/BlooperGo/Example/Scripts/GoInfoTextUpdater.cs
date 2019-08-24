using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blooper.Go;
using UnityEngine.UI;
public class GoInfoTextUpdater : MonoBehaviour
{
    public GameStateInfo gameStateInfo;
    public Text text;

    void Awake(){
        if(text == null){
            text = GetComponent<Text>();
        }
    }
    void Update()
    {
        text.text = "Turn "+gameStateInfo.turnNumber+": "+gameStateInfo.currentTurn.ToString();
        text.text = text.text+"\n\n";
        text.text = text.text+"white: "+gameStateInfo.whitePointsArea;
        text.text = text.text+"\nblack: "+gameStateInfo.blackPointsArea;
    }
}
