using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using API;
using API.Commands;
using API.Events;
using UnityEngine;

public class CharacterStatus{}

public class Aura{}
public class Curse{}

public class Stats{
    
    public int Level; //Level
    public int Xp;
    
    public int HP;
    public int MP;
    
    public int STR; //Affects melee PHYSICAL ATTACK & HP
    public int INT; //Affects the strength of OFFENSIVE MAGIC & MP
    public int DEX; //Affects melee and ranged ACCURACY and ATTACK. Affect MVP & Ranged ATK 
    
    public int MVP; //Affects the ability to move pts
    public int VIT; //Affects DEFENSE against attacks
    public int AGI; //Affects the ACCURACY of attacks.
    public int AVD; //Affects the ability to AVOID ATTACKS while defending.
    public int MND; //Affects the success of SPECIAL SKILLS and offensive and defensive MAGIC.
    public int RES; //Affects the ability to RESIST MAGIC.
    public int LUC; //Affects the chance to DROP ITEMS.
    
    public int ATK;
    public int DEF;
    
    public Aura[] Auras;
    public Curse[] Curses;
    
    public void AddXp(int xp, int foeLevel){
        Xp += xp;
        
        var xpMultiplier = GetXpMultiplier(foeLevel);
        
        //TODO: Check if ready to levelup
    }
    public int XpUntilNextLevel{get;set;}
    public double GetXpMultiplier(int foeLevel){
        int safeZone = (int)Math.Floor( (3+ (decimal)Level/16));
        double effDiff = Math.Max( Math.Abs(Level-foeLevel)-safeZone, 0);
        var xpMultiplier = 0.0;
        if(Level > 95)
            xpMultiplier = Math.Max( Math.Pow( (Level+5)/(Level+5+ Math.Pow(effDiff,2.5)),1.5 ), 0.01);
        else //Math.round(((Math.pow((lvl+5),1.5)) / (Math.pow((lvl+5+(Math.pow(effectivediff,2.5))),1.5)))*10000);
            xpMultiplier = Math.Round(((Math.Pow((Level+5),1.5)) / (Math.Pow((Level+5+(Math.Pow(effDiff,2.5))),1.5)))*10000);
        return xpMultiplier;
    }

    public void ComputeStats(){
        
        
    }    
}

public class Character
{
    public int Id;
    public string Name;
    public int Level;
    public bool IsEnnemy;
    public bool IsGuest;
    public bool AutoCombat;
    
    //Stats
    public Stats Stats;
}
public class Ennemy : Character{
    public Ennemy()
    {
        IsEnnemy = true;
    }
}


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
        Character c1 = new Character{Id=1, Stats=new Stats{HP=10}};
        Character c2 = new Character{Id=2, Stats=new Stats{HP=10}};
        Character c3 = new Character{Id=3, Stats=new Stats{HP=10}};
        Ennemy e1 = new Ennemy{Id=4, Stats=new Stats{HP=10}};
        Ennemy e2 = new Ennemy{Id=5, Stats=new Stats{HP=10}};
        Ennemy e3 = new Ennemy{Id=6, Stats=new Stats{HP=10}};
        
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
