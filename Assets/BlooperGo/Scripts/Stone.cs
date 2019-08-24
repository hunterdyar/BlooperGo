using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blooper.Go{
    public class Stone
    {
        public Point point;
        public Chain chain = null;
        public StoneColor color;
        public int turnNumberPlayed;
        //chain
        public bool captured = false;

    }
}
