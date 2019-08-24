using System.Collections.Generic;
using UnityEngine;
using Blooper.Go;
namespace Blooper.SOA
{
    [CreateAssetMenu]
    public class Vector2GameEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<Vector2GameEventListener> eventListeners = 
            new List<Vector2GameEventListener>();

       

        public void Raise(Vector2 v)
        {
            for(int i = eventListeners.Count -1; i >= 0; i--)
                eventListeners[i].OnEventRaised(v);
        }

        public void RegisterListener(Vector2GameEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(Vector2GameEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }
}