using System.Collections.Generic;
using UnityEngine;
using Blooper.Go;
namespace Blooper.SOA
{
    [CreateAssetMenu]
    public class StringGameEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<StringGameEventListener> eventListeners = 
            new List<StringGameEventListener>();

       

        public void Raise(string s)
        {
            for(int i = eventListeners.Count -1; i >= 0; i--)
                eventListeners[i].OnEventRaised(s);
        }

        public void RegisterListener(StringGameEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(StringGameEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }
}