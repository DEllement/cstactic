using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class HideCharacterHealthStatusData{
        public GameObject GameObject;
        public HideCharacterHealthStatusData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class HideCharacterHealthStatus : UnityEvent<HideCharacterHealthStatusData>{}
}