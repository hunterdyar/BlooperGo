using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blooper.Go;
public class StoneVisual : MonoBehaviour
{
    public Stone stone;
    public BoardSetup board;
    public Sprite whiteSprite;
    public Sprite blackSprite;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = board.GetPoint(stone.position).worldPosition;
        if(stone.color == StoneColor.black){
            spriteRenderer.sprite = blackSprite;
        }else{
            spriteRenderer.sprite = whiteSprite;
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
