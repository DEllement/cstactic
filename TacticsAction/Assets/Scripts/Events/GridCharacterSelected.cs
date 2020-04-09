using UnityEngine;
using UnityEngine.Events;

public class GridCharacterSelectedData{
    public GameObject GameObject;
    public GridCharacterSelectedData(GameObject gameObject){
        this.GameObject = gameObject;
    }
}
public class GridCharacterSelected : UnityEvent<GridCharacterSelectedData>{}