﻿using System;
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
        GameEvents.GridCharacterSelected.AddListener(OnSelected);
        GameEvents.GridCharacterDeSelected.AddListener(OnDeSelected);
        GameCommands.MoveGridCharacter.AddListener(Execute);
    }

    private void OnSelected(GridCharacterSelectedData data)
    {
        if( data.GameObject != this.gameObject )
            return;
        
        this.GetComponent<Renderer>().material.color = Color.blue;
    }
    private void OnDeSelected(GridCharacterDeSelectedData data)
    {
        if( data.GameObject != this.gameObject )
            return;
        
        this.GetComponent<Renderer>().material.color = Color.green;
    }
    
    public float moveSpeed = 2.0f;
    bool isMoving;
    GridPath[] movePath;
    int currPathIndex;

    private void Execute(MoveGridCharacterData data){
        if(data.Path == null || data.Path.Length == 0)
            return;
            
        movePath = data.Path;
        currPathIndex = 0;
        isMoving = true;
    }
    Vector3 lastPath;
    // Update is called once per frame
    void Update()
    {
        if(isMoving){
            X = movePath[currPathIndex].X;
            Y = movePath[currPathIndex].Y;
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
        
        if(++currPathIndex >= movePath.Length){
            isMoving = false;
        }
    }
}
