using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class ForceNextTurnData{
        public GameObject GameObject;
        public ForceNextTurnData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class ForceNextTurn : UnityEvent<ForceNextTurnData>{}
}