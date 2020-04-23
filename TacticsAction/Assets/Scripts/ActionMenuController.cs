using System;
using System.Collections;
using System.Collections.Generic;
using API;
using API.Commands;
using API.Events;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenuController : MonoBehaviour
{
    public GameObject ActionMenuItem;
    
    private ActionType OverAction;
    private List<ActionType> CurrentActionPath; 
    
    //Life Cycle
    void Start()
    {
        GameEvents.GridCharacterSelected.AddListener(Handle);
        GameEvents.GridCharacterDeSelected.AddListener(Handle);
        GameCommands.ShowActionsMenu.AddListener(Execute); 
        GameCommands.HideActionsMenu.AddListener(Execute);
    }
    
    Camera mainCam;
    
    void Update()
    {
        if(mainCam == null)
            mainCam = Camera.main;
        
        if(this.gameObject.activeSelf && target != null)
           this.gameObject.transform.position = mainCam.WorldToScreenPoint( target.transform.position + new Vector3(0f,0f,0f));
    }

    GameObject target;
    
    //Events Handlers
    private void Handle(GridCharacterSelectedData data)
    {
        target = data.GameObject;
        this.gameObject.SetActive(true);
        CreateMenu();
    }
    private void Handle(GridCharacterDeSelectedData arg0)
    {
        //this.gameObject.SetActive(false);
    }

    
    // Commands Handlers
    private void Execute(ShowActionsMenuData menuData)
    {
        if(this.gameObject.activeSelf)
            return;
        
        OverAction = ActionType.Move;
        CurrentActionPath = new List<ActionType>();
        CreateMenu();
        this.gameObject.SetActive(true);
    }
    private void Execute(HideActionsMenuData menuData)
    {
        this.gameObject.SetActive(false);
    }
    
    public int MarginBetweenBtn = 30;
    public int MarginLeft = 150;
    public int MarginTop = 50;
    
    private void CreateMenu(){
        
        for(var j = 0; j < this.gameObject.transform.childCount; j++){
            if(!this.gameObject.transform.GetChild(j).CompareTag("DebugLabel"))
                Destroy(this.gameObject.transform.GetChild(j).gameObject);
        }
        
        float i = 0f;
        ActionTreeManager.instance.Actions.ForEach(item=>{
            var actionBtn = Instantiate(ActionMenuItem, transform);
            var itemCtrl = actionBtn.AddComponent<ActionMenuItemController>();
            itemCtrl.ActionItem = item;
            actionBtn.transform.localPosition = new Vector3(MarginLeft, (i * -MarginBetweenBtn) + MarginTop);
            
            if( item.Children != null){
                item.Children()?.ForEach(subItem=>{
                    var actionBtn2 = Instantiate(ActionMenuItem, transform);
                    var itemCtrl2 = actionBtn2.AddComponent<ActionMenuItemController>();
                    itemCtrl2.ActionItem = subItem;
                    actionBtn2.transform.localPosition = new Vector3(MarginLeft+200f, (i * -MarginBetweenBtn) + MarginTop);    
                
                });            
            }
            
            i++;
            
        });
    }
}
