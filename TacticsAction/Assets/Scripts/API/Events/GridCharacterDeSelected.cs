using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class GridCharacterDeSelectedData{
        public GameObject GameObject;
        public GridCharacterDeSelectedData(GameObject gameObject){
            this.GameObject = gameObject;
        }
    }
    public class GridCharacterDeSelected : UnityEvent<GridCharacterDeSelectedData>{}
}