using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using API;
using API.Commands;
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

public class CharacterTurnStarted{
}
public class CharacterTurnEnded{
}

public class TurnManager {
    public Queue<Character> lineUp;
    public int Round;
    List<Character> friendly;
    List<Character> ennemies;
    
    //Computed Properties
    public int TotalFriendlyAlive => friendly.Count(x=>x.Stats.HP > 0);
    public int TotalEnnemiesAlive => ennemies.Count(x=>x.Stats.HP > 0);
    List<Character> FriendlyUnitsAlive => friendly.Where(x=>x.Stats.HP > 0).ToList();
    List<Character> EnnemiesUnitsAlive => ennemies.Where(x=>x.Stats.HP > 0).ToList();
    
    public TurnManager()
    {
        this.lineUp = new Queue<Character>();
    }

    public void Init(List<Character> friendly, List<Character> ennemies){
        
        this.friendly = friendly;
        this.ennemies = ennemies;
        
        
        var i_f = 0;
        var i_e = 0;
        
        var friendlyAlive = FriendlyUnitsAlive;
        var ennemiesAlive = EnnemiesUnitsAlive;
        
        var totalFriendlyAlive = friendlyAlive.Count;
        var totalEnnemiesAlive = ennemiesAlive.Count;
        var totalCharactersAlive = totalFriendlyAlive+totalEnnemiesAlive;
        
        
        for(var i=0; i < totalCharactersAlive; i++){
            //Friendly
            if(i%2==0){
                if(i_f++>=totalFriendlyAlive)
                    continue;
                lineUp.Enqueue(friendlyAlive[i_f]);  
            }
            //Ennemy
            else{
                if(i_e++>=totalEnnemiesAlive)
                    continue;
                lineUp.Enqueue(ennemiesAlive[i_e]);    
            }
        }
    }
    
    public void Next(){
    
        var character = lineUp.Dequeue();
        if (character == null)
            return; //Dispatch: GameEvents.AllCharactersDied.Invoke();
        
        if (character.Stats.HP <= 0){
            Next();
            return;
        }

        lineUp.Enqueue(character);
        
        if(character.IsEnnemy || character.IsGuest || character.AutoCombat)
            GameCommands.ExecuteCharacterTurn.Invoke(null);
        else
            GameCommands.SelectCharacter.Invoke(null);
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
    }

    //Event Handlers
    
    private void OnGridReady()
    {
        Character c1 = new Character();
        Character c2 = new Character();
        Ennemy e1 = new Ennemy();

        var turnManager = new TurnManager();
        turnManager.Init(new List<Character>{c1,c2}, new List<Character>{e1} );
        
        
        //TODO: Get selected start position
        GameObject Character01 = GameObject.Find("Character01");
        GameObject Character02 = GameObject.Find("Character02");
        
        GameObject Ennemy01 = GameObject.Find("Ennemy01");
            
        GameCommands.AssignCharacterToGrid.Invoke(new AssignCharacterToGridData(Ennemy01,9,9));  
        GameCommands.AssignCharacterToGrid.Invoke(new AssignCharacterToGridData(Character01,5,5));  
        GameCommands.AssignCharacterToGrid.Invoke(new AssignCharacterToGridData(Character02,0,1));  
    
    
        //After the level started
        
        //TurnManager.Init();
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
