using System.Collections.Generic;
using UnityEngine;
using Blooper.Go;
namespace Blooper.SOA
{
    [CreateAssetMenu]
    public class TurnGameEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<TurnGameEventListener> eventListeners = 
            new List<TurnGameEventListener>();

        public void Raise(Vector2 p,StoneColor c)
        {
            for(int i = eventListeners.Count -1; i >= 0; i--)
                eventListeners[i].OnEventRaised(p,c);
        }

        public void RegisterListener(TurnGameEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(TurnGameEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }
}