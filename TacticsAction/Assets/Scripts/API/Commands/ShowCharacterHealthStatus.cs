using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class ShowCharacterHealthStatusData{
        public GameObject GameObject;
        public ShowCharacterHealthStatusData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class ShowCharacterHealthStatus : UnityEvent<ShowCharacterHealthStatusData>{}
}