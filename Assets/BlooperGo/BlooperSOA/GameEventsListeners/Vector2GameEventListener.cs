using UnityEngine;
using UnityEngine.Events;
namespace Blooper.SOA
{
    public class Vector2GameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public Vector2GameEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public Vector2Event Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(Vector2 v)
        {
            Response.Invoke(v);
        }
    }
}