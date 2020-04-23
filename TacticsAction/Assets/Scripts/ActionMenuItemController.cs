using System;
using System.Collections;
using System.Collections.Generic;
using API;
using API.Events;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenuItemController : MonoBehaviour
{
    public ActionItem ActionItem;
    void Start()
    {
        var btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
        
        btn.GetComponentInChildren<Text>().text = ActionItem.ActionType.ToString();
        //TODO: Assign Image
    }
    void Update()
    {
        this.GetComponent<Button>().enabled = ActionItem.Enabled();
    }
    
    private void OnClick()
    {
        if(ActionItem.Children != null){
        
        }
        else if(ActionItem.Enabled())
            GameEvents.ActionMenuItemClicked.Invoke(new ActionMenuItemClickedData(ActionItem.ActionType));
    }
    
    private void OnMouseEnter()
    {
        //GameEvents.ActionMenuItemClicked.Invoke();
    }

    private void OnMouseExit()
    {
        //GameEvents.ActionMenuItemClicked.Invoke();
    }
}
