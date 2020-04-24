using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using API;
using API.Commands;
using API.Events;
using Dijkstra.NET.Graph;
using Dijkstra.NET.ShortestPath;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


[ExecuteInEditMode]
public class GridController : MonoBehaviour
{
    public int Rows;
    public int Cols;
    public int Space;
    public GameObject prefab;
    
    private bool _initialized;
    private bool _isActionMenuOpen = false;
    private GameObject [,] _gridCells;
    private GameObject _selectedCharacter;
    private Graph<uint, string> _graph;
    private Dictionary<string, (int x,int y ,GridCellDir dir)> _charactersPositions;
    private Dictionary<uint, GridCellController> _cellIdxToGridCellCtrls = new Dictionary<uint, GridCellController>();
    private Dictionary<uint, uint> _avMoveCellToNodeIdx = new Dictionary<uint, uint>();
    
    public void GenerateGrid(){
    
        if(_initialized)
            return;
        
        print("GenerateGrid");
  
        _gridCells = new GameObject[Rows,Cols];
        
        uint i=0;
        for(var y = 0; y < Rows; y++)
            for(var x =0; x < Cols; x++){
                var obj = Instantiate(prefab, new Vector3(x*Space, 0, y*Space), Quaternion.identity);
                obj.tag = "GridCell";
                obj.GetComponentInChildren<TextMesh>().text = x+","+y;
                obj.transform.SetParent(gameObject.transform);
                var gridCellCtrl = obj.AddComponent<GridCellController>();
                gridCellCtrl.Init();
                gridCellCtrl.X = x;
                gridCellCtrl.Y = y;
                gridCellCtrl.CellIndex = ++i;
                obj.name = GetGridCellName(x,y);
                _gridCells[x,y] = obj;
                _cellIdxToGridCellCtrls[gridCellCtrl.CellIndex] = gridCellCtrl;
            }
            
        ComputeEdges();
        _initialized = true;
        GameEvents.GridReady.Invoke();
    }
    public string GetGridCellName(int x, int y){
        return "GridCell ("+x+","+y+")";
    }
    public void DestroyGrid()
    {
        if(_gridCells == null)
            print("DestroyGrid :: gridCells is null");
        if(_gridCells != null)
            for(var y = 0; y < Rows; y++)
                for(var x =0; x < Cols; x++)
                    DestroyImmediate(_gridCells[x,y]); //GameObject.Find(GetGridCellName(x,y));
        
        _initialized = false;
    }
    private bool CanWalkAt(int x, int y){
        if(x < 0 || x >= Cols)
            return false;
        if(y < 0 || y >= Rows)
            return false;
        return _gridCells[x,y] != null &&
               _gridCells[x,y].GetComponent<GridCellController>().IsWalkable &&
               (_gridCells[x,y].GetComponent<GridCellController>().OccupiedBy == null ||
                _gridCells[x,y].GetComponent<GridCellController>().OccupiedBy.CompareTag("Player"));
    }
    public void ComputeEdges(){
        
        if(_gridCells == null)
            print("ComputeEdges :: gridCells is null");
        
        if(_gridCells != null)
            for(var y = 0; y < Rows; y++)
                for(var x =0; x < Cols; x++){
                    var gridCellCtrl = _gridCells[x,y].GetComponent<GridCellController>();
                    gridCellCtrl.Get(GridCellDir.N).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x,y+1);
                    gridCellCtrl.Get(GridCellDir.S).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x,y-1);
                    gridCellCtrl.Get(GridCellDir.W).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x-1,y);
                    gridCellCtrl.Get(GridCellDir.E).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x+1,y);
                    gridCellCtrl.Get(GridCellDir.NW).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x-1,y+1) && gridCellCtrl.Get(GridCellDir.N).Enabled && gridCellCtrl.Get(GridCellDir.W).Enabled;
                    gridCellCtrl.Get(GridCellDir.NE).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x+1,y+1) && gridCellCtrl.Get(GridCellDir.N).Enabled && gridCellCtrl.Get(GridCellDir.E).Enabled;
                    gridCellCtrl.Get(GridCellDir.SW).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x-1,y-1) && gridCellCtrl.Get(GridCellDir.S).Enabled && gridCellCtrl.Get(GridCellDir.W).Enabled;
                    gridCellCtrl.Get(GridCellDir.SE).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x+1,y-1) && gridCellCtrl.Get(GridCellDir.S).Enabled && gridCellCtrl.Get(GridCellDir.E).Enabled;
                }
    }
    public void LoadGridCellsFromChilds(){
        print("LoadGridCellsFromChilds " + transform.childCount);
        _gridCells = new GameObject[Rows,Cols];
        for(var i = 0 ; i < transform.childCount; i++){
            var go = transform.GetChild(i).gameObject;
            if( go.CompareTag("GridCell")){
                var gridCellCtrl = go.GetComponent<GridCellController>();
                _gridCells[gridCellCtrl.X,gridCellCtrl.Y] = go;
                _cellIdxToGridCellCtrls[gridCellCtrl.CellIndex] = gridCellCtrl;
            }
        }  
        GameEvents.GridReady.Invoke();
    }
    public void ToggleGridCellsLabels(){
        for(var y = 0; y < Rows; y++)
        for(var x =0; x < Cols; x++){
            var label = _gridCells[x,y].transform.Find("Label");
            label.gameObject.SetActive(!label.gameObject.activeSelf);
        }
    }
    private void DeSelectSelectedCharacter(){
        currentActionSelected = ActionType.None;
        if(_selectedCharacter == null)
            return;
        var characterCtrl = _selectedCharacter.GetComponent<GridCharacterController>();
        _gridCells[characterCtrl.X, characterCtrl.Y].GetComponent<GridCellController>().DeSelect(); //this should dispatch GridCharacterDeSelected
    }
    private void ShowGridCellAsPossibleMove(){
        foreach (var cellIdx in _avMoveCellToNodeIdx.Keys)
            _cellIdxToGridCellCtrls[cellIdx].ShowGridCellAsPossibleMove();
    }
    private void HideGridCellAsPossibleMove(){
        print("HideGridCellAsPossibleMove");
        foreach (var cellIdx in _avMoveCellToNodeIdx.Keys)
            _cellIdxToGridCellCtrls[cellIdx].HideGridCellAsPossibleMove();
        _avMoveCellToNodeIdx.Clear();
    }

    
    
    private void ConnectNodesUsingGridCellCtrls(GridCellController gridCellCtrlA, GridCellController gridCellCtrlB, bool isDiagonal=false){
        if(!_avMoveCellToNodeIdx.ContainsKey(gridCellCtrlA.CellIndex) || !_avMoveCellToNodeIdx.ContainsKey(gridCellCtrlB.CellIndex))
            return;
        _graph.Connect(_avMoveCellToNodeIdx[gridCellCtrlA.CellIndex], _avMoveCellToNodeIdx[gridCellCtrlB.CellIndex], isDiagonal ? 2 : 1, null);
    }
    
    #region Events Handlers
    
    private void Handle(GridCharacterSelectedData data)
    {
        if(_selectedCharacter == data.GameObject)
            return;
        if(_selectedCharacter != null){
            DeSelectSelectedCharacter();
        }
        
        _selectedCharacter = data.GameObject;
        var gridCharCtrl = _selectedCharacter.GetComponent<GridCharacterController>();
        var charPos = (gridCharCtrl.X, gridCharCtrl.Y);
        _graph = new Graph<uint, string>();
        
        _avMoveCellToNodeIdx.Clear();
        
        var maxMoveCost = 2; //This will be dynamic
        for(var y = Math.Max(charPos.Y-maxMoveCost,0); y < Math.Min(Rows,charPos.Y+maxMoveCost+1); y++)
            for(var x =Math.Max(charPos.X-maxMoveCost,0); x < Math.Min(Cols,charPos.X+maxMoveCost+1); x++){
                var gridCellCtrl = _gridCells[x,y].GetComponent<GridCellController>();
                if( !gridCellCtrl.IsWalkable || (gridCellCtrl.OccupiedBy != null && gridCellCtrl.OccupiedBy.CompareTag("Ennemy")))
                    continue;
                _graph.AddNode(gridCellCtrl.CellIndex);
                _avMoveCellToNodeIdx.Add(gridCellCtrl.CellIndex, (uint)_graph.NodesCount);
        }
   
        for(var y = Math.Max(charPos.Y-maxMoveCost,0); y < Math.Min(Rows,charPos.Y+maxMoveCost+1); y++)
            for(var x =Math.Max(charPos.X-maxMoveCost,0); x < Math.Min(Cols,charPos.X+maxMoveCost+1); x++){
                var gridCellCtrl = _gridCells[x,y].GetComponent<GridCellController>();
                if(!_avMoveCellToNodeIdx.ContainsKey(gridCellCtrl.CellIndex))
                    continue;
                gridCellCtrl.Edges.ForEach(edge=>{
                    if(!edge.Enabled)
                        return;
                    switch(edge.Dir){
                        case GridCellDir.N:  ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x,y+1].GetComponent<GridCellController>()); return;
                        case GridCellDir.S:  ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x,y-1].GetComponent<GridCellController>()); return;
                        case GridCellDir.W:  ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x-1,y].GetComponent<GridCellController>()); return;
                        case GridCellDir.E:  ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x+1,y].GetComponent<GridCellController>()); return;
                        case GridCellDir.NW: ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x-1,y+1].GetComponent<GridCellController>(),true); return;
                        case GridCellDir.NE: ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x+1,y+1].GetComponent<GridCellController>(),true); return;
                        case GridCellDir.SW: ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x-1,y-1].GetComponent<GridCellController>(),true); return;
                        case GridCellDir.SE: ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x+1,y-1].GetComponent<GridCellController>(),true); return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
            }
        
        //Remove impossible move and calculate move cost
        var charGridCellCtrl = _gridCells[charPos.X,charPos.Y].GetComponent<GridCellController>();
        _avMoveCellToNodeIdx.Select( kvp=>{
            if( charGridCellCtrl.CellIndex == kvp.Key)
                return (kvp.Key, -1); //already there
            var result = _graph.Dijkstra(_avMoveCellToNodeIdx[charGridCellCtrl.CellIndex], _avMoveCellToNodeIdx[kvp.Key]);
            //result.Distance;
            var moveCost = result.GetPath().Count();
            return (kvp.Key, moveCost);
        })
          .ToList()
          .ForEach(x=>{
              if(x.Item2 == 0)
                _avMoveCellToNodeIdx.Remove(x.Key);
          });
        
        ShowGridCellAsPossibleMove();
    }
    private void Handle(GridCharacterDeSelectedData data){
        if( _selectedCharacter == data.GameObject){
            _selectedCharacter = null;
            HideGridCellAsPossibleMove();
        }
    }
    private void Handle(GridCharacterDoneMovingData arg0)
    {
        ComputeEdges();
    }

 
    private void OnActionMenuOpened()
    {
        _isActionMenuOpen=true;
    }
    private void OnActionMenuClosed()
    {
        _isActionMenuOpen=false;
        print("OnActionMenuClosed");
    }
    
    #endregion
    
    #region Commands Handlers
    private void Execute(AssignCharacterToGridData data)
    {
        print("AssignCharacterToGridData");
        _gridCells[data.X,data.Y].GetComponent<GridCellController>().OccupiedBy = data.GameObject;
        data.GameObject.GetComponent<GridCharacterController>().X = data.X;
        data.GameObject.GetComponent<GridCharacterController>().Y = data.Y;
        data.GameObject.transform.position = _gridCells[data.X,data.Y].gameObject.transform.position;
        //Commands.SetCharacterPositionOnGrid.Invoke(data.GameObject, data.X,data.Y); //TODO: add dir?
    }
    
    private ActionType currentActionSelected;  
    private void Execute(ShowPossibleMoveData data)
    {
        ShowGridCellAsPossibleMove();
        if(data.SetAsChooseLocation){
            currentActionSelected = ActionType.Move;
        }
    }
    #endregion

    #region LifeCycle
    void Start()
    {
        GameCommands.AssignCharacterToGrid.AddListener(Execute);
        GameCommands.ShowPossibleMove.AddListener(Execute);
        
        GameEvents.GridCharacterSelected.AddListener(Handle);
        GameEvents.GridCharacterDeSelected.AddListener(Handle);
        GameEvents.GridCharacterDoneMoving.AddListener(Handle);
        GameEvents.ActionMenuOpened.AddListener(OnActionMenuOpened);
        GameEvents.ActionMenuClosed.AddListener(OnActionMenuClosed);
        
        print("Start");
        if(_gridCells == null)
            LoadGridCellsFromChilds();      
    }
    void Update()
    {
        DetectGridCellClick();
    }
    
    private void DetectGridCellClick(){
        
        if(_isActionMenuOpen)
            return;
        
        if(Input.GetMouseButtonUp(0)){
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                var selection = hit.transform;
                if( selection.CompareTag("GridCell")){
                    print("GridCell hit");
                    OnGridCellClicked(selection.gameObject);
                }
            }
        }
    }

    private void OnGridCellClicked(GameObject gridCellGameObject){
        var gridCellCtrl = gridCellGameObject.GetComponent<GridCellController>();
        gridCellCtrl.Select();

        if (_selectedCharacter == null || _graph == null)
            return;
        
        var characterCtrl = _selectedCharacter.GetComponent<GridCharacterController>();
        //Check if not clicked outside of the possible move zone
        if( gridCellCtrl.OccupiedBy == null &&                                        //Clicked on empty cell
            !_avMoveCellToNodeIdx.ContainsKey(gridCellCtrl.CellIndex) &&               //Clicked on impossible move
            (gridCellCtrl.X != characterCtrl.X || gridCellCtrl.Y != characterCtrl.Y)){ //Clicked not on character
            DeSelectSelectedCharacter();
            return;
        }
        //Otherwise        
        MoveGridCharacter(gridCellCtrl);
    }
    
    private void MoveGridCharacter(GridCellController gridCellCtrl)
    {
        print("moveGridCharacter");
        var characterCtrl = _selectedCharacter.GetComponent<GridCharacterController>();
        if( _selectedCharacter == null || _graph == null)
            return;
        
        var from = _gridCells[characterCtrl.X, characterCtrl.Y].GetComponent<GridCellController>().CellIndex;
        var to = gridCellCtrl.CellIndex;
        if (@from == to || _graph.NodesCount <= 0 || _avMoveCellToNodeIdx.Count <= 0)
            return;
        
        //test if have possible move
        var result = _graph.Dijkstra(_avMoveCellToNodeIdx[@from], _avMoveCellToNodeIdx[to]);
        var path = result.GetPath().Select(nodeIndex =>{
            var ctrl = _cellIdxToGridCellCtrls[_graph[nodeIndex].Item];
            return new GridPath(ctrl.X, ctrl.Y, ctrl.gameObject.transform.position, ctrl.TileType);
        }).ToArray();
        
        if (path.Length <= 0)
            return;
        
        GameCommands.MoveGridCharacter.Invoke(new MoveGridCharacterData(_selectedCharacter, path));
        HideGridCellAsPossibleMove();
    }

    #endregion
}

