using System;
using System.Collections.Generic;
using Controllers.BattleScene.Actions;

namespace Model
{
    public class ActionItem
    {
        public Func<List<ActionItem>> Children = ()=>null;
        public ActionType ActionType;
        public Func<bool> Enabled = ()=>true;
        public Func<bool> Visible = ()=>true;
        public bool Executable;
        public Func<BattleAction> Command = ()=> null;
    }
}