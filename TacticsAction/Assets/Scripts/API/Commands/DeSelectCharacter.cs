using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class DeSelectCharacterData{
        
        public GameObject CharacterGameObject;
        public DeSelectCharacterData(GameObject characterGameObject)
        {
            CharacterGameObject = characterGameObject;
        }
    }
    public class DeSelectCharacter : UnityEvent<DeSelectCharacterData>{}
}