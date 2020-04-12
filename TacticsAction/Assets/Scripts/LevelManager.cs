using System.Collections;
using System.Collections.Generic;
using API;
using API.Commands;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.GridReady.AddListener(OnGridReady);
    }

    //Event Handlers
    
    private void OnGridReady()
    {
        //TODO: Get selected start position
        GameObject Character01 = GameObject.Find("Character01");
        GameObject Character02 = GameObject.Find("Character02");
        
        GameCommands.AssignCharacterToGrid.Invoke(new AssignCharacterToGridData(Character01,5,5));  
        GameCommands.AssignCharacterToGrid.Invoke(new AssignCharacterToGridData(Character02,0,1));  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
