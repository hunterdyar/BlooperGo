using UnityEngine;
using UnityEngine.Events;
namespace Blooper.SOA
{
    public class BoolGameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public BoolGameEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public BoolEvent Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(bool v)
        {
            Response.Invoke(v);
        }
    }
}