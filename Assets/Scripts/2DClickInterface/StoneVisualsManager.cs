using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blooper.Go;
//sing ScriptableObjectArchitecture;
public class StoneVisualsManager : MonoBehaviour
{
    public GameObject StoneVisualPrefab;
    Dictionary<Stone,GameObject> stoneObjects = new Dictionary<Stone, GameObject>();
    
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
    }
    public void RemoveStone(Stone s){
        GameObject svgo;
        if(stoneObjects.TryGetValue(s, out svgo)){
            stoneObjects.Remove(s);
            svgo.GetComponent<StoneVisual>().RemoveStone();
        }else{
            Debug.Log("Cant remove stone. Is it already gone?");
        }
    }
    public void TryStonePlayedResponseHandler(bool succesful){
        if(!succesful){
            //play an error noise or whatever.
        }        
    }
}
