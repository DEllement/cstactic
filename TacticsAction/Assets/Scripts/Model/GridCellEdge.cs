using UnityEngine;

namespace Model
{
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
}