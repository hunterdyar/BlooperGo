using UnityEngine;
using UnityEngine.Events;
namespace Blooper.SOA
{
    public class StringGameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public StringGameEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public StringEvent Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(string s)
        {
            Response.Invoke(s);
        }
    }
}