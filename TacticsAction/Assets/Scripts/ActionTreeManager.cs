using System.Collections;
using System.Collections.Generic;
using API;
using API.Events;
using UnityEngine;

public class ActionTree{

}
public class ActionTreeItem{
    
}
public class ActionState{
    bool haveMoved;
    bool haveActed;
    
    
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
