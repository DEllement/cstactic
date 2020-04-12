using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class GridCharacterDoneMovingData{
        public int X; 
        public int Y;
        public GameObject GameObject;
        public GridCharacterDoneMovingData(int X, int Y,GameObject gameObject){
            this.GameObject = gameObject;
            this.X = X;
            this.Y = Y;
        }
    }
    public class GridCharacterDoneMoving : UnityEvent<GridCharacterDoneMovingData>{}
}