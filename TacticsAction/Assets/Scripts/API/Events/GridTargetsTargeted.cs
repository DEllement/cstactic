using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class GridTargetsTargetedData{
        public List<GameObject> GridCells;
        public GridTargetsTargetedData(List<GameObject> gameObjects)
        {
            this.GridCells = gameObjects;
        }
    }
    public class GridTargetsTargeted : UnityEvent<GridTargetsTargetedData>{}
}