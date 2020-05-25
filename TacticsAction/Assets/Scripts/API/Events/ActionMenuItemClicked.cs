using Model;
using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class ActionMenuItemClickedData{
        public ActionItem ActionItem;
        public ActionMenuItemClickedData(ActionItem actionItem)
        {
            ActionItem = actionItem;
        }
    }
    public class ActionMenuItemClicked : UnityEvent<ActionMenuItemClickedData>{}
}