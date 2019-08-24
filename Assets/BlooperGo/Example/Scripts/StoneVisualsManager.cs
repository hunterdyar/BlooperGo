using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blooper.Go;
//sing ScriptableObjectArchitecture;
public class StoneVisualsManager : MonoBehaviour
{
    public GameObject StoneVisualPrefab;
    Dictionary<Stone,GameObject> stoneObjects = new Dictionary<Stone, GameObject>();
    Dictionary<Vector2,GameObject> stoneVisualObjects = new Dictionary<Vector2, GameObject>();
    public BoardSetup board;
    void Awake(){
        stoneObjects.Clear();
    }
    public void ResetGameBoard(){
        stoneObjects.Clear();
        foreach(Transform child in transform){
            Destroy(child.gameObject);
        }
    }
    public void NewStoneCreated(Stone s)
    {
        GameObject stoneVGO = GameObject.Instantiate(StoneVisualPrefab,transform);
        stoneVGO.GetComponent<StoneVisual>().stone = s;
        stoneObjects.Add(s,stoneVGO);
        stoneVisualObjects.Add(s.point.position,stoneVGO);
    }
    public void RemoveStone(Stone s){
        Debug.Log("visual manager: remove stone");
        GameObject svgo;
        if(stoneVisualObjects.TryGetValue(s.point.position,out svgo)){
            stoneVisualObjects.Remove(s.point.position);
            svgo.GetComponent<StoneVisual>().RemoveStone();
        }
        // if(stoneObjects.TryGetValue(s, out svgo)){
        //     stoneObjects.Remove(s);
        //     svgo.GetComponent<StoneVisual>().RemoveStone();
        // }else{
        //     Stone x = board.GetPoint(s.point.position).stoneHere;
        //     if(stoneObjects.TryGetValue(x, out svgo)){
        //         stoneObjects.Remove(x);
        //         svgo.GetComponent<StoneVisual>().RemoveStone();
        //     }else{
        //         Debug.Log("Cant remove stone. Is it already gone?");
        //     }
        // }
    }

    public void TryStonePlayedResponseHandler(bool succesful){
        if(!succesful){
            //play an error noise or whatever.
        }        
    }
}
