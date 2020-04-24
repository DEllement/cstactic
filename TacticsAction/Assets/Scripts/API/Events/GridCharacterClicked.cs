using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class GridCharacterClickedData{
        public GameObject GameObject;
        public GridCharacterClickedData(GameObject gameObject){
            this.GameObject = gameObject;
        }
    }
    public class GridCharacterClicked : UnityEvent<GridCharacterClickedData>{}
}