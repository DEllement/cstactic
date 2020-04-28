using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class GridTargetsSelectedData{
        public List<GameObject> GridCells;
        public GridTargetsSelectedData(List<GameObject> gridCells)
        {
            this.GridCells = gridCells;
        }
    }
    public class GridTargetsSelected : UnityEvent<GridTargetsSelectedData>{}
}