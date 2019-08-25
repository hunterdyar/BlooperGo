using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blooper.Go{
    [System.Serializable]
    public struct Stone
    {
        public Vector2 position;
        public Chain chain;
        public StoneColor color;
        public int turnNumberPlayed;
        //chain

    }
}
