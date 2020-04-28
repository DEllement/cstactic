using System;
using System.Collections;
using System.Collections.Generic;
using API;
using API.Commands;
using API.Events;
using Model;
using UnityEngine;

public class GridCharacterController : MonoBehaviour
{
    public int X;
    public int Y;
    public (int x, int y) GridPosition => (X,Y);
    public Character Character;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Select()
    {
        this.GetComponent<Renderer>().material.color = Color.blue;
        GameEvents.GridCharacterClicked.Invoke(new GridCharacterClickedData(this.gameObject));
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

    public void MoveGridCharacter(GridPath[] path)
    {
        if(path == null || path.Length == 0)
            return;
            
        movePath = path;
        //movePath. transform.position;
        currPathIndex = 0;
        isMoving = true;
    }
    
    Vector3 lastPath;
    // Update is called once per frame
    void Update()
    {
        if(isMoving){
            
            float step =  moveSpeed * Time.deltaTime;
            //print(X + " " + Y);
            transform.position = Vector3.MoveTowards(transform.position, movePath[currPathIndex].Position + new Vector3(0,.5f,0), step);
            if( Vector3.Distance(transform.position, movePath[currPathIndex].Position + new Vector3(0,.5f,0)) < 0.001f)
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
