using UnityEngine;
using UnityEngine.Events;
using Blooper.Go;
namespace Blooper.SOA
{
    public class StoneGameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public StoneGameEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public StoneEvent Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(Stone s)
        {
            Response.Invoke(s);
        }
    }
}