using UnityEditor;
using UnityEngine;

namespace Editors
{
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
}