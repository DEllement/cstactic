using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] public HealthBarsController healthsBar;
    [SerializeField] public CharacterStatusBarController leftCharacterStatusBar;
    [SerializeField] public CharacterStatusBarController rightCharacterStatusBar;
    
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
    public BattleSceneState GetState(){
        return _state;
    }
    
    private bool _gridReady, _turnBarReady, _setupStarted;
    private bool CanSetupLevel => _gridReady && _turnBarReady && !_setupStarted;
    private void OnGridReady()
    {
        _gridReady = true;
        if(CanSetupLevel)
            SetupLevel();
    }
    private void OnTurnBarReady(){
        _turnBarReady = true;
        if(CanSetupLevel)
            SetupLevel();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.GridCellClicked.AddListener(Handle);
        GameEvents.GridTargetsTargeted.AddListener(Handle);
        GameEvents.GridTargetsSelected.AddListener(Handle);
        GameEvents.GridOutsideTargetRangeClicked.AddListener(Handle);
        GameEvents.GridCharacterClicked.AddListener(Handle);
        GameEvents.GridCharacterDeSelected.AddListener(Handle);
        GameEvents.GridCharacterDoneMoving.AddListener(Handle);
        GameEvents.ActionMenuOpened.AddListener(OnActionMenuOpened);
        GameEvents.ActionMenuClosed.AddListener(OnActionMenuClosed);
        GameEvents.ActionMenuItemClicked.AddListener(OnActionMenuItemClicked);
        GameEvents.CharacterTurnStarted.AddListener(Handle);
        GameEvents.GridReady.AddListener(OnGridReady);
        GameEvents.TurnBarReady.AddListener(OnTurnBarReady);
        GameEvents.NonUIClicked.AddListener(Handle);
        
        GameCommands.AssignCharacterToGrid.AddListener(Execute);
        GameCommands.ShowHealthStatus.AddListener(Execute);
        GameCommands.HideHealthStatus.AddListener(Execute);
        
        SetState(new NothingSelectedState(this));
    }




    private void SetupLevel(){
        _setupStarted = true;
        levelManager.SetupLevel();
        turnManager.Init(levelManager.Friends, levelManager.Ennemies);
        turnManager.Next();
        //SetState(new BattleIntroState(this));
    }

    private void Execute(ShowAllHealthStatusData data)
    {
        var damageableTargets =  grid.GetAllDamageableTargets();
        var items = damageableTargets.Select( x=> new HealthBarsController.DamageableTargetInfo{ damageableCtrl = x } )
                                                              .ToList();
        healthsBar.ShowHealthStatus(items);
    }
    private void Execute(HideHealthStatusData data)
    {
        healthsBar.HideHealthStatus();
    }
    private void Execute(AssignCharacterToGridData data)
    {
        grid.AssignCharacterToGrid(data);
    }
    
    private void Handle(GridCellClickedData data)
    {
        StartCoroutine(_state.OnGridCellClicked(data));
    }
    private void Handle(GridTargetsTargetedData data)
    {
        StartCoroutine(_state.OnGridTargetsTargeted(data));
    }
    private void Handle(GridTargetsSelectedData data)
    {
        StartCoroutine(_state.OnGridTargetsSelected(data));
    }
    private void Handle(GridOutsideTargetRangeClickedData data)
    {
        StartCoroutine(_state.OnGridOutsideTargetRangeClicked(data));
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
    private void Handle()
    {
        StartCoroutine(_state.OnNonUIClicked());
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
