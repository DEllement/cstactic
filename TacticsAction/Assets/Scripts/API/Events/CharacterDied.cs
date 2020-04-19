using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class CharacterDiedData{
        public GameObject GameObject;
        public CharacterDiedData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class CharacterDied : UnityEvent<CharacterDiedData>{}
}