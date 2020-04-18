using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class CharacterTurnEndedData{
        public GameObject GameObject;
        public CharacterTurnEndedData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class CharacterTurnEnded : UnityEvent<CharacterTurnEndedData>{}
}