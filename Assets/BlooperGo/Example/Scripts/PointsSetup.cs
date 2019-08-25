using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blooper.Go;
public class PointsSetup : MonoBehaviour
{
    public GameObject pointObjectPrefab;
    public GameToWorldMappingInfo settings;
    public BoardSetup board;

    [ContextMenu("Generate Board")]
    public void GenerateBoard()
    {
        //The boardSetup function can generate a grid of points, but it won't assign a worldPosition to them.
        board.GenerateBoard(settings.boardSize);

        //The board script can run call this function. I'm calling it here for clarity, and have the "generate points" bool in the go script unchecked.
        //It would be fine to check that, and not manually call it with that above line.


        //We will go through and assign the world position by inferring it from the vector position. basically a y=mx+b re-mapping.
        //The other way to do this would be to have some gameObject at every point, and a function there that either 
        //a: knows its xy board pos and can use that to get the point, and assign the world position
        //or b: it just creates a point and does AddPoint, and we get rid of or dont generate the boards points to begin with. (the go board has a bool script for that)
        foreach(Point p in board.rawPointsList){
            p.worldPosition = new Vector2(settings.bottomLeftPos.x+settings.pos11Distance.x*p.position.x,settings.bottomLeftPos.y+settings.pos11Distance.y*p.position.y);
        }
    }
}
