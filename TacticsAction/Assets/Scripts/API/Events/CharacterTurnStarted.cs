using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class CharacterTurnStartedData{
        public GameObject GameObject;
        public CharacterTurnStartedData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class CharacterTurnStarted : UnityEvent<CharacterTurnStartedData>{}
}