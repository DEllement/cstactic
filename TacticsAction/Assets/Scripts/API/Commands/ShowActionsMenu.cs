using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class ShowActionsMenuData{
        public GameObject GameObject;
        public ShowActionsMenuData(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
    public class ShowActionsMenu : UnityEvent<ShowActionsMenuData>{}
}