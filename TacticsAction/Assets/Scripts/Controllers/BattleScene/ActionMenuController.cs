using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using API;
using API.Commands;
using API.Events;
using Model;
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
    public GameObject target;
    
    private ActionType _overAction;
    private List<ActionType> _currentActionPath; 
    private Camera _mainCam;
    private List<GameObject> _menuItems;
    //Life Cycle
    void Start()
    {
    }
    void Update()
    {
        if( _mainCam == null)
            _mainCam = Camera.main;
        
        if(this.gameObject.activeSelf && target != null)
           this.gameObject.transform.position = _mainCam.WorldToScreenPoint( target.transform.position + new Vector3(0f,0f,0f));
    }

    public void ShowActionsMenu(){
        _overAction = ActionType.Move;
        _currentActionPath = new List<ActionType>();
        CreateMenu();
        this.gameObject.SetActive(true);
        Update(); //This fix the mini ui glich, remove once ActionMenu will not be a follow gameobject
        GameEvents.ActionMenuOpened.Invoke();
    }
    public void CloseActionMenu(){
        this.gameObject.SetActive(false);
        GameEvents.ActionMenuClosed.Invoke();
    }
    
    private void CreateMenu(){
        _menuItems?.ForEach(obj=>{
            Destroy(obj);
        });
        _menuItems = CreateMenuItems(ActionTreeManager.instance.Actions, new Vector3(MarginLeft, MarginTop), transform, true);
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
