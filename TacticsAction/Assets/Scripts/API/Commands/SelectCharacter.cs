using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class SelectCharacterData{
        public GameObject GameObject;
        public SelectCharacterData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class SelectCharacter : UnityEvent<SelectCharacterData>{}
}