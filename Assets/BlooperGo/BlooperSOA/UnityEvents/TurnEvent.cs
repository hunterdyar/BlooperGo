using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Blooper.Go;

[System.Serializable]
public class TurnEvent : UnityEvent <Vector2,StoneColor> {}