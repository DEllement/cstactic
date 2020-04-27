using System;
using System.Collections.Generic;
using System.Linq;
using API;
using API.Commands;
using API.Events;
using Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    
    public void Click()
    {
        GameEvents.GridCellClicked.Invoke(new GridCellClickedData(gameObject, (X,Y)));
        
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
