using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class HideHealthStatusData{
        public GameObject GameObject;
        public HideHealthStatusData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class HideHealthStatus : UnityEvent<HideHealthStatusData>{}
}