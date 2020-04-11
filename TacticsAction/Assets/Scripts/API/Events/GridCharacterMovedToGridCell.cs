using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class GridCharacterMovedToGridCellData{
        public int X; 
        public int Y;
        public GameObject GameObject;
        public GridCharacterMovedToGridCellData(int X, int Y, GameObject gameObject){
            this.GameObject = gameObject;
            this.X = X;
            this.Y = Y;
        }
    }
    public class GridCharacterMovedToGridCell : UnityEvent<GridCharacterMovedToGridCellData>{}
}