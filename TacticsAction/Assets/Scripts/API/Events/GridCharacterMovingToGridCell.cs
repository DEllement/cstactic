using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class GridCharacterMovingToGridCellData{
        public int X; 
        public int Y;
        public GameObject GameObject;
        public GridCharacterMovingToGridCellData(int X, int Y,GameObject gameObject){
            this.GameObject = gameObject;
            this.X = X;
            this.Y = Y;
        }
    }
    public class GridCharacterMovingToGridCell : UnityEvent<GridCharacterMovingToGridCellData>{}
}