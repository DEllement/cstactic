using Model;
using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class AssignCharacterToGridData{
        public GameObject GameObject;
        public Character Character;
        public int X;
        public int Y;
        
        public AssignCharacterToGridData(GameObject gameObject, Character character, int x, int y)
        {
            GameObject = gameObject;
            Character = character;
            X = x;
            Y = y;
        }
    }
    public class AssignCharacterToGrid : UnityEvent<AssignCharacterToGridData>{}
}
