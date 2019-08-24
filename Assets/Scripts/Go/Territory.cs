using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Blooper.Go{
    public class Territory
    {
    public List<Point> points = new List<Point>();
    public StoneColor territoryOwner;
    public bool isOwnerDefined = false;

    }
}