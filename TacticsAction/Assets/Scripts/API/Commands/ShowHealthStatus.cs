using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class ShowAllHealthStatusData{
        public GameObject GameObject;
        public ShowAllHealthStatusData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class ShowHealthStatus : UnityEvent<ShowAllHealthStatusData>{}
}