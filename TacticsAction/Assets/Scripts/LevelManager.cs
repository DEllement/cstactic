using System.Collections;
using System.Collections.Generic;
using API;
using API.Commands;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject Character01;
    
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.GridReady.AddListener(OnGridReady);
    }

    //Event Handlers
    
    private void OnGridReady()
    {
        //TODO: Get selected start position
        
        GameCommands.AssignCharacterToGrid.Invoke(new AssignCharacterToGridData(Character01,0,0));        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
