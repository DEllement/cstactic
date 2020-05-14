using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class GridOutsideTargetRangeClickedData{
        public GameObject GameObject;
        public GridOutsideTargetRangeClickedData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class GridOutsideTargetRangeClicked : UnityEvent<GridOutsideTargetRangeClickedData>{}
}