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
    
    public Material gridCellMat;
    
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
                obj.GetComponent<Renderer>().material.color = Color.white;
                obj.GetComponentInChildren<TextMesh>().text = x+","+y;
                obj.transform.SetParent(gameObject.transform);
                obj.AddComponent<GridCellController>();
                var gridCellCtrl = obj.GetComponent<GridCellController>();
                gridCellCtrl.Init();
                gridCellCtrl.X = x;
                gridCellCtrl.Y = y;
                gridCellCtrl.GraphIndex = ++i;
                obj.name = GetGridCellName(x,y);
                gridCells[x,y] = obj;
                graphIndexToGridCellCtrls[gridCellCtrl.GraphIndex] = gridCellCtrl;
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
        return gridCells[x,y] != null && gridCells[x,y].GetComponent<GridCellController>().IsWalkable; //TODO: and nothing is on it
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
                graphIndexToGridCellCtrls[gridCellCtrl.GraphIndex] = gridCellCtrl;
            }
        }  
        GameEvents.GridReady.Invoke();
    }
    
    //Life Cycle
    
    // Start is called before the first frame update
    void Start()
    {
        charactersPositions = new Dictionary<string, (int x, int y, GridCellDir dir)>();
        graphIndexToGridCellCtrls = new Dictionary<uint, GridCellController>();
        
        GameCommands.AssignCharacterToGrid.AddListener(Execute);
        GameEvents.GridCharacterSelected.AddListener(Handle);
        
        print("Start");
        if(gridCells == null)
            LoadGridCellsFromChilds();      
    }
    private Dictionary<string, (int x,int y ,GridCellDir dir)> charactersPositions;
    private GameObject currentSelectedCharacter;
    private Graph<uint, string> graph;
    private Dictionary<uint, GridCellController> graphIndexToGridCellCtrls;
    private void Handle(GridCharacterSelectedData data)
    {
        currentSelectedCharacter = data.GameObject;
        
        graph = new Graph<uint, string>();
        
        //Calculate Possible move
        //Show possible move on grid
        
        //for each on possible move
        for(var y = 0; y < Rows; y++)
            for(var x =0; x < Cols; x++){
                graph.AddNode(gridCells[x,y].GetComponent<GridCellController>().GraphIndex);
        }
        for(var y = 0; y < Rows; y++)
        for(var x =0; x < Cols; x++){
            var gridCellCtrl = gridCells[x,y].GetComponent<GridCellController>();
            gridCellCtrl.Edges.ForEach(edge=>{
                    if(!edge.Enabled)
                        return;
                    switch(edge.Dir){
                        case GridCellDir.N: graph.Connect(gridCellCtrl.GraphIndex, gridCells[x,y+1].GetComponent<GridCellController>().GraphIndex, 1, null); return;
                        case GridCellDir.S: graph.Connect(gridCellCtrl.GraphIndex, gridCells[x,y-1].GetComponent<GridCellController>().GraphIndex, 1, null); return;
                        case GridCellDir.W: graph.Connect(gridCellCtrl.GraphIndex, gridCells[x-1,y].GetComponent<GridCellController>().GraphIndex, 1, null); return;
                        case GridCellDir.E: graph.Connect(gridCellCtrl.GraphIndex, gridCells[x+1,y].GetComponent<GridCellController>().GraphIndex, 1, null); return;
                        case GridCellDir.NW: graph.Connect(gridCellCtrl.GraphIndex, gridCells[x-1,y+1].GetComponent<GridCellController>().GraphIndex, 2, null); return;
                        case GridCellDir.NE: graph.Connect(gridCellCtrl.GraphIndex, gridCells[x+1,y+1].GetComponent<GridCellController>().GraphIndex, 2, null); return;
                        case GridCellDir.SW: graph.Connect(gridCellCtrl.GraphIndex, gridCells[x-1,y-1].GetComponent<GridCellController>().GraphIndex, 2, null); return;
                        case GridCellDir.SE: graph.Connect(gridCellCtrl.GraphIndex, gridCells[x+1,y-1].GetComponent<GridCellController>().GraphIndex, 2, null); return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
                print("EdgesCount"+ graph.EdgesCount(gridCellCtrl.GraphIndex));
            }
        
        print("NodesCount "+graph.NodesCount);
        //TODO:
        //Show possible move on grid
        //Generate PathFinding Graph
        //Assign current char selected
        //On next click, move the char if char selected
    }

    private void Execute(AssignCharacterToGridData data)
    {
        print("AssignCharacterToGridData");
        gridCells[data.X,data.Y].GetComponent<GridCellController>().OccupiedBy = data.GameObject;
        data.GameObject.GetComponent<GridCharacterController>().X = data.X;
        data.GameObject.GetComponent<GridCharacterController>().Y = data.Y;
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
                print("GridCell hit");
                var selection = hit.transform;
                if( selection.CompareTag("GridCell")){
                    GameEvents.GridCellSelected.Invoke(new GridCellSelectedData(selection.gameObject));
                    //TODO: refactor in method
                    if(currentSelectedCharacter != null && graph != null){
                        var characterCtrl = currentSelectedCharacter.GetComponent<GridCharacterController>();
                        var from =  gridCells[characterCtrl.X, characterCtrl.Y].GetComponent<GridCellController>().GraphIndex;
                        var to = selection.gameObject.GetComponent<GridCellController>().GraphIndex;
                        if(from != to){ 
                            var result = graph.Dijkstra(from, to);
                            var path = result.GetPath().Select(graphIndex=> {
                                var ctrl = graphIndexToGridCellCtrls[graph[graphIndex].Item];
                                return new GridPath(ctrl.X, ctrl.Y, ctrl.gameObject.transform.position, ctrl.TileType);
                            }).ToArray();
                            print("path length " + path.Length);
                            GameCommands.MoveGridCharacter.Invoke(new MoveGridCharacterData(currentSelectedCharacter, path));
                        }
                    }
                    
                    
                }
            }
        }
    }
}

