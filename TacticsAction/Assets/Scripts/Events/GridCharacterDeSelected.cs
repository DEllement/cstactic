using UnityEngine;
using UnityEngine.Events;

public class GridCharacterDeSelectedData{
    public GameObject GameObject;
    public GridCharacterDeSelectedData(GameObject gameObject){
        this.GameObject = gameObject;
    }
}
public class GridCharacterDeSelected : UnityEvent<GridCharacterDeSelectedData>{}