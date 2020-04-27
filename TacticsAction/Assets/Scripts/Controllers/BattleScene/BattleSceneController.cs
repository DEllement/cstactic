using System.Collections;
using System.Collections.Generic;
using API;
using API.Commands;
using API.Events;
using Controllers.BattleScene.States;
using UnityEngine;

public class BattleSceneController : MonoBehaviour
{
    [SerializeField] public GridController grid;
    [SerializeField] public ActionMenuController actionMenu;
    [SerializeField] public TurnBarController turnBar;
    [SerializeField] public LevelManager levelManager;
    [SerializeField] public ActionTreeManager actionTreeManager;
    [SerializeField] public BattleManager battleManager;
    [SerializeField] public TurnManager turnManager;
    
    private BattleSceneState _previousState;
    private BattleSceneState _state;
    public void SetState(BattleSceneState state){
        if(_state != null)
            StartCoroutine(_state.Exit());
        _state = state;
        StartCoroutine(_state.Enter());
        print(_state.ToString());
    } 
    
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.GridCellClicked.AddListener(Handle);
        GameEvents.GridCharacterClicked.AddListener(Handle);
        GameEvents.GridCharacterDeSelected.AddListener(Handle);
        GameEvents.GridCharacterDoneMoving.AddListener(Handle);
        GameEvents.ActionMenuOpened.AddListener(OnActionMenuOpened);
        GameEvents.ActionMenuClosed.AddListener(OnActionMenuClosed);
        GameEvents.ActionMenuItemClicked.AddListener(OnActionMenuItemClicked);
        GameEvents.CharacterTurnStarted.AddListener(Handle);
        
        GameCommands.AssignCharacterToGrid.AddListener(Execute);
        //GameCommands.ShowPossibleMove.AddListener(Execute);
        
        SetState(new NothingSelectedState(this));
    }

    private void Handle(GridCellClickedData data)
    {
        StartCoroutine(_state.OnGridCellClicked(data));
    }

    private void Execute(AssignCharacterToGridData data)
    {
        grid.AssignCharacterToGrid(data);
    }

    private void Handle(GridCharacterClickedData data) 
    {
        StartCoroutine(_state.OnGridCharacterClicked(data));
    }
    private void Handle(GridCharacterDeSelectedData data)
    {
        //StartCoroutine(_state.OnGridCharacterDeSelected(data));
    }
    private void Handle(GridCharacterDoneMovingData data)
    {
        StartCoroutine(_state.OnGridCharacterDoneMoving(data));
    }
    private void Handle(CharacterTurnStartedData arg0)
    {
        
    }
    
    private void OnActionMenuOpened()
    {
        //SetState(new ActionMenuOpenState(this));
    }
    private void OnActionMenuClosed()
    {
        StartCoroutine(_state.OnActionMenuClosed());
    }
    private void OnActionMenuItemClicked(ActionMenuItemClickedData data)
    {
        StartCoroutine(_state.OnActionMenuItemClicked(data));
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
