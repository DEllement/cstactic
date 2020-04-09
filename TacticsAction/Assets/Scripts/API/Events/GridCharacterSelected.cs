using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class GridCharacterSelectedData{
        public GameObject GameObject;
        public GridCharacterSelectedData(GameObject gameObject){
            this.GameObject = gameObject;
        }
    }
    public class GridCharacterSelected : UnityEvent<GridCharacterSelectedData>{}
}