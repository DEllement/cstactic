using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class HideActionsMenuData{
        public GameObject GameObject;
        public HideActionsMenuData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class HideActionsMenu : UnityEvent<HideActionsMenuData>{}
}