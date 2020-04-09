using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class AssignCharacterToGridData{
        public GameObject GameObject;
        public int X;
        public int Y;
        
        public AssignCharacterToGridData(GameObject gameObject, int x, int y)
        {
            GameObject = gameObject;
            X = x;
            Y = y;
        }
    }
    public class AssignCharacterToGrid : UnityEvent<AssignCharacterToGridData>{}
}
