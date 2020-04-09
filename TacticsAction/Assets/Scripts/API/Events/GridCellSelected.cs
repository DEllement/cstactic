using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class GridCellSelectedData{
        public GameObject GameObject;
        public GridCellSelectedData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class GridCellSelected : UnityEvent<GridCellSelectedData>{}
}