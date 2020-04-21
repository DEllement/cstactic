using System.Collections;
using System.Collections.Generic;
using API;
using API.Events;
using UnityEngine;

public enum ActionType
{
    Move,
    Act,
    Wait,
}

public class ActionTreeProvider
{
    private static ActionItem Move = new ActionItem();
    
    public List<ActionItem> Get(ActionState _state)
    {
        
        
        return new List<ActionItem>{
            Move,   
        };


    }
}

public class ActionItem
{
    public ActionState _state;
    public List<ActionItem> Children;
    public ActionType ActionType;
}
public class ActionState{
    bool haveMoved;
    bool haveActed;
    private Character character;
    public List<ActionItem> Actions;
}

public class ActionTreeManager : MonoBehaviour
{
    private Dictionary<int, Character> characters;
    
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.CharacterTurnStarted.AddListener(Handle);
        GameEvents.CharacterTurnEnded.AddListener(Handle);
    }

    private void Handle(CharacterTurnStartedData data)
    {
        
    }
    private void Handle(CharacterTurnEndedData data){
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
