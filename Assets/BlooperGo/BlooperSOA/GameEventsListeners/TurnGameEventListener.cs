using UnityEngine;
using UnityEngine.Events;
using Blooper.Go;
namespace Blooper.SOA
{
    public class TurnGameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public TurnGameEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public TurnEvent Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(Vector2 p,StoneColor c)
        {
            Response.Invoke(p,c);
        }
    }
}