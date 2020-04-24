using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class GridCellClickedData{
        public GameObject GameObject;
        public (int X, int Y) GridPosition;
        public GridCellClickedData(GameObject gameObject, (int X, int Y) gridPosition)
        {
            this.GameObject = gameObject;
            this.GridPosition = gridPosition;
        }
    }
    public class GridCellClicked : UnityEvent<GridCellClickedData>{}
}