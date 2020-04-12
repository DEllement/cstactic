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
   
    private GameObject [,] gridCells;
    
    private bool Initialized;
    
    public void GenerateGrid(){
    
        if(Initialized)
            return;
        
        print("GenerateGrid");
  
        gridCells = new GameObject[Rows,Cols];
        
        uint i=0;
        for(var y = 0; y < Rows; y++)
            for(var x =0; x < Cols; x++){
                var obj = Instantiate(prefab, new Vector3(x*Space, 0, y*Space), Quaternion.identity);
                obj.tag = "GridCell";
                obj.GetComponentInChildren<TextMesh>().text = x+","+y;
                obj.transform.SetParent(gameObject.transform);
                obj.AddComponent<GridCellController>();
                var gridCellCtrl = obj.GetComponent<GridCellController>();
                gridCellCtrl.Init();
                gridCellCtrl.X = x;
                gridCellCtrl.Y = y;
                gridCellCtrl.CellIndex = ++i;
                obj.name = GetGridCellName(x,y);
                gridCells[x,y] = obj;
                cellIdxToGridCellCtrls[gridCellCtrl.CellIndex] = gridCellCtrl;
            }
            
        ComputeEdges();
        Initialized = true;
        GameEvents.GridReady.Invoke();
    }
    public string GetGridCellName(int x, int y){
        return "GridCell ("+x+","+y+")";
    }
    
    public void DestroyGrid()
    {
        if(gridCells == null)
            print("DestroyGrid :: gridCells is null");
        if(gridCells != null)
            for(var y = 0; y < Rows; y++)
                for(var x =0; x < Cols; x++)
                    DestroyImmediate(gridCells[x,y]); //GameObject.Find(GetGridCellName(x,y));
        
        Initialized = false;
    }
    
    private bool CanWalkAt(int x, int y){
        if(x < 0 || x >= Cols)
            return false;
        if(y < 0 || y >= Rows)
            return false;
        return gridCells[x,y] != null &&
               gridCells[x,y].GetComponent<GridCellController>().IsWalkable &&
               (gridCells[x,y].GetComponent<GridCellController>().OccupiedBy == null ||
                gridCells[x,y].GetComponent<GridCellController>().OccupiedBy.CompareTag("Player"));
    }
    
    public void ComputeEdges(){
        
        if(gridCells == null)
            print("ComputeEdges :: gridCells is null");
        
        if(gridCells != null)
            for(var y = 0; y < Rows; y++)
                for(var x =0; x < Cols; x++){
                    var gridCellCtrl = gridCells[x,y].GetComponent<GridCellController>();
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
        gridCells = new GameObject[Rows,Cols];
        for(var i = 0 ; i < transform.childCount; i++){
            var go = transform.GetChild(i).gameObject;
            if( go.CompareTag("GridCell")){
                var gridCellCtrl = go.GetComponent<GridCellController>();
                gridCells[gridCellCtrl.X,gridCellCtrl.Y] = go;
                cellIdxToGridCellCtrls[gridCellCtrl.CellIndex] = gridCellCtrl;
            }
        }  
        GameEvents.GridReady.Invoke();
    }
    
    public void ToggleGridCellsLabels(){
        for(var y = 0; y < Rows; y++)
        for(var x =0; x < Cols; x++){
            var label = gridCells[x,y].transform.Find("Label");
            label.gameObject.SetActive(!label.gameObject.activeSelf);
        }
    }
    
    //Life Cycle
    
    // Start is called before the first frame update
    void Start()
    {
        charactersPositions = new Dictionary<string, (int x, int y, GridCellDir dir)>();
        cellIdxToGridCellCtrls = new Dictionary<uint, GridCellController>();
        
        GameCommands.AssignCharacterToGrid.AddListener(Execute);
        GameEvents.GridCharacterSelected.AddListener(Handle);
        GameEvents.GridCharacterDeSelected.AddListener(Handle);
        GameEvents.GridCharacterDoneMoving.AddListener(Handle);
        
        print("Start");
        if(gridCells == null)
            LoadGridCellsFromChilds();      
    }

 

    private Dictionary<string, (int x,int y ,GridCellDir dir)> charactersPositions;
    private GameObject selectedCharacter;
    private Graph<uint, string> graph;
    private Dictionary<uint, GridCellController> cellIdxToGridCellCtrls;
    private Dictionary<uint, uint> avMoveCellToNodeIdx = new Dictionary<uint, uint>();
    private void ConnectNodesUsingGridCellCtrls(GridCellController gridCellCtrlA, GridCellController gridCellCtrlB, bool isDiagonal=false){
        if(!avMoveCellToNodeIdx.ContainsKey(gridCellCtrlA.CellIndex) || !avMoveCellToNodeIdx.ContainsKey(gridCellCtrlB.CellIndex))
            return;
        graph.Connect(avMoveCellToNodeIdx[gridCellCtrlA.CellIndex], avMoveCellToNodeIdx[gridCellCtrlB.CellIndex], isDiagonal ? 2 : 1, null);
    }
    private void Handle(GridCharacterSelectedData data)
    {
        if(selectedCharacter == data.GameObject)
            return;
        if(selectedCharacter != null){
            DeSelectSelectedCharacter();
        }
        
        selectedCharacter = data.GameObject;
        var gridCharCtrl = selectedCharacter.GetComponent<GridCharacterController>();
        var charPos = (gridCharCtrl.X, gridCharCtrl.Y);
        graph = new Graph<uint, string>();
        
        avMoveCellToNodeIdx.Clear();
        
        var maxMoveCost = 2; //This will be dynamic
        for(var y = Math.Max(charPos.Y-maxMoveCost,0); y < Math.Min(Rows,charPos.Y+maxMoveCost+1); y++)
            for(var x =Math.Max(charPos.X-maxMoveCost,0); x < Math.Min(Cols,charPos.X+maxMoveCost+1); x++){
                var gridCellCtrl = gridCells[x,y].GetComponent<GridCellController>();
                if( !gridCellCtrl.IsWalkable || (gridCellCtrl.OccupiedBy != null && gridCellCtrl.OccupiedBy.CompareTag("Ennemy")))
                    continue;
                graph.AddNode(gridCellCtrl.CellIndex);
                avMoveCellToNodeIdx.Add(gridCellCtrl.CellIndex, (uint)graph.NodesCount);
        }
   
        for(var y = Math.Max(charPos.Y-maxMoveCost,0); y < Math.Min(Rows,charPos.Y+maxMoveCost+1); y++)
            for(var x =Math.Max(charPos.X-maxMoveCost,0); x < Math.Min(Cols,charPos.X+maxMoveCost+1); x++){
                var gridCellCtrl = gridCells[x,y].GetComponent<GridCellController>();
                if(!avMoveCellToNodeIdx.ContainsKey(gridCellCtrl.CellIndex))
                    continue;
                gridCellCtrl.Edges.ForEach(edge=>{
                    if(!edge.Enabled)
                        return;
                    switch(edge.Dir){
                        case GridCellDir.N:  ConnectNodesUsingGridCellCtrls(gridCellCtrl, gridCells[x,y+1].GetComponent<GridCellController>()); return;
                        case GridCellDir.S:  ConnectNodesUsingGridCellCtrls(gridCellCtrl, gridCells[x,y-1].GetComponent<GridCellController>()); return;
                        case GridCellDir.W:  ConnectNodesUsingGridCellCtrls(gridCellCtrl, gridCells[x-1,y].GetComponent<GridCellController>()); return;
                        case GridCellDir.E:  ConnectNodesUsingGridCellCtrls(gridCellCtrl, gridCells[x+1,y].GetComponent<GridCellController>()); return;
                        case GridCellDir.NW: ConnectNodesUsingGridCellCtrls(gridCellCtrl, gridCells[x-1,y+1].GetComponent<GridCellController>(),true); return;
                        case GridCellDir.NE: ConnectNodesUsingGridCellCtrls(gridCellCtrl, gridCells[x+1,y+1].GetComponent<GridCellController>(),true); return;
                        case GridCellDir.SW: ConnectNodesUsingGridCellCtrls(gridCellCtrl, gridCells[x-1,y-1].GetComponent<GridCellController>(),true); return;
                        case GridCellDir.SE: ConnectNodesUsingGridCellCtrls(gridCellCtrl, gridCells[x+1,y-1].GetComponent<GridCellController>(),true); return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
            }
        
        //Remove impossible move and calculate move cost
        var charGridCellCtrl = gridCells[charPos.X,charPos.Y].GetComponent<GridCellController>();
        avMoveCellToNodeIdx.Select( kvp=>{
            if( charGridCellCtrl.CellIndex == kvp.Key)
                return (kvp.Key, -1); //already there
            var result = graph.Dijkstra(avMoveCellToNodeIdx[charGridCellCtrl.CellIndex], avMoveCellToNodeIdx[kvp.Key]);
            //result.Distance;
            var moveCost = result.GetPath().Count();
            return (kvp.Key, moveCost);
        })
          .ToList()
          .ForEach(x=>{
              if(x.Item2 == 0)
                avMoveCellToNodeIdx.Remove(x.Key);
          });
        
        ShowGridCellAsPossibleMove();
    }
    private void Handle(GridCharacterDeSelectedData data){
        if( selectedCharacter == data.GameObject){
            selectedCharacter = null;
            HideGridCellAsPossibleMove();
        }
    }
    private void DeSelectSelectedCharacter(){
        if(selectedCharacter == null)
            return;
        var characterCtrl = selectedCharacter.GetComponent<GridCharacterController>();
        gridCells[characterCtrl.X, characterCtrl.Y].GetComponent<GridCellController>().DeSelect(); //this should dispatch GridCharacterDeSelected
    }
    private void ShowGridCellAsPossibleMove(){
        foreach (var cellIdx in avMoveCellToNodeIdx.Keys)
            cellIdxToGridCellCtrls[cellIdx].ShowGridCellAsPossibleMove();
    }
    private void HideGridCellAsPossibleMove(){
        print("HideGridCellAsPossibleMove");
        foreach (var cellIdx in avMoveCellToNodeIdx.Keys)
            cellIdxToGridCellCtrls[cellIdx].HideGridCellAsPossibleMove();
        avMoveCellToNodeIdx.Clear();
    }

    private void Handle(GridCharacterDoneMovingData arg0)
    {
        ComputeEdges();
    }
    
    private void Execute(AssignCharacterToGridData data)
    {
        print("AssignCharacterToGridData");
        gridCells[data.X,data.Y].GetComponent<GridCellController>().OccupiedBy = data.GameObject;
        data.GameObject.GetComponent<GridCharacterController>().X = data.X;
        data.GameObject.GetComponent<GridCharacterController>().Y = data.Y;
        data.GameObject.transform.position = gridCells[data.X,data.Y].gameObject.transform.position;
        //Commands.SetCharacterPositionOnGrid.Invoke(data.GameObject, data.X,data.Y); //TODO: add dir?
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0)){
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                var selection = hit.transform;
                if( selection.CompareTag("GridCell")){
                    print("GridCell hit");
                
                    var gridCellCtrl = selection.gameObject.GetComponent<GridCellController>();
                    gridCellCtrl.Select();
                    
                    //TODO: refactor in method
                    //MoveGridCharacter
                    if(selectedCharacter != null && graph != null){
                        var characterCtrl = selectedCharacter.GetComponent<GridCharacterController>();
                        if( gridCellCtrl.OccupiedBy == null &&                                        //Clicked on empty cell
                            !avMoveCellToNodeIdx.ContainsKey(gridCellCtrl.CellIndex) &&               //Clicked on impossible move
                            (gridCellCtrl.X != characterCtrl.X || gridCellCtrl.Y != characterCtrl.Y)) //Clicked not on character
                        { 
                            DeSelectSelectedCharacter();
                            return;
                        }
                        
                        var from =  gridCells[characterCtrl.X, characterCtrl.Y].GetComponent<GridCellController>().CellIndex;
                        var to   = gridCellCtrl.CellIndex;
                        if(from != to && graph.NodesCount > 0 && avMoveCellToNodeIdx.Count > 0){ //test if have possible move
                            var result = graph.Dijkstra(avMoveCellToNodeIdx[from], avMoveCellToNodeIdx[to]);
                            var path = result.GetPath().Select(nodeIndex=> {
                                var ctrl = cellIdxToGridCellCtrls[graph[nodeIndex].Item];
                                return new GridPath(ctrl.X, ctrl.Y, ctrl.gameObject.transform.position, ctrl.TileType);
                            }).ToArray();
                            if( path.Length > 0 ){
                                GameCommands.MoveGridCharacter.Invoke(new MoveGridCharacterData(selectedCharacter, path));
                                HideGridCellAsPossibleMove();
                            }
                        }
                    }
                }
            }
        }
    }
}

