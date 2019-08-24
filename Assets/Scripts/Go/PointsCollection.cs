using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blooper.Go{
    public class PointsCollection : ScriptableObject
    {
      public List<Point> points = new List<Point>();
      
      public void AddPoint(Point p){
         points.Add(p);
      }
    }
}