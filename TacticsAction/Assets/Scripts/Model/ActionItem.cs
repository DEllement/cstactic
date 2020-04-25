using System;
using System.Collections.Generic;

namespace Model
{
    public class ActionItem
    {
        public Func<List<ActionItem>> Children = ()=>null;
        public ActionType ActionType;
        public Func<bool> Enabled = ()=>true;
        public Func<bool> Visible = ()=>true;
        public bool Executable;
    }
}