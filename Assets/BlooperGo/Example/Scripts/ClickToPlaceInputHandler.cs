using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blooper.Go;
using Blooper.SOA;
public class ClickToPlaceInputHandler : MonoBehaviour
{
    public GameToWorldMappingInfo settings;
    public GameStateInfo gsi;
    public TurnGameEvent tryPlayAStoneEvent;
    void Update(){
        if(Input.GetMouseButtonDown(0)){
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos = new Vector2(worldPos.x - settings.bottomLeftPos.x,worldPos.y-settings.bottomLeftPos.y);
            worldPos = new Vector2(worldPos.x/settings.pos11Distance.x,worldPos.y/settings.pos11Distance.y);
           // Debug.Log(worldPos);
        Vector2 playPosition = new Vector2(Mathf.Round(worldPos.x),Mathf.Round(worldPos.y));
        tryPlayAStoneEvent.Raise(playPosition,gsi.currentTurn);
        

        }
    }
}
