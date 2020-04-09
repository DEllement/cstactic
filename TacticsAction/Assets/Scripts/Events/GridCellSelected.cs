using UnityEngine;
using UnityEngine.Events;

public class GridCellSelectedData{
    public GameObject GameObject;
    public GridCellSelectedData(GameObject gameObject)
    {
        this.GameObject = gameObject;
    }
}
public class GridCellSelected : UnityEvent<GridCellSelectedData>{}