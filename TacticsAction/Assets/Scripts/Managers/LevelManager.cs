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

    }

    //Event Handlers
    

    public List<Character> Friends;
    public List<Character> Ennemies;
    
    public void SetupLevel(){
        Character c1 = new Character{Id=1, Class="Templar", Name="Landry de Lauzon", Level=2, Stats=new Stats{ HP=10, HPMax=10}, Inventory=new Inventory(), Equipments= new Equipments()};
        Character c2 = new Character{Id=2, Class="Monk", Name="Benedicte",Level=1, Stats=new Stats{ HP=10, HPMax=10}, Inventory=new Inventory(), Equipments= new Equipments()};
        Character c3 = new Character{Id=3, Class="Knight", Name="Marcus Aurelius", Level=5, Stats=new Stats{ HP=10, HPMax=10}, Inventory=new Inventory(), Equipments= new Equipments()};
        Ennemy e1 = new Ennemy{Id=4, Class="Soldier", Level=1, Stats=new Stats{HP=10, HPMax=10},Inventory=new Inventory(), Equipments= new Equipments()};
        Ennemy e2 = new Ennemy{Id=5, Class="Soldier", Level=1, Stats=new Stats{HP=10, HPMax=10},Inventory=new Inventory(), Equipments= new Equipments()};
        Ennemy e3 = new Ennemy{Id=6, Class="Soldier", Level=1, Stats=new Stats{HP=10, HPMax=10},Inventory=new Inventory(), Equipments= new Equipments()};
        
        Friends = new List<Character>{c1,c2,c3};
        Ennemies = new List<Character>{e1,e2,e3};
        
        //TODO: Get selected start position
        GameObject Character01 = GameObject.Find("Character01"); 
        GameObject Character02 = GameObject.Find("Character02");
        
        GameObject Ennemy01 = GameObject.Find("Ennemy01");
            
        GameCommands.AssignCharacterToGrid.Invoke(new AssignCharacterToGridData(Ennemy01, e1,9,9));  
        GameCommands.AssignCharacterToGrid.Invoke(new AssignCharacterToGridData(Character01, c1,5,5));  
        GameCommands.AssignCharacterToGrid.Invoke(new AssignCharacterToGridData(Character02,c3,0,1)); 
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
