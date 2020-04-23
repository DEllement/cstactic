using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using API;
using API.Events;
using Model;
using UnityEngine;

public enum ActionType
{
    Move,
    Act,
        Attack,
            Melee,
            Ranged,
        Magic,
            White,
            Grey,
            Black,
    Inventory,
        Scrolls,
        Consumable,
    Wait,
}

public class ActionItem
{
    public Func<List<ActionItem>> Children = ()=>null;
    public ActionType ActionType;
    public Func<bool> Enabled = ()=>true;
    public Func<bool> Visible = ()=>true;
    public bool Executable;
}
public class ActionState{
    public bool HaveMoved;
    public bool HaveActed;
    public Character Character;
}

public class ActionTreeManager : MonoBehaviour
{
    public static ActionTreeManager instance;
    
    private Dictionary<int, Character> characters;
    
    private Character CurrentCharacterTurn;
    private ActionState ActionState;
    public List<ActionItem> Actions;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        GameEvents.CharacterTurnStarted.AddListener(Handle);
        GameEvents.CharacterTurnEnded.AddListener(Handle);
        GameEvents.ActionMenuItemClicked.AddListener(Handle);
    }

    private void Handle(ActionMenuItemClickedData arg0)
    {
    }

    private void Handle(CharacterTurnStartedData data)
    {
        CurrentCharacterTurn = data.Character;
        ActionState = new ActionState{
            Character = CurrentCharacterTurn
        };
        Actions = GetSelfActions(()=>ActionState);
    }
    private void Handle(CharacterTurnEndedData data){
        CurrentCharacterTurn = null;
    }
    
    //TODO: Handle other character or tile selection cause action tree will need to change accordingly

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //Factory
    
    //TODO: Terrain Selection Action Tree
    //TODO: Ennemy Selection Action Tree
    
    //Self Selection Action Tree
    public List<ActionItem> GetSelfActions(Func<ActionState> _state)
    {
        return new List<ActionItem>{
            new ActionItem{ActionType = ActionType.Move,  Enabled=()=>!_state().HaveMoved, Executable=true },
            new ActionItem{ActionType = ActionType.Act,   Enabled=()=>!_state().HaveActed,
                Children= ()=> new List<ActionItem>{
                    new ActionItem{ ActionType=ActionType.Attack, 
                        Children=()=> new List<ActionItem>{
                            new ActionItem{ ActionType=ActionType.Melee, Children=null, Executable=true  },
                            new ActionItem{ ActionType=ActionType.Ranged, Children=null, Executable=true  }
                        }},
                    new ActionItem{ ActionType=ActionType.Magic  }
                },
            },
            new ActionItem{ActionType = ActionType.Inventory, Enabled=()=>!_state().HaveActed, 
                Children= ()=> _state().Character.Inventory.GetConsumables().Select(x=>new ActionItem{}).ToList(),  },
            new ActionItem{ActionType = ActionType.Wait, Children=null},
        };
    }
}
