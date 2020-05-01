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
  
        _gridCells = new GameObject[Rows,Cols];
        
        uint i=0;
        for(var y = 0; y < Rows; y++)
            for(var x =0; x < Cols; x++){
                var obj = Instantiate(prefab, new Vector3(x*Space, 0, y*Space), Quaternion.identity);
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
                    gridCellCtrl.Get(GridCellDir.N ).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x,y+1);
                    gridCellCtrl.Get(GridCellDir.S ).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x,y-1);
                    gridCellCtrl.Get(GridCellDir.W ).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x-1,y);
                    gridCellCtrl.Get(GridCellDir.E ).Enabled = gridCellCtrl.IsWalkable && CanWalkAt(x+1,y);
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
        GameEvents.GridReady.Invoke(); //TODO: Could pass the matrix down
    }
    public void ToggleGridCellsLabels(){
        for(var y = 0; y < Rows; y++)
        for(var x =0; x < Cols; x++){
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
    
    private bool _enableTargetRayTracker;
    private List<GameObject> _attackZoneGridCells;
    private List<GameObject> _attackTargetGridCells;
    private List<AttackRayTarget> _raysTargets;
    
    //CreateTargetTracker(true, (5,5), new List<GridCellDir>{},0,1);
    public class AttackRayTarget{
        public float startOffset;
        public float length;
        public GridCellDir Dir;
        public Vector3 Start;
        public Vector3 End => Start + (GridCellEdge.ToDirVector(Dir) * length );

        public Vector3 heading => End - Start;
        public float distance => heading.magnitude;
        public Vector3 direction => heading / distance; // This is now the normalized direction.
        
        public AttackRayTarget( GridCellDir dir, Vector3 start, float startOffset=0, float length=1)
        {
            this.startOffset = startOffset;
            this.length = length;
            Dir = dir;
            Start = start;
        }
    }
    
    public void CreateTargetTracker(
        (int x, int y) origPos,
        List<GridCellDir> attackZoneDirs,
        float attackZoneMinRadius=0,
        float attackZoneMaxRadius=1,
        int attackRangeType=0, //Circle, Square, Line
        int attackPatternType=0){ //Single, Line, Square
    
        var targetTrackerOrigin = _gridCells[origPos.x,origPos.y].transform.position;
        
        //Get Affected GridCell
            
        _attackZoneGridCells = new List<GameObject>();
        _attackTargetGridCells = new List<GameObject>();
        _raysTargets = new List<AttackRayTarget>();
     
        //Create Zone of Action
        attackZoneDirs.ForEach(dir=>{ //This should be an other pattern + radius
            //_raysTargets.Add(new AttackRayTarget(dir, targetTrackerOrigin,attackZoneMinRadius, attackZoneMaxRadius));
            var attackRangeRayTarget = new AttackRayTarget(dir, targetTrackerOrigin,attackZoneMinRadius, attackZoneMaxRadius);
            var hits = Physics.RaycastAll(attackRangeRayTarget.Start, attackRangeRayTarget.direction, attackRangeRayTarget.distance);
            foreach (var hit in hits)
            {
                if( hit.transform.CompareTag("AttackColider") ){
                    var gridCell = hit.transform.parent.gameObject;
                    _attackZoneGridCells.Add(gridCell);
                    gridCell.GetComponent<GridCellController>().Quad.GetComponent<Renderer>().material.color = Color.yellow;
                }
            }
        });
        //Create Target Rays
        if(attackPatternType == 0){ //1 Generic Forward attack
            _raysTargets.Add(new AttackRayTarget(GridCellDir.UP, Vector3.zero, 0f, 4f));
        }
        
        _enableTargetRayTracker = true;
    }
    private void UpdateTargetTracker(){
        if(!_enableTargetRayTracker)
            return;
        
        var overGridCellInZone = _attackZoneGridCells.Contains(_overGridCell);
        
        _attackTargetGridCells.ForEach(tGridCell=>{
            tGridCell.GetComponent<GridCellController>().Quad.GetComponent<Renderer>().material.color = Color.white;
        });
        _attackTargetGridCells.Clear();
        _attackZoneGridCells.ForEach(tGridCell=>{
            tGridCell.GetComponent<GridCellController>().Quad.GetComponent<Renderer>().material.color = Color.yellow;
        });
        
        if(overGridCellInZone){
            _raysTargets.ForEach( rayInfo=>{
                rayInfo.Start = _overGridCell.transform.position+ new Vector3(0f, -1f, 0f);
                
                //Debug.DrawRay(rayInfo.Start,  rayInfo.direction*rayInfo.distance, Color.red );
                var hits = Physics.RaycastAll(rayInfo.Start, rayInfo.direction, rayInfo.distance);
                foreach (var hit in hits)
                {
                    if( hit.transform.CompareTag("GridCell") ){
                        var gridCell = hit.transform.gameObject;//hit.transform.parent.gameObject;
                        _attackTargetGridCells.Add(gridCell);
                        gridCell.GetComponent<GridCellController>().Quad.GetComponent<Renderer>().material.color = Color.red;
                    }
                }
            });
        }
        print(_attackTargetGridCells.Count + " " + Input.GetMouseButtonUp(0));
        if(Input.GetMouseButtonUp(0) && _attackTargetGridCells.Count > 0){
            GameEvents.GridTargetsSelected.Invoke(new GridTargetsSelectedData(_attackTargetGridCells));
        }
    }
    public void CancelTargetTracker(){
        _enableTargetRayTracker = false;
    }
    
    #endregion
    
    #region GridHelpers
    
    private float [,] ToWorkMatrix((int x,int y) center, int radius){
        int rows = radius*2+1;
        int cols = rows;
        float [,] output = new float [cols,rows];
        for(var x = Math.Max(0, center.x-radius); x < Math.Min(_gridCells.GetLength(0), center.x+radius); x++)    
            for(var y = Math.Max(0, center.y-radius); y < Math.Min(_gridCells.GetLength(1), center.y+radius); y++)
            {
                var gridCellCtrl = _gridCells[x,y].GetComponent<GridCellController>();
                output[x,y] = gridCellCtrl.IsWalkable ? 1 : -4; //Floor , Wall
                if (gridCellCtrl.OccupiedBy == null)
                    continue;
                var charCtrl = gridCellCtrl.OccupiedBy.GetComponent<GridCharacterController>();
                if( charCtrl != null)
                    output[x,y] = charCtrl.Character.IsEnnemy ? -2 : -1; //Ennemy Friend    
                else
                    output[x,y] = -3; //Crate or other object
            }
            
        return output;
    }
    
    /*
     * -1 = character
     * 0 = walkable (empty)
     * 1 = 
     * 
     */
    
    public class GridWorker{
        public float[,] grid;
        public float[,] results;

        public void Snapshot(float[,] gameGridSection){
            grid = gameGridSection;
        }
        
        public void Apply(){
                    
        }

        public float[,] Rotate(float[,] matrix, float angle)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            var output = new float[rows,cols];

            /*float[,] yrollValues = {{Mathf.Cos(angleY), 0, Mathf.Sin(angleY),0},
                                   {0,1,0,0},
                                   {-Mathf.Sin(angleY), 0, Mathf.Cos(angleY),0},
                                   {0,0,0,1}};*/
            
            switch(angle){
                case -90:
                case 270:
                    for(var x=0; x < cols; x++)
                        for(var y = 0; y < rows; y++)
                            output[x,y] = matrix[y,cols - y - 1];
                    break;
                case 90:
                case -270:
                    for(var x=0; x < cols; x++)
                        for(var y = 0; y < rows; y++)
                            output[x,y] = matrix[cols - y - 1,x];
                    break;
                case 180:
                case -180:
                    for(var x=0; x < cols; x++)
                        for(var y = 0; y < rows; y++)
                            output[x,y] = matrix[cols - x - 1,x];
                    break;
                default:
                    output = (float[,]) matrix.Clone();
                    break;
            }
            
            return output;
        }
    }
    
    #endregion
    
}

