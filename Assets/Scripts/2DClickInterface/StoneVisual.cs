using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blooper.Go;
public class StoneVisual : MonoBehaviour
{
    public Stone stone;
    public Sprite whiteSprite;
    public Sprite blackSprite;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(stone != null){
        transform.position = stone.point.worldPosition;
        if(stone.color == StoneColor.black){
            spriteRenderer.sprite = blackSprite;
        }else{
            spriteRenderer.sprite = whiteSprite;
        }
        }else{
            Debug.Log("cant do the thing? probably because either wasnt called correctly or because the stone was destroyed in teh same turn it was placed");
            
            //self destruct.
            Destroy(gameObject);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RemoveStone(){
        //call an animation or whatever.
        Destroy(gameObject);
    }
}
