using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Blooper.Go{
    public class BoardSetup : ScriptableObject
    {
        public Vector2 boardSize = new Vector2(19,19);
        public List<Point> rawPointsList = new List<Point>();
        public Dictionary<Vector2, Point> points = new Dictionary<Vector2, Point>();
        public int totalPoints{get{return points.Count;}}
        public void Reset(){
            points.Clear();
        }
        public Point GetPoint(Vector2 pos){
            Point p = null;
            points.TryGetValue(pos,out p);
            return p;
        }
        public void AddPoint(Point p){
            rawPointsList.Add(p);
        }
        public void RegisterPoint(Point p, Vector2 pos)
        {
            points.Add(pos,p);
        }
        [ContextMenu("Initiate Board SO")]
        public void InitiateBoard(){
            Reset();
                
                foreach(Point p in rawPointsList){
                    RegisterPoint(p,p.position);
                }
                foreach(Point p in rawPointsList){
                    p.AssignNeighbors(this);
                }
            Debug.Log("board Setup Initialized. There are "+points.Count+" points in this boardSetup.");
        }

        public void GenerateBoard(Vector2 boardSize){
        rawPointsList.Clear();//Lets start from scratch with our points list first. 

        //get generating
        for(int x = 0;x<boardSize.x;x++){
            for(int y = 0;y<boardSize.y;y++){
                Point p = new Point();
                p.position = new Vector2(x,y);
                p.stone.color = StoneColor.none;
                p.stone.position = p.position;//this redundancy is dumb but important for comparing pointless (ha) stones.
                p.stone.turnNumberPlayed = -1;
                AddPoint(p);
            }
        }
    }
    }
}