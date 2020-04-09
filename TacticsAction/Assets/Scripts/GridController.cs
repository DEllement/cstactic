using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        for(var y = 0; y < Rows; y++)
            for(var x =0; x < Cols; x++){
                var obj = Instantiate(prefab, new Vector3(x*Space, 0, y*Space), Quaternion.identity);
                obj.tag = "GridCell";
                obj.GetComponent<Renderer>().material.color = Color.white;
                obj.GetComponentInChildren<TextMesh>().text = x+","+y;
                obj.transform.SetParent(gameObject.transform);
                obj.AddComponent<GridCellController>();
                obj.GetComponent<GridCellController>().Init();
                obj.GetComponent<GridCellController>().X = x;
                obj.GetComponent<GridCellController>().Y = y;
                obj.name = GetGridCellName(x,y);
                gridCells[x,y] = obj;
            }
            
        //Temps
        gridCells[0,0].GetComponent<GridCellController>().OccupiedBy = GameObject.Find("Character01");
        DisableImpossibleEdges();
        Initialized = true;
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
    
    public void DisableImpossibleEdges(){
        
        if(gridCells == null)
            print("DisableImpossibleEdges :: gridCells is null");
        
        if(gridCells != null)
            for(var y = 0; y < Rows; y++)
                for(var x =0; x < Cols; x++){
                    
                    var gridCellCtrl = gridCells[x,y].GetComponent<GridCellController>();
                    print(gridCellCtrl);
                    
                    gridCellCtrl.Edges.ForEach(edge=>{
                        edge.Enabled = gridCellCtrl.IsWalkable;
                    });
                    if(gridCellCtrl.IsWalkable){
                        gridCellCtrl.Get(GridCellDir.NW).Enabled = x>0 && y < Rows-1 && gridCells[x-1,y+1] != null && gridCells[x-1,y+1].GetComponent<GridCellController>().IsWalkable;
                        gridCellCtrl.Get(GridCellDir.N).Enabled = y < Rows-1 && gridCells[x,y+1] != null && gridCells[x,y+1].GetComponent<GridCellController>().IsWalkable;
                        gridCellCtrl.Get(GridCellDir.NE).Enabled = x<Cols-1 && y< Rows-1 && gridCells[x+1,y+1] != null && gridCells[x+1,y+1].GetComponent<GridCellController>().IsWalkable;
                        gridCellCtrl.Get(GridCellDir.W).Enabled = x>0 && gridCells[x-1,y] != null && gridCells[x-1,y].GetComponent<GridCellController>().IsWalkable;
                        gridCellCtrl.Get(GridCellDir.E).Enabled = x<Cols-1 && gridCells[x+1,y] != null && gridCells[x+1,y].GetComponent<GridCellController>().IsWalkable;
                        gridCellCtrl.Get(GridCellDir.SW).Enabled = x>0 && y > 0 && gridCells[x-1,y-1] != null && gridCells[x-1,y-1].GetComponent<GridCellController>().IsWalkable;
                        gridCellCtrl.Get(GridCellDir.S).Enabled =  y > 0 && gridCells[x,y-1] != null && gridCells[x,y-1].GetComponent<GridCellController>().IsWalkable;
                        gridCellCtrl.Get(GridCellDir.SE).Enabled = x<Cols-1 && y > 0 && gridCells[x+1,y-1] != null && gridCells[x+1,y-1].GetComponent<GridCellController>().IsWalkable;
                    }
                }
    }
    T[,] ResizeArray<T>(T[,] original, int rows, int cols)
    {
        var newArray = new T[rows,cols];
        int minRows = Math.Min(rows, original.GetLength(0));
        int minCols = Math.Min(cols, original.GetLength(1));
        for(int i = 0; i < minRows; i++)
        for(int j = 0; j < minCols; j++)
            newArray[i, j] = original[i, j];
        return newArray;
    }
    
    void LoadGridCellsFromChilds(){
        print("LoadGridCellsFromChilds " + transform.childCount);
        gridCells = new GameObject[Rows,Cols];
        for(var i = 0 ; i < transform.childCount; i++){
            var go = transform.GetChild(i).gameObject;
            if( go.CompareTag("GridCell")){
                var gridCellCtrl = go.GetComponent<GridCellController>();
                gridCells[gridCellCtrl.X,gridCellCtrl.Y] = go;
            }
        }     
    }
    
    /*void Awake(){
        print("Awake");
        if(gridCells == null)
            LoadGridCellsFromChilds();  
    }*/
    
    // Start is called before the first frame update
    void Start()
    {
        print("Start");
        if(gridCells == null)
            LoadGridCellsFromChilds();      
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
                    GameEvents.GridCellSelected.Invoke(new GridCellSelectedData(selection.gameObject));
                }
            }
        }
    }
}

[CustomEditor(typeof(GridController))]
public class GridControllerEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridController myTarget = (GridController)target;
        if(GUILayout.Button("Generate Grid"))
        {
            Undo.RecordObject(target, "Generated Grid");
            myTarget.GenerateGrid();
        }
        if(GUILayout.Button("Disable Impossible Edge"))
        {
            Undo.RecordObject(target, "Disable Impossible Edge");
            myTarget.DisableImpossibleEdges();
        }
        if(GUILayout.Button("Destroy Grid"))
        {
            Undo.RecordObject(target, "Destroyed Grid");
            myTarget.DestroyGrid();
        }
    }
}
