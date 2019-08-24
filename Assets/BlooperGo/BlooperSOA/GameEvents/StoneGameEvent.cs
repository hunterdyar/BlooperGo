using System.Collections.Generic;
using UnityEngine;
using Blooper.Go;
namespace Blooper.SOA
{
    [CreateAssetMenu]
    public class StoneGameEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<StoneGameEventListener> eventListeners = 
            new List<StoneGameEventListener>();

       

        public void Raise(Stone s)
        {
            for(int i = eventListeners.Count -1; i >= 0; i--)
                eventListeners[i].OnEventRaised(s);
        }

        public void RegisterListener(StoneGameEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(StoneGameEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }
}