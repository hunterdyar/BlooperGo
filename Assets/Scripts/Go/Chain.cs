using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Blooper.Go{
    public class Chain
    {
    public List<Stone> stones = new List<Stone>();
    public StoneColor color;
    public List<Point> liberties = new List<Point>();


    public void DefineLiberties(){
        liberties.Clear();
        foreach(Stone s in stones){
            //for every stone in this chain (which is sometimes just 1)
            List<Point> newLibs = Go.GetEmptyNeighbors(s.point);//we get all of the empty neighbors
            foreach(Point p in newLibs){
                if(!liberties.Contains(p)){//for this list, we check if it isn't in our liberties, and add it.
                    liberties.Add(p);
                }
            }
        }

        
    }
    }
}