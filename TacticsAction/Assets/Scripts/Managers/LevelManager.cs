using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using API;
using API.Commands;
using API.Events;
using Model;
using UnityEngine;



public class LevelManager : MonoBehaviour
{
    public Character[] characters;
    public Ennemy[] ennemies;
    
    // Life Cycle
    void Start()
    {
        GameEvents.GridReady.AddListener(OnGridReady);
        GameEvents.TurnBarReady.AddListener(OnTurnBarReady);
    }

    //Event Handlers
    bool gridReady, turnBarReady, setupStarted;
    bool CanSetupLevel => gridReady && turnBarReady && !setupStarted;
    
    private void OnGridReady()
    {
        gridReady = true;
        if(CanSetupLevel)
            SetupLevel();
    }
    private void OnTurnBarReady(){
        turnBarReady = true;
        if(CanSetupLevel)
            SetupLevel();
    }

    private void SetupLevel(){
        setupStarted = true;
        Character c1 = new Character{Id=1, Stats=new Stats{HP=10}, Inventory=new Inventory()};
        Character c2 = new Character{Id=2, Stats=new Stats{HP=10}, Inventory=new Inventory()};
        Character c3 = new Character{Id=3, Stats=new Stats{HP=10}, Inventory=new Inventory()};
        Ennemy e1 = new Ennemy{Id=4, Stats=new Stats{HP=10},Inventory=new Inventory()};
        Ennemy e2 = new Ennemy{Id=5, Stats=new Stats{HP=10},Inventory=new Inventory()};
        Ennemy e3 = new Ennemy{Id=6, Stats=new Stats{HP=10},Inventory=new Inventory()};
        
        var turnManager = new TurnManager();
        turnManager.Init(new List<Character>{c1,c2,c3}, new List<Character>{e1,e2,e3} );
        
        //TODO: Get selected start position
        GameObject Character01 = GameObject.Find("Character01");
        GameObject Character02 = GameObject.Find("Character02");
        
        GameObject Ennemy01 = GameObject.Find("Ennemy01");
            
        GameCommands.AssignCharacterToGrid.Invoke(new AssignCharacterToGridData(Ennemy01,9,9));  
        GameCommands.AssignCharacterToGrid.Invoke(new AssignCharacterToGridData(Character01,5,5));  
        GameCommands.AssignCharacterToGrid.Invoke(new AssignCharacterToGridData(Character02,0,1)); 
        
        //TODO: Execute intro animation/scene
        //TODO: Show Level Name
        //TODO: Show Objectives
        //Start Turn
        turnManager.Next();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
