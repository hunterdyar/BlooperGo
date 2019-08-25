using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blooper.Go;
//sing ScriptableObjectArchitecture;
public class StoneVisualsManager : MonoBehaviour
{
    public GameObject StoneVisualPrefab;
    Dictionary<Vector2,GameObject> stoneVisualObjects = new Dictionary<Vector2, GameObject>();
    public BoardSetup board;
    void Awake(){
        stoneVisualObjects.Clear();
    }
    public void ResetGameBoard(){
        stoneVisualObjects.Clear();
        foreach(Transform child in transform){
            Destroy(child.gameObject);
        }
    }
    public void NewStoneCreated(Stone s)
    {
        GameObject stoneVGO = GameObject.Instantiate(StoneVisualPrefab,transform);
        stoneVGO.GetComponent<StoneVisual>().stone = s;
        stoneVisualObjects.Add(s.position,stoneVGO);
    }
    public void RemoveStone(Stone s){
        GameObject svgo;
        if(stoneVisualObjects.TryGetValue(s.position,out svgo)){
            stoneVisualObjects.Remove(s.position);
            svgo.GetComponent<StoneVisual>().RemoveStone();
        }

    }

    public void TryStonePlayedResponseHandler(bool succesful){
        if(!succesful){
            //play an error noise or whatever.
        }        
    }
}
