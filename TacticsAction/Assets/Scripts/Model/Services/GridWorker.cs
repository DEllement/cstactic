using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Services
{
    
    //TODO:
    //    - SetHeightMap
    //    - SetTargetHeightMap
    public class GridWorker{
        public float[,] workGrid;
        public float[,] range;
        public float[,] rangeResult;
        public float[,] targetBase;
        public float[,] targetResult;
        public List<Vector2> rangeCells = new List<Vector2>();
        public List<Vector2> previousRangeCells = new List<Vector2>();
        public List<Vector2> targetCells = new List<Vector2>();
        public List<Vector2> previousTargetCells = new List<Vector2>();
        public int currentTargetAngle = 0;
        
        public Vector2 offsetPos;
        public Vector2 cursorPos;
        public Vector2 agentPos;
        
        public bool targetCenteredOnCursor;

        public void New(float[,] workGrid, Vector2 offsetPos, Vector2 agentPos, bool targetCenteredOnCursor=false){
            this.workGrid = workGrid;
            this.agentPos = agentPos;
            this.offsetPos = offsetPos;
            this.targetCenteredOnCursor = targetCenteredOnCursor;
        }
        
        public void SetRange(float[,] rangeTemplate)
        {
            range = rangeTemplate;
            rangeResult = (float[,]) rangeTemplate.Clone();
            var aOffset = targetCenteredOnCursor ? cursorPos-offsetPos :agentPos-offsetPos;
            Apply(ref workGrid, ref rangeResult, aOffset);
            UpdateRangeCells();
        }
        public void SetTarget(float[,] targetTemplate){
            targetBase = targetTemplate;
            targetResult = (float[,])targetTemplate.Clone();
        }

        public float[,] Rotate(float[,] matrix, float angle)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            var output = new float[rows,cols];

            var roundedAngle = angle;//(int) (Math.Round(angle/90f)*90f);
            switch(roundedAngle){
                case 270:
                case -90:
                    for(var x=0; x < cols; x++)
                    for(var y = 0; y < rows; y++)
                        output[x,y] = matrix[cols - y - 1,x];
                    break;
                case 180:
                case -180:
                    output = Rotate(Rotate(matrix, 90f), 90f);
                    break;
                case 90: //Rotate Anti-clockwise
                case -270:
                    for(var x=0; x < cols; x++)
                    for(var y = 0; y < rows; y++)
                        output[x,y] = matrix[y,cols - x - 1];
                    
                    break;
                default:
                    for(var x=0; x < cols; x++)
                        for(var y = 0; y < rows; y++)
                            output[x,y] = matrix[x,y];
                    break;
            }
            
            return output;
        }

        //Return true if changed
        public void SetCursorAt(Vector2 cursorPos)
        {
            this.cursorPos = cursorPos;
            //UpdateTargetCells();
        }

        // Apply (add) a to b
        public void Apply(ref float[,] a, ref float[,] b, Vector2 aOffset)
        {
            var aL = a.GetLength(0);
            var bL = b.GetLength(0);
            for(var tx=0;tx < bL; tx++)
                for (var ty = 0; ty < bL; ty++){
                    var x = tx + (int)aOffset.x;
                    var y = ty + (int)aOffset.y;
                    if(x >= 0 && y >=0 && x < aL && y < aL)
                        b[tx, ty] = a[x, y] + b[tx, ty];
                }
        }
        public void SetAgentAt(Vector2 agentPos)
        {
            this.agentPos = agentPos;
        }
        public void SetAngleFromCenter(float angle)
        {
            var roundedAngle = (int) (Math.Round(angle/90f)*90f);
            if( currentTargetAngle == roundedAngle )
                return;
            
            currentTargetAngle = roundedAngle;
            targetResult = Rotate(targetBase, currentTargetAngle);
            
            Debug.Log("angle " + currentTargetAngle);
            
            var aOffset = targetCenteredOnCursor ? cursorPos-offsetPos :agentPos-offsetPos;
            Apply(ref workGrid, ref targetResult, aOffset);
            UpdateTargetCells();
        }
        public void UpdateRangeCells(){
            previousRangeCells = rangeCells;
            for (var x = 0; x < rangeResult.GetLength(0); x++){
                for (var y = 0; y < rangeResult.GetLength(1); y++){
                    if(rangeResult[x,y] > 0)
                        rangeCells.Add(new Vector2(y,x) + offsetPos);
                }
            }
        }
        public void UpdateTargetCells()
        {
            //Print(ref targetResult);
            previousTargetCells = targetCells;
            targetCells = new List<Vector2>();
            for (var x = 0; x < targetResult.GetLength(0); x++){
                for (var y = 0; y < targetResult.GetLength(1); y++){
                    if(targetResult[x,y] > 0){
                        var pos = new Vector2(y,x) + offsetPos; //FIXME: why did i had to invert x & y
                       targetCells.Add(pos);
                    }
                }
            }
        }
        public void Print(ref float[,] matrix){
            Debug.Log("==== a:" + currentTargetAngle);
            for(var x=0; x < matrix.GetLength(0); x++){
                var str = "";
                for(var y=0; y < matrix.GetLength(1); y++){
                    str += matrix[x,y]+",";
                }
                Debug.Log(str);
            }
        }

    
    }
    
}