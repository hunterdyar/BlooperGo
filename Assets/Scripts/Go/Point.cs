using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Blooper.Go{
    public class Point
    {
        public Stone stoneHere = null;
        public Territory territory;
        public Vector2 position = Vector2.zero;
        public Transform worldTransform = null;
        public Vector3 worldPosition = Vector3.zero;
        public List<Point> neighbors = new List<Point>();
        public void AssignNeighbors(BoardSetup bs){   
            neighbors.Clear();
            Vector2[] dirs = {Vector2.left,Vector2.right,Vector2.up,Vector2.down};
            foreach(Vector2 d in dirs){
                Point n;
                if(bs.points.TryGetValue(position+d,out n)){
                    neighbors.Add(n);
                }
            }
        }

    }
}
