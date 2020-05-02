using System;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class GridWorkerTests
    {
        private void PrintMatrix(float[,] matrix){
            for(var x=0; x < matrix.GetLength(0); x++){
                for(var y=0; y < matrix.GetLength(1); y++){
                    TestContext.Out.Write(matrix[x,y]);
                }
                TestContext.Out.Write("\n");
            }
        }
        
        [Test]
        public void ItShouldRotateTheAllMatrix(){
        
            var gridWorker = new GridController.GridWorker();
                    
            float[,] matrix = new float[,] {
                {1,1,1,1},
                {2,2,2,2},
                {3,3,3,3},
                {4,4,4,4}
            };
            
            PrintMatrix(matrix);
            
            var output = gridWorker.Rotate( matrix , 90);
            TestContext.Out.WriteLine(" 90 result:");
            PrintMatrix(output);
            
            output = gridWorker.Rotate( matrix , -90);
            TestContext.Out.WriteLine(" -90 result:");
            PrintMatrix(output);
            
            output = gridWorker.Rotate( matrix , 180);
            TestContext.Out.WriteLine(" 180 result:");
            PrintMatrix(output);
            
            output = gridWorker.Rotate( matrix , -180);
            TestContext.Out.WriteLine(" -180 result:");
            PrintMatrix(output);
            
            output = gridWorker.Rotate( matrix , 270);
            TestContext.Out.WriteLine(" 270 result:");
            PrintMatrix(output);
            
            output = gridWorker.Rotate( matrix , -270);
            TestContext.Out.WriteLine(" -270 result:");
            PrintMatrix(output);
            
            output = gridWorker.Rotate( matrix , 0);
            TestContext.Out.WriteLine(" 0 result:");
            PrintMatrix(output);
            
            TestContext.Out.WriteLine(" Large Matrix:");
            PrintMatrix(matrix);
            matrix = new float[,] {
                {0,0,0,0,0,0,0,0,0,0},
                {1,1,1,1,1,1,1,1,1,1},
                {2,2,2,2,2,2,2,2,2,2},
                {3,3,3,3,3,3,3,3,3,3},
                {4,4,4,4,4,4,4,4,4,4},
                {5,5,5,5,5,5,5,5,5,5},
                {6,6,6,6,6,6,6,6,6,6},
                {7,7,7,7,7,7,7,7,7,7},
                {8,8,8,8,8,8,8,8,8,8},
                {9,9,9,9,9,9,9,9,9,9},
            };
            
            output = gridWorker.Rotate( matrix , 90);
            TestContext.Out.WriteLine(" 90 result:");
            PrintMatrix(output);
            
            output = gridWorker.Rotate( matrix , -90);
            TestContext.Out.WriteLine(" -90 result:");
            PrintMatrix(output);
            
            output = gridWorker.Rotate( matrix , 180);
            TestContext.Out.WriteLine(" 180 result:");
            PrintMatrix(output);
            
            output = gridWorker.Rotate( matrix , -180);
            TestContext.Out.WriteLine(" -180 result:");
            PrintMatrix(output);
            
            output = gridWorker.Rotate( matrix , 270);
            TestContext.Out.WriteLine(" 270 result:");
            PrintMatrix(output);
            
            output = gridWorker.Rotate( matrix , -270);
            TestContext.Out.WriteLine(" -270 result:");
            PrintMatrix(output);
            
            output = gridWorker.Rotate( matrix , 0);
            TestContext.Out.WriteLine(" 0 result:");
            PrintMatrix(output);
        }
        
        [Test]
        public void ItShouldRotateTheTarget(){
            var gridWorker = new GridController.GridWorker();
            float[,] baseM = new float[,] {
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            };
            gridWorker.New(baseM, (5,5), (5,5) );
            gridWorker.ApplyTarget(new float[,] {
                {0,0,0},
                {0,0,1},
                {0,0,0},
            });
            
            gridWorker.SetCursorAt((5,0));
            TestContext.Out.WriteLine("result: " + gridWorker.currentTargetAngle + "~" + (float) (Math.Round(gridWorker.currentTargetAngle/90f)*90f));
            PrintMatrix(gridWorker.targetResult);
            
            gridWorker.SetCursorAt((8,9));
            TestContext.Out.WriteLine("result: " + gridWorker.currentTargetAngle + "~" + (float) (Math.Round(gridWorker.currentTargetAngle/90f)*90f));
            PrintMatrix(gridWorker.targetResult);
            
            gridWorker.SetCursorAt((0,5));
            TestContext.Out.WriteLine("result: " + gridWorker.currentTargetAngle + "~" + (float) (Math.Round(gridWorker.currentTargetAngle/90f)*90f));
            PrintMatrix(gridWorker.targetResult);
        }
        
        [Test]
        public void ItShouldPerformTheFullOperation(){
        
            var gridWorker = new GridController.GridWorker();
                    
            float[,] baseM = new float[,] {
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            };
            
            gridWorker.New(baseM, (5,5), (5,5) );
            gridWorker.ApplyRange(new float[,] {
                {0,1,0},
                {1,1,1},
                {0,1,0},
            });
            gridWorker.ApplyTarget(new float[,] {
                {0,1,0},
                {0,0,0},
                {0,0,0},
            });
            
            gridWorker.SetCursorAt((5,0));
            TestContext.Out.WriteLine(" -90 result:");
            
            gridWorker.SetCursorAt((5,4));
            TestContext.Out.WriteLine(" -90 result:");
        }
    }
}