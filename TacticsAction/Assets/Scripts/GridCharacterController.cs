using System;
using System.Collections;
using System.Collections.Generic;
using API;
using API.Commands;
using API.Events;
using UnityEngine;

public class GridCharacterController : MonoBehaviour
{
    public int X;
    public int Y;
    
    // Start is called before the first frame update
    void Start()
    {
        GameCommands.DeSelectCharacter.AddListener(Execute);
        GameCommands.MoveGridCharacter.AddListener(Execute);
    }

    public void Select()
    {
        this.GetComponent<Renderer>().material.color = Color.blue;
        GameEvents.GridCharacterSelected.Invoke(new GridCharacterSelectedData(this.gameObject));
    }
    public void DeSelect()
    {
        this.GetComponent<Renderer>().material.color = CompareTag("Ennemy") ? Color.red : Color.green;
        GameEvents.GridCharacterDeSelected.Invoke(new GridCharacterDeSelectedData(this.gameObject));
    }
    
    public float moveSpeed = 2.0f;
    bool isMoving;
    GridPath[] movePath;
    int currPathIndex;

    private void Execute(MoveGridCharacterData data){
        if(data.CharacterGameObject != gameObject)
            return;
        if(data.Path == null || data.Path.Length == 0)
            return;
            
        movePath = data.Path;
        //movePath. transform.position;
        currPathIndex = 0;
        isMoving = true;
    }
    private void Execute(DeSelectCharacterData data){
        if( data.CharacterGameObject != this.gameObject )
            return;
        
        DeSelect();
    }
    
    Vector3 lastPath;
    // Update is called once per frame
    void Update()
    {
        if(isMoving){
            
            float step =  moveSpeed * Time.deltaTime;
            //print(X + " " + Y);
            transform.position = Vector3.MoveTowards(transform.position, movePath[currPathIndex].Position, step);
            if( Vector3.Distance(transform.position, movePath[currPathIndex].Position) < 0.001f)
            {
                SetNextPath();
            }
        }
        if( movePath != null && movePath.Length > 0){
            lastPath = movePath[0].Position;
            for(var i =1;i < movePath.Length; i++)
            {
                Debug.DrawLine(lastPath+(Vector3.up*.5f), movePath[i].Position+Vector3.up*.5f, Color.red);   
                lastPath=movePath[i].Position;
            }
        }
    }
    void SetNextPath() {
        GameEvents.GridCharacterLeavingGridCell.Invoke(new GridCharacterLeavingGridCellData(X,Y, this.gameObject));
        X = movePath[currPathIndex].X;
        Y = movePath[currPathIndex].Y;
        GameEvents.GridCharacterMovedToGridCell.Invoke(new GridCharacterMovedToGridCellData(X,Y, this.gameObject));
        if(currPathIndex+1 >= movePath.Length){
            isMoving = false;
            GameEvents.GridCharacterDoneMoving.Invoke(new GridCharacterDoneMovingData(X,Y, this.gameObject));
        }else{
            currPathIndex++;
            GameEvents.GridCharacterMovingToGridCell.Invoke(new GridCharacterMovingToGridCellData(movePath[currPathIndex].X,movePath[currPathIndex].Y, this.gameObject));
        }
    }
}
