using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class MoveGridCharacterData{
        
        public GameObject CharacterGameObject;
        public GridPath[] Path;
        
        public MoveGridCharacterData(GameObject characterGameObject, GridPath[] path)
        {
            CharacterGameObject = characterGameObject;
            Path = path;
        }
    }
    public class MoveGridCharacter : UnityEvent<MoveGridCharacterData>{}
}