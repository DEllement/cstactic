using System;
using System.Collections;
using System.Collections.Generic;
using API;
using API.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionMenuItemController : MonoBehaviour
{
    public ActionItem ActionItem;
    
    public List<GameObject> ChildMenuItems { get; set; }

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
    
    bool showChild;
    private void OnClick()
    {
        if(ChildMenuItems != null){
            showChild = !showChild;
            ShowHideChildItems(showChild);
        }
        else if(ActionItem.Enabled())
            GameEvents.ActionMenuItemClicked.Invoke(new ActionMenuItemClickedData(ActionItem.ActionType));
    }
    
    public void ShowHideChildItems(bool active){
        ChildMenuItems?.ForEach(cmi=>{
            cmi.SetActive(active);
        });
    }
    
}
