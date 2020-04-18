using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class ExecuteCharacterTurnData{
        public GameObject GameObject;
        public ExecuteCharacterTurnData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class ExecuteCharacterTurn : UnityEvent<ExecuteCharacterTurnData>{}
}