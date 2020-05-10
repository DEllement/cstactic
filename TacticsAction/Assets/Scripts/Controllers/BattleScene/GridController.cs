using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using API;
using API.Commands;
using API.Events;
using Dijkstra.NET.Graph;
using Dijkstra.NET.ShortestPath;
using Model;
using Model.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


[ExecuteInEditMode]
public class GridController : MonoBehaviour
{
    public int Rows;
    public int Cols;
    public int Space;
    public GameObject prefab;
        
    public GridSelectionMode selectionMode;
    
    private bool _initialized;
    private bool _isActionMenuOpen = false;
    private GameObject [,] _gridCells;
    private GameObject _selectedCharacter;
    private Graph<uint, string> _graph;
    //private Dictionary<string, (int x,int y ,GridCellDir dir)> _charactersPositions;
    private Dictionary<uint, GridCellController> _cellIdxToGridCellCtrls = new Dictionary<uint, GridCellController>();
    private Dictionary<uint, uint> _avCellToNodeIdx = new Dictionary<uint, uint>();
    
    public GameObject SelectedCharacter => _selectedCharacter;
    
    private GameObject GetGridCell(int x, int y){
        if(x < 0 || y < 0 || x >= Cols || y >= Rows)
            return null;
        return _gridCells[x, y];
    }
    private GameObject GetGridCell((int x, int y) pos){
        return GetGridCell(pos.x, pos.y);
    }
    private GameObject GetGridCell(Vector2 pos){
        return GetGridCell((int)pos.x, (int)pos.y);
    }
    private GridCellController GetGridCellCtrl(int x, int y){
        return GetGridCell(x, y)?.GetComponent<GridCellController>();
    }
    private GridCellController GetGridCellCtrl((int x, int y) pos)
    {
        return GetGridCell(pos)?.GetComponent<GridCellController>();
    }
    private GridCellController GetGridCellCtrl(Vector2 v){
        return GetGridCell((int)v.x, (int)v.y)?.GetComponent<GridCellController>();
    }
    private GridCellController GetGridCellCtrl(uint cellIdx)
    {
        return _cellIdxToGridCellCtrls.ContainsKey(cellIdx) ? _cellIdxToGridCellCtrls[cellIdx] : null;
    }
    
    #region LifeCycle
    void Start()
    {
        print("Start");
        if(_gridCells == null)
            LoadGridCellsFromChilds();      
    }
    void Update()
    {
        DetectGridCellClick();
        UpdateTargetTracker();
    }
    #endregion
    
