using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class ActionMenuItemClickedData{
        public ActionType ActionType;
        public ActionMenuItemClickedData(ActionType actionType)
        {
            this.ActionType = actionType;
        }
    }
    public class ActionMenuItemClicked : UnityEvent<ActionMenuItemClickedData>{}
}