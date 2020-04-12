using UnityEditor;
using UnityEngine;

namespace Editors
{
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
            if(GUILayout.Button("Compute Edges"))
            {
                Undo.RecordObject(target, "Compute Edges");
                myTarget.ComputeEdges();
            }
            if(GUILayout.Button("LoadGridCellsFromChilds"))
            {
                Undo.RecordObject(target, "GridCellsFromChildsLoaded");
                myTarget.LoadGridCellsFromChilds();
            }
            if(GUILayout.Button("Toggle GridCellsLabels"))
            {
                Undo.RecordObject(target, "ToggleGridCellsLabels");
                myTarget.ToggleGridCellsLabels();
            }
            if(GUILayout.Button("Destroy Grid"))
            {
                Undo.RecordObject(target, "Destroyed Grid");
                myTarget.DestroyGrid();
            }
        }
    }
}