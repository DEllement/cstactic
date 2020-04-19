using System;
using System.Collections.Generic;
using System.Linq;
using API;
using API.Commands;
using API.Events;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum GridCellDir{
    NW,N,NE,W,E,SW,S,SE
}
public enum TileType{
    Default,
} 
[System.Serializable]
public class GridCellEdge{
    public GridCellDir Dir;
    public bool Enabled=true;
    public int MoveCost=1;
    
    public static Vector3 NW = new Vector3(-.5f,0f,.5f);
    public static Vector3 N = new Vector3(.0f,0f,.5f);
    public static Vector3 NE = new Vector3(.5f,0f,.5f);
    public static Vector3 W = new Vector3(-.5f,0f,0f);
    public static Vector3 E = new Vector3(.5f,0f,0f);
    public static Vector3 SW = new Vector3(-.5f,0f,-.5f);
    public static Vector3 S = new Vector3(.0f,0f,-.5f);
    public static Vector3 SE = new Vector3(.5f,0f,-.5f);
    
    public Vector3 DirVector(){
        switch(Dir){
            case GridCellDir.NW: return NW;
            case GridCellDir.N: return N;
            case GridCellDir.NE: return NE;
            case GridCellDir.W: return W;
            case GridCellDir.E: return E;
            case GridCellDir.SW: return SW;
            case GridCellDir.S: return S;
            case GridCellDir.SE: return SE;
        }
        return Vector3.zero;
    }
}
[System.Serializable]
public class GridCellController : MonoBehaviour{
    public int X;
    public int Y;
    public uint CellIndex; //used for graph node correlation Rename to CellIdx
    public TileType TileType;
    public GameObject OccupiedBy;
    public List<GridCellEdge> Edges;
    public GridCellEdge Get(GridCellDir dir){
        return Edges.First(x=>x.Dir == dir);
    }
    
    public bool IsWalkable; //TODO: that could be computed
    
    private bool _isSelected;
    public bool IsSelected {
        get => _isSelected;
        set{
            _isSelected = value;
            _isRenderedDirty = true;
        }
    }
    private bool _isRenderedDirty;

    //LifeCycle

    public void Init(){
        Edges = new List<GridCellEdge>
        {
            new GridCellEdge {Dir = GridCellDir.NW, Enabled = true},
            new GridCellEdge {Dir = GridCellDir.N, Enabled = true},
            new GridCellEdge {Dir = GridCellDir.NE, Enabled = true},
            new GridCellEdge {Dir = GridCellDir.W, Enabled = true},
            new GridCellEdge {Dir = GridCellDir.E, Enabled = true},
            new GridCellEdge {Dir = GridCellDir.SW, Enabled = true},
            new GridCellEdge {Dir = GridCellDir.S, Enabled = true},
            new GridCellEdge {Dir = GridCellDir.SE, Enabled = true}
        };
        IsWalkable = true;
    }
    Color defaultColor;
    Color overColor;
    public void Start(){
        GameEvents.GridCharacterLeavingGridCell.AddListener(OnGridCharacterLeavingGridCell);
        GameEvents.GridCharacterMovedToGridCell.AddListener(OnGridCharacterMovedToGridCell);
        GameEvents.GridCharacterMovingToGridCell.AddListener(OnGridCharacterMovingToGridCell);
        
        defaultColor = new Color {r=255f,g=255f,b=255f, a = 0.25f};
        overColor = new Color {r=0f,g=0f,b=255f, a = 0.25f};
        GetComponent<Renderer>().material.color = defaultColor;
    }
    
    public void Update(){
    
            //if(OccupiedBy != null)
            //    GetComponent<Renderer>().material.color = Color.green;
            //else
            //    GetComponent<Renderer>().material.color = Color.white;
        
        if(Debug.isDebugBuild)
            foreach (var gridCellEdge in Edges)
            {
                if(gridCellEdge.Enabled)
                    Debug.DrawLine(gameObject.GetComponent<Transform>().position, gameObject.GetComponent<Transform>().position + gridCellEdge.DirVector(), Color.green);     
            }
    }
    
    void OnDrawGizmos()
    {
        foreach (var gridCellEdge in Edges)
        {
            if(gridCellEdge.Enabled){
                Gizmos.color = Color.green;
                Gizmos.DrawLine(gameObject.GetComponent<Transform>().position, gameObject.GetComponent<Transform>().position + gridCellEdge.DirVector());     
            }
        }
    }
    
    //Commands
    private bool showAsPossibleMove;
    public void ShowGridCellAsPossibleMove(){
         showAsPossibleMove = true;
         GetComponent<Renderer>().material.color = Color.magenta;
    }
    public void HideGridCellAsPossibleMove(){
        showAsPossibleMove = false;
        GetComponent<Renderer>().material.color = defaultColor;
    }
    
    //Event Handlers
    
    void OnMouseOver()
    {
        if(showAsPossibleMove)
            GetComponent<Renderer>().material.color = Color.red;
        else
            GetComponent<Renderer>().material.color = overColor;
        
        //GameEvents.GridCellMouseOver.Invoke(gameObject);
    }

    void OnMouseExit()
    {
        if(showAsPossibleMove)
            GetComponent<Renderer>().material.color = Color.magenta;
        else
            GetComponent<Renderer>().material.color = defaultColor;
        
        //GameEvents.GridCellMouseExit.Invoke(gameObject);
    }
    
    public void Select()
    {
        IsSelected = true;
    
        //TODO: Base on what is OccupiedBy on the tile we need to dispatch the correct event (or a generic one)
        if(OccupiedBy != null)
            OccupiedBy.GetComponent<GridCharacterController>().Select();
    }
    public void DeSelect(){
        if(OccupiedBy != null)
            OccupiedBy.GetComponent<GridCharacterController>().DeSelect();
    }
    
    private void OnGridCharacterLeavingGridCell(GridCharacterLeavingGridCellData data){
        if(this.OccupiedBy == data.GameObject){
            this.OccupiedBy = null;
            GetComponent<Renderer>().material.color = defaultColor;
        }
    }
    private void OnGridCharacterMovedToGridCell(GridCharacterMovedToGridCellData data){
        if(data.GameObject == OccupiedBy && data.X != X && data.Y != Y){
            OccupiedBy = null;
            GetComponent<Renderer>().material.color = defaultColor;
            _isRenderedDirty =true;
        }else if(data.X == X && data.Y == Y){
            OccupiedBy = data.GameObject;
        }
    }
    private void OnGridCharacterMovingToGridCell(GridCharacterMovingToGridCellData data){
        if(data.X == X && data.Y == Y){
            GetComponent<Renderer>().material.color = Color.yellow;
            _isRenderedDirty =true;
        }
    }
    
}
