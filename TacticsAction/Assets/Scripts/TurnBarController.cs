using System;
using System.Collections;
using System.Collections.Generic;
using API;
using UnityEngine;

using API.Events;
using Model;
using UnityEngine.UI;

public class TurnBarController : MonoBehaviour
{
    private List<Character> lineUp;

    private Dictionary<int, GameObject> idxToItems= new Dictionary<int, GameObject>();
    public GameObject TurnBarItem;
    public int PaddingLeft;
    public float MoveSpeed=1f;
    
    // Start is called before the first frame update
    void Start()
    {
        print("TurnBarController::start");
        idxToItems = new Dictionary<int, GameObject>();
        
        GameEvents.TurnManagerInitialized.AddListener(Handle);
        GameEvents.TurnManagerLineUpChanged.AddListener(Handle);
        
        GameEvents.CharacterTurnStarted.AddListener(Handle);
        GameEvents.CharacterTurnEnded.AddListener(Handle);
        GameEvents.CharacterDied.AddListener(Handle);
        
        GameEvents.GridCharacterSelected.AddListener(Handle);
        GameEvents.GridCharacterDeSelected.AddListener(Handle);
        
        GameEvents.TurnBarReady.Invoke();
    }

    bool moveItems;
    float moveDelta=0;
    // Update is called once per frame
    GameObject lastItem = null;
    void Update()
    {
        if(moveItems && lineUp != null){
            float step =  MoveSpeed * Time.deltaTime;
            moveDelta += Time.deltaTime;
            for(var i=0; i< lineUp.Count; i++){
                var item = idxToItems[lineUp[i].Id];
                var pos = item.transform.position;
                pos.x = PaddingLeft+ i * 50;
                if(lastItem != null)
                    item.transform.SetSiblingIndex(lastItem.transform.GetSiblingIndex());
                lastItem = item;
                item.transform.position = Vector3.Lerp(item.transform.position, pos,step);
                if(i == 0)
                    item.transform.localScale = Vector3.Lerp(item.transform.localScale, new Vector3(.75f,.75f,1f), step);
                else
                    item.transform.localScale = new Vector3(.5f,.5f,1f);
            }
            var firstItem = idxToItems[lineUp[0].Id];
            if(Math.Abs(firstItem.transform.position.x - PaddingLeft) < 0.001)
                moveItems = false;
            
        }
    }
    
    //Events Handler
    
    private void Handle(TurnManagerInitializedData data){
       
        print("TurnManagerInitializedData");
        lineUp = data.LineUp;
        
        //Create TurnItem instances
        for(var i=0; i< lineUp.Count; i++){
            var c = lineUp[i];
            var item = Instantiate(TurnBarItem, transform);
            item.transform.SetParent(gameObject.transform);
            var pos = item.transform.position;
            pos.x = PaddingLeft+ i * 50;
            item.transform.position = pos;
            if( c.IsEnnemy )
                item.GetComponent<Image>().color = Color.red;
            else
                item.GetComponent<Image>().color = Color.green;
            
            if( lastItem != null )
                item.transform.SetSiblingIndex(lastItem.transform.GetSiblingIndex());
            lastItem = item;
            
            item.name = "turnItem"+c.Id;
            item.GetComponentInChildren<Text>().text = c.Id.ToString();
            idxToItems.Add(c.Id, item);
            if(i==0)
                item.transform.localScale = new Vector3(.75f,.75f,1f);
            else
                item.transform.localScale = new Vector3(.5f,.5f,1f);
        }
    }
    private void Handle(TurnManagerLineUpChangedData data){

        lineUp = data.LineUp;
        moveItems = true;
    }
    private void Handle(CharacterDiedData data){
    
    }
    private void Handle(CharacterTurnStartedData data){
    
    }
    private void Handle(CharacterTurnEndedData data){
    
    }
    private void Handle(GridCharacterSelectedData data){
    
    }
    private void Handle(GridCharacterDeSelectedData data){
    
    }
}
