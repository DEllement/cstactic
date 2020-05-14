using System.Collections;
using System.Collections.Generic;
using API;
using UnityEngine;

public class TestButtonsHandlers : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnNextTurnClick(){
        print("OnNextTurnClick");
        GameCommands.ForceNextTurn.Invoke(null);
    }
    public void OnShowActionMenuClick(){
       GameCommands.ShowActionsMenu.Invoke(null);
    }
    public void OnShowCharacterHealthStatusClick(){
        GameCommands.ShowHealthStatus.Invoke(null);
    }
    public void OnHideCharacterHealthStatusClick(){
        GameCommands.HideHealthStatus.Invoke(null);
    }
    
}
