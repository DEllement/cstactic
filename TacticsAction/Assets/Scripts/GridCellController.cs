using System;
using System.Collections.Generic;
using System.Linq;
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
        
        GameEvents.GridCellSelected.AddListener(OnGridCellSelected);
    }

    public void Update(){
    
        if(_isRenderedDirty){
            if(IsSelected)
                GetComponent<Renderer>().material.color = Color.green;
            else
                GetComponent<Renderer>().material.color = Color.white;
        }
        
        foreach (var gridCellEdge in Edges)
        {
            if(gridCellEdge.Enabled)
                Debug.DrawLine(gameObject.GetComponent<Transform>().position, gameObject.GetComponent<Transform>().position + gridCellEdge.DirVector(), Color.green);     
        }
    }
    
    //Event Handlers
    
    private void OnGridCellSelected(GridCellSelectedData data)
    {
        bool wasSelected = IsSelected;
        IsSelected = data.GameObject == this.gameObject;
    
        if(OccupiedBy != null)
            if(IsSelected && !wasSelected)
                GameEvents.GridCharacterSelected.Invoke(new GridCharacterSelectedData(OccupiedBy));
            else if(wasSelected)
                GameEvents.GridCharacterDeSelected.Invoke(new GridCharacterDeSelectedData(OccupiedBy));
    }
    
}
[CanEditMultipleObjects]
[CustomEditor(typeof(GridCellController))]
public class GridCellEditor : Editor 
{
    bool showEdges =true;
    string edgesLabel = "Edges";
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridCellController gridCell = (GridCellController)target;
        showEdges = EditorGUILayout.Foldout(showEdges, edgesLabel);
        if( showEdges){
            foreach (var gridCellEdge in gridCell.Edges)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(25f);
                GUILayout.Label(gridCellEdge.Dir.ToString(), GUILayout.Width(50));
                gridCellEdge.Enabled = GUILayout.Toggle(gridCellEdge.Enabled,"", GUILayout.Width(20));
                gridCellEdge.MoveCost = EditorGUILayout.IntField(gridCellEdge.MoveCost, GUILayout.Width(50));
            
                GUILayout.EndHorizontal();
            }
        }
    }
   
}