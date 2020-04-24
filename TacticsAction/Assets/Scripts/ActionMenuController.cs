using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using API;
using API.Commands;
using API.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ActionMenuController : MonoBehaviour
{
    public GameObject ActionMenuItem;
    public int MarginBetweenBtn = 30;
    public int MarginLeft = 150;
    public int MarginTop = 50;
    
    private ActionType OverAction;
    private List<ActionType> CurrentActionPath; 
    private Camera mainCam;
    private GameObject target;
    //Life Cycle
    void Start()
    {
        GameEvents.GridCharacterSelected.AddListener(Handle);
        GameEvents.GridCharacterDeSelected.AddListener(Handle);
        GameEvents.ActionMenuItemClicked.AddListener(Handle);
        GameCommands.ShowActionsMenu.AddListener(Execute); 
        GameCommands.HideActionsMenu.AddListener(Execute);
    }
    void Update()
    {
        if( mainCam == null)
            mainCam = Camera.main;
        
        if(this.gameObject.activeSelf && target != null)
           this.gameObject.transform.position = mainCam.WorldToScreenPoint( target.transform.position + new Vector3(0f,0f,0f));
        
        if(Input.GetMouseButtonUp(0)){
            if(!EventSystem.current.IsPointerOverGameObject())
                Execute(new HideActionsMenuData(null));
        }
    }

    //Events Handlers
    private void Handle(GridCharacterSelectedData data)
    {
        if(target == data.GameObject && this.gameObject.activeSelf)
            return;
        
        target = data.GameObject;
        Execute(new ShowActionsMenuData(null));
    }
    private void Handle(GridCharacterDeSelectedData arg0)
    {
        //this.gameObject.SetActive(false);
    }
    private void Handle(ActionMenuItemClickedData arg0)
    {
        Execute(new HideActionsMenuData(null));
    }
    
    // Commands Handlers
    private void Execute(ShowActionsMenuData menuData)
    {
        OverAction = ActionType.Move;
        CurrentActionPath = new List<ActionType>();
        CreateMenu();
        this.gameObject.SetActive(true);
        GameEvents.ActionMenuOpened.Invoke();
    }
    private void Execute(HideActionsMenuData menuData)
    {
        this.gameObject.SetActive(false);
        GameEvents.ActionMenuClosed.Invoke();
    }
    
    private List<GameObject> MenuItems;
    private void CreateMenu(){
        MenuItems?.ForEach(obj=>{
            Destroy(obj);
        });
        MenuItems = CreateMenuItems(ActionTreeManager.instance.Actions, new Vector3(MarginLeft, MarginTop), transform, true);
    }
    
    private List<GameObject> CreateMenuItems(List<ActionItem> items, Vector3 startPos, Transform parent, bool active){
        float i = 0f;
        return items?.Select(item=>{
            var actionBtn = Instantiate(ActionMenuItem, parent);
            actionBtn.SetActive(active);
            var itemCtrl = actionBtn.AddComponent<ActionMenuItemController>();
            itemCtrl.ActionItem = item;
            actionBtn.transform.localPosition = startPos+new Vector3(0, (i * -MarginBetweenBtn));
                
            if( item.Children != null){
                itemCtrl.ChildMenuItems = CreateMenuItems(item.Children(), new Vector3(175f,0), actionBtn.transform, false);
            }
            i++;
            
            return actionBtn;
            
        }).ToList();
    }
}
