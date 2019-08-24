using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class GameToWorldMappingInfo : ScriptableObject
    {
       [Tooltip("Rows and columns. Should be the same?")]
       public Vector2 boardSize;

       [Tooltip("origin of a grid")]
       public Vector2 bottomLeftPos;

       [Tooltip("The Rise and the Run")]
       public Vector2 pos11Distance;
    }