    public void GenerateGrid(){
    
        if(_initialized)
            return;
        
        print("GenerateGrid");
  
        _gridCells = new GameObject[Cols,Rows];
        
        float yOffset = Rows*Space;
        
        uint i=0;
        for(var y = 0; y < Rows; y++)
            for(var x =0; x < Cols; x++){
                var obj = Instantiate(prefab, new Vector3(x*Space, 0, yOffset-y*Space), Quaternion.identity);
                    obj.tag = "GridCell";
                    obj.name = GetGridCellName(x,y);
                    obj.GetComponentInChildren<TextMesh>().text = x+","+y;
                    obj.transform.SetParent(gameObject.transform);
                var gridCellCtrl = obj.AddComponent<GridCellController>();
                    gridCellCtrl.Init();
                    gridCellCtrl.X = x;
                    gridCellCtrl.Y = y;
                    gridCellCtrl.CellIndex = ++i;
                _gridCells[x,y] = obj;
                _cellIdxToGridCellCtrls[gridCellCtrl.CellIndex] = gridCellCtrl;
            }
            
        ComputeEdges();
        _initialized = true;
        GameEvents.GridReady.Invoke(); //TODO: could pass the matrix down
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
    public bool CanWalkAt(int x, int y){
        var gridCellCtrl = GetGridCellCtrl(x,y);
        return  gridCellCtrl != null &&
                gridCellCtrl.IsWalkable &&
               (gridCellCtrl.OccupiedBy == null ||
                gridCellCtrl.OccupiedBy.CompareTag("Player"));
    }
    public void ComputeEdges(){
        
        if(_gridCells == null)
            print("ComputeEdges :: gridCells is null");
        
        if(_gridCells != null)
            for(var y = 0; y < Rows; y++)
                for(var x =0; x < Cols; x++){
                    var gridCellCtrl = _gridCells[x,y].GetComponent<GridCellController>();
                    gridCellCtrl.Get(GridCellDir.N ).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x,y-1);
                    gridCellCtrl.Get(GridCellDir.S ).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x,y+1);
                    gridCellCtrl.Get(GridCellDir.W ).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x-1,y);
                    gridCellCtrl.Get(GridCellDir.E ).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x+1,y);
                    gridCellCtrl.Get(GridCellDir.NW).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x-1,y-1) && gridCellCtrl.Get(GridCellDir.N).Enabled && gridCellCtrl.Get(GridCellDir.W).Enabled;
                    gridCellCtrl.Get(GridCellDir.NE).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x+1,y-1) && gridCellCtrl.Get(GridCellDir.N).Enabled && gridCellCtrl.Get(GridCellDir.E).Enabled;
                    gridCellCtrl.Get(GridCellDir.SW).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x-1,y+1) && gridCellCtrl.Get(GridCellDir.S).Enabled && gridCellCtrl.Get(GridCellDir.W).Enabled;
                    gridCellCtrl.Get(GridCellDir.SE).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x+1,y+1) && gridCellCtrl.Get(GridCellDir.S).Enabled && gridCellCtrl.Get(GridCellDir.E).Enabled;
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
        GameEvents.GridReady.Invoke(); //TODO: Could pass the matrix down
    }
    public void ToggleGridCellsLabels(){
        for(var y = 0; y < Rows; y++)
        for(var x = 0; x < Cols; x++){
            var label = _gridCells[x,y].transform.Find("Label");
            label.gameObject.SetActive(!label.gameObject.activeSelf);
        }
    }
    public void DeSelectSelectedCharacter(){
        print("DeSelectSelectedCharacter");
        selectionMode = GridSelectionMode.Cell;
        
        if(_selectedCharacter == null)
            return;
        var characterCtrl = _selectedCharacter.GetComponent<GridCharacterController>();
        _gridCells[characterCtrl.X, characterCtrl.Y].GetComponent<GridCellController>().DeSelect(); //this should dispatch GridCharacterDeSelected
    }
    public void SelectCharacter(GridCharacterClickedData data){ //TODO: change params
        /*if(_selectedCharacter != null && _selectedCharacter != data.GameObject){
            DeSelectSelectedCharacter();
        }*/
        _selectedCharacter = data.GameObject;
    }
    public void SelectCharacter(int characterId){
        
        foreach (var gridCell in _gridCells)
        {
            if( gridCell.GetComponent<GridCellController>().OccupiedBy != null){
                
                var gridCharacterCtrl = gridCell.GetComponent<GridCellController>()
                                                .GetComponentInChildren<GridCharacterController>();
                if( gridCharacterCtrl != null && gridCharacterCtrl.Character.Id == characterId){
                    gridCharacterCtrl.Select();
                    break;
                }
            }
        }
    }
    
    public void ShowGridCellAsReachable(){
        foreach (var cellIdx in _avCellToNodeIdx.Keys)
            _cellIdxToGridCellCtrls[cellIdx].ShowGridCellAsPossibleMove();
    }
    public void HideGridCellAsReachable(){
        print("HideGridCellAsPossibleMove");
        foreach (var cellIdx in _avCellToNodeIdx.Keys)
            _cellIdxToGridCellCtrls[cellIdx].HideGridCellAsPossibleMove();
        _avCellToNodeIdx.Clear();
    }
    private void ConnectNodesUsingGridCellCtrls(GridCellController gridCellCtrlA, GridCellController gridCellCtrlB, bool isDiagonal=false){
        if(!_avCellToNodeIdx.ContainsKey(gridCellCtrlA.CellIndex) || !_avCellToNodeIdx.ContainsKey(gridCellCtrlB.CellIndex))
            return;
        _graph.Connect(_avCellToNodeIdx[gridCellCtrlA.CellIndex], _avCellToNodeIdx[gridCellCtrlB.CellIndex], isDiagonal ? 2 : 1, null);
    }
    public void BuildPossibleGroundMoveGraph(int maxMoveCost){
        var gridCharCtrl = _selectedCharacter.GetComponent<GridCharacterController>();
        var charPos = (gridCharCtrl.X, gridCharCtrl.Y);
        _graph = new Graph<uint, string>();
        _avCellToNodeIdx.Clear();
        for(var y = Math.Max(charPos.Y-maxMoveCost,0); y < Math.Min(Rows,charPos.Y+maxMoveCost+1); y++)
            for(var x =Math.Max(charPos.X-maxMoveCost,0); x < Math.Min(Cols,charPos.X+maxMoveCost+1); x++){
                var gridCellCtrl = _gridCells[x,y].GetComponent<GridCellController>();
                if( !gridCellCtrl.IsWalkable || (gridCellCtrl.OccupiedBy != null && gridCellCtrl.OccupiedBy.CompareTag("Ennemy")))
                    continue;
                _graph.AddNode(gridCellCtrl.CellIndex);
                _avCellToNodeIdx.Add(gridCellCtrl.CellIndex, (uint)_graph.NodesCount);
        }
   
        for(var y = Math.Max(charPos.Y-maxMoveCost,0); y < Math.Min(Rows,charPos.Y+maxMoveCost+1); y++)
            for(var x =Math.Max(charPos.X-maxMoveCost,0); x < Math.Min(Cols,charPos.X+maxMoveCost+1); x++){
                var gridCellCtrl = _gridCells[x,y].GetComponent<GridCellController>();
                if(!_avCellToNodeIdx.ContainsKey(gridCellCtrl.CellIndex))
                    continue;
                gridCellCtrl.Edges.ForEach(edge=>{
                    if(!edge.Enabled)
                        return;
                    switch(edge.Dir){
                        case GridCellDir.N:  ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x,y-1].GetComponent<GridCellController>()); return;
                        case GridCellDir.S:  ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x,y+1].GetComponent<GridCellController>()); return;
                        case GridCellDir.W:  ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x-1,y].GetComponent<GridCellController>()); return;
                        case GridCellDir.E:  ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x+1,y].GetComponent<GridCellController>()); return;
                        case GridCellDir.NW: ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x-1,y-1].GetComponent<GridCellController>(),true); return;
                        case GridCellDir.NE: ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x+1,y-1].GetComponent<GridCellController>(),true); return;
                        case GridCellDir.SW: ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x-1,y+1].GetComponent<GridCellController>(),true); return;
                        case GridCellDir.SE: ConnectNodesUsingGridCellCtrls(gridCellCtrl, _gridCells[x+1,y+1].GetComponent<GridCellController>(),true); return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
            }
        
        //Remove impossible move and calculate move cost
        var charGridCellCtrl = _gridCells[charPos.X,charPos.Y].GetComponent<GridCellController>();
        _avCellToNodeIdx.Select( kvp=>{
            if( charGridCellCtrl.CellIndex == kvp.Key)
                return (kvp.Key, -1); //already there
            var result = _graph.Dijkstra(_avCellToNodeIdx[charGridCellCtrl.CellIndex], _avCellToNodeIdx[kvp.Key]);
            //result.Distance;
            var moveCost = result.GetPath().Count();
            return (kvp.Key, moveCost);
        })
          .ToList()
          .ForEach(x=>{
              if(x.Item2 == 0)
                _avCellToNodeIdx.Remove(x.Key);
          });
    }
    public void BuildPossibleRangeGraph(int maxRangeCost){
    
    }
    
    public void AssignCharacterToGrid(AssignCharacterToGridData data){
        print("AssignCharacterToGridData");
        //TODO: this is not dynamic yet
        _gridCells[data.X,data.Y].GetComponent<GridCellController>().OccupiedBy = data.GameObject;
        data.GameObject.GetComponent<GridCharacterController>().X = data.X;
        data.GameObject.GetComponent<GridCharacterController>().Y = data.Y;
        data.GameObject.GetComponent<GridCharacterController>().Character = data.Character;
        data.GameObject.transform.position = _gridCells[data.X,data.Y].transform.position + new Vector3(0,.5f,0);
    }
    private GameObject _overGridCell;
    private Vector3 _overGridCellWorldMousePos;
    private void DetectGridCellClick(){
       
        if(selectionMode == GridSelectionMode.Disabled)
            return;
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if(!hits.Any(x=> x.transform.CompareTag("UI")))
        {
            var hit = hits.FirstOrDefault(x=> x.transform.CompareTag("GridCell"));
            if( hit.transform != null){
                _overGridCell = hit.transform.gameObject;
                _overGridCellWorldMousePos = hit.point;
                if(Input.GetMouseButtonUp(0)){
                    ClickGridCell(_overGridCell);
                }
            }
        }
    }
    private void ClickGridCell(GameObject gridCellGameObject){
        var gridCellCtrl = gridCellGameObject.GetComponent<GridCellController>();
        gridCellCtrl.Click(); //This will generate GridCellClicked AND GridCharacterClicked if any
    }
    public bool IsOutSideOfMoveZone((int x, int y) pos){
        var characterCtrl = this._selectedCharacter.GetComponent<GridCharacterController>(); 
        var gridCellCtrl = _gridCells[pos.x, pos.y].GetComponent<GridCellController>(); 
        return gridCellCtrl.OccupiedBy != null || (                                   //Clicked on a non empty cell
            !_avCellToNodeIdx.ContainsKey(gridCellCtrl.CellIndex) &&              //Clicked on impossible move
            (gridCellCtrl.X != characterCtrl.X || gridCellCtrl.Y != characterCtrl.Y));//Clicked not on character
    }
    public bool IsInsideZone((int x, int y) pos){
        var gridCellCtrl = _gridCells[pos.x, pos.y].GetComponent<GridCellController>();
        return _avCellToNodeIdx.ContainsKey(gridCellCtrl.CellIndex);
    }
    public bool IsInsideGrid(Vector2 pos){
        return pos.x >= 0 && pos.y >=0 && pos.x < Cols && pos.y < Rows;
    }
    public bool WalkCharacterToIfPossible((int x, int y) toPos){
        var characterCtrl = _selectedCharacter.GetComponent<GridCharacterController>();
        var gridCellCtrl = _gridCells[toPos.x, toPos.y].GetComponent<GridCellController>(); 
        if( _selectedCharacter == null || _graph == null)
            return false;
        var from = _gridCells[characterCtrl.X, characterCtrl.Y].GetComponent<GridCellController>().CellIndex;
        var to = gridCellCtrl.CellIndex;
        if (@from == to || _graph.NodesCount <= 0 || _avCellToNodeIdx.Count <= 0)
            return false;
        
        //test if have possible move
        if(!_avCellToNodeIdx.ContainsKey(to))
            return false;
        
        var result = _graph.Dijkstra(_avCellToNodeIdx[@from], _avCellToNodeIdx[to]);
        var path = result.GetPath().Select(nodeIndex =>{
            var ctrl = _cellIdxToGridCellCtrls[_graph[nodeIndex].Item];
            return new GridPath(ctrl.X, ctrl.Y, ctrl.gameObject.transform.position, ctrl.TileType);
        }).ToArray();
        
        if( path.Length > 0 ){
            characterCtrl.MoveGridCharacter(path);
            return true;
        }
        
        return false;
    }
    
    
    #region RayCastCollision
    
    private bool _enableAttack;

    private GridWorker _attackGridWorker = new GridWorker();
  
    public void CreateTargetTracker(
        (int x, int y) origPos,
        List<GridCellDir> attackZoneDirs,
        float attackZoneMinRadius=0,
        float attackZoneMaxRadius=1,
        int attackRangeType=0, //Circle, Square, Line
        int attackPatternType=0){ //Single, Line, Square
    
        var charCtrl =_selectedCharacter.GetComponent<GridCharacterController>();
        if (attackRangeType == 0)
        { 
            var targetTrackerOrigin = charCtrl.CellCoords;//_gridCells[origPos.x,origPos.y].transform.position;
            var radius = 2; //FIXME: = template.length/2+1
            _attackGridWorker.New( ToWorkMatrix(targetTrackerOrigin, radius ), new Vector2(origPos.x-radius, origPos.y-radius), charCtrl.CellCoords );
            _attackGridWorker.SetRange(new float[,] {
                {0,0,0,0,0},
                {0,0,1,0,0},
                {0,1,0,1,0},
                {0,0,1,0,0},
                {0,0,0,0,0},
            });
            _attackGridWorker.SetTarget(new float[,] {
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,1,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
            });
        }
        
        _enableAttack = true;
    }
    Vector2 OverGridCellPosVector => _overGridCell?.GetComponent<GridCellController>()?.GridPosVector ?? new Vector2(-1,-1);
    List<Vector2> _lastTargetedCells;
    bool _hasTargetsCellsChanged = false;
    private void UpdateTargetTracker(){
        if(!_enableAttack || _overGridCell == null)
            return;
        
        _attackGridWorker.previousRangeCells.ForEach(cellPos=>{ 
            if( IsInsideGrid(cellPos) )
                GetGridCellCtrl(cellPos).Quad.GetComponent<Renderer>().material.color = Color.white;
        });
        
        var worldCharacterPos = _selectedCharacter.gameObject.GetComponentInParent<Transform>().position;
        var angle = (float) (Math.Atan2(_overGridCellWorldMousePos.z-worldCharacterPos.z,_overGridCellWorldMousePos.x-worldCharacterPos.x) * 180 / Math.PI);
        _attackGridWorker.SetAngleFromCenter(angle);
        _attackGridWorker.SetCursorAt(OverGridCellPosVector);
        
        _hasTargetsCellsChanged = _attackGridWorker.targetCells != _lastTargetedCells;
        
        _attackGridWorker.rangeCells.ForEach(cellPos=>{
            if( IsInsideGrid(cellPos) )
                GetGridCellCtrl(cellPos).Quad.GetComponent<Renderer>().material.color = Color.yellow;
        });
        _attackGridWorker.targetCells.ForEach(cellPos=>{
            if( IsInsideGrid(cellPos) )
                GetGridCellCtrl(cellPos).Quad.GetComponent<Renderer>().material.color = Color.red;
        });
        var overGridCellInZone = _attackGridWorker.targetCells.Contains(OverGridCellPosVector);
        if( overGridCellInZone ){
            if(_hasTargetsCellsChanged){
                GameEvents.GridTargetsTargeted.Invoke(new GridTargetsTargetedData(_attackGridWorker.targetCells
                                                                                                   .Where( IsInsideGrid )
                                                                                                   .Select( GetGridCell )
                                                                                                   .ToList()));            
            }
            if( Input.GetMouseButtonUp(0) )
                GameEvents.GridTargetsSelected.Invoke(new GridTargetsSelectedData(_attackGridWorker.targetCells
                                                                                                   .Where( IsInsideGrid )
                                                                                                   .Select( GetGridCell )
                                                                                                   .ToList()));
        }
    }
    public void CancelTargetTracker(){
        _enableAttack =false;
    }
    
    #endregion
    
    private float [,] ToWorkMatrix(Vector2 center, int radius){
        int rows = radius*2+1;
        int cols = rows;
        float [,] output = new float [cols,rows];
        int ox = 0;
        int oy = 0;
        for(var x = center.x-radius; x < cols; x++,ox++)
            for(var  y = center.y-radius; y < rows; y++,oy++){
                var gridCellCtrl = GetGridCellCtrl(center);
                output[ox,oy] = gridCellCtrl != null && gridCellCtrl.IsWalkable ? 0 : int.MinValue;
        }
        return output;
    }
}

