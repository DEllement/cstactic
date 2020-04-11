using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class GridCharacterLeavingGridCellData{
        public int X; 
        public int Y;
        public GameObject GameObject;
        public GridCharacterLeavingGridCellData(int X, int Y,GameObject gameObject){
            this.GameObject = gameObject;
            this.X = X;
            this.Y = Y;
        }
    }
    public class GridCharacterLeavingGridCell : UnityEvent<GridCharacterLeavingGridCellData>{}
}