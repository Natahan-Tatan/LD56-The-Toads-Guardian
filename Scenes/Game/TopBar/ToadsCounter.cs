using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class ToadsCounter : HBoxContainer
    {
#region Signals
#endregion

#region Exported Properties
        [Export]
        public PackedScene PurpleToad {get;set;}

        [Export]
        public PackedScene PurpleToad_OK {get;set;}

        [Export]
        public PackedScene PurpleToad_DEAD {get;set;}

        [Export]
        public PackedScene YellowToad {get;set;}

        [Export]
        public PackedScene YellowToad_OK {get;set;}

        [Export]
        public PackedScene YellowToad_DEAD {get;set;}
#endregion

#region Public Properties
#endregion

#region Internal Properties
        private bool _sortingChildrenCall = false;
#endregion

#region Public Methods
#endregion

#region Internal Methods
        protected void _sortChildren()
        {
            var newOrder = this.GetChildren()
                        .Cast<Control>()
                        .OrderByDescending(c => c.Name.Contains("_ALIVE") ? 1 : c.Name.Contains("_DEAD") ? 0 : -1)
                        .ToList();

            for(int i=0; i<newOrder.Count(); i++)
            {
                this.MoveChild(newOrder[i], i);
            }

             _sortingChildrenCall = false;
        }
#endregion

#region Signals Hooks
        public void _on_ToadsManager_ToadsSpawnFromEgg(IEnumerable<Toad> toads)
        {
            foreach(var toad in toads)
            {
                toad.Connect(nameof(Toad.Arrived), this, nameof(_on_Toad_Arrived));
                toad.Connect(nameof(Toad.Died), this, nameof(_on_Toad_Died));

                if(toad is PurpleToad)
                {
                    this.AddChild(PurpleToad.Instance());
                }
                else
                {
                    this.AddChild(YellowToad.Instance());
                }
            }

            if(!_sortingChildrenCall)
            {
                this.CallDeferred(nameof(_sortChildren)); 
                _sortingChildrenCall = true;
            }
        }

        public void _on_Toad_Arrived(Toad toad)
        {  
            foreach(Node child in this.GetChildren())
            {
                if(toad is PurpleToad && child.Name.Contains($"PurpleToad_ALIVE"))
                {
                    this.RemoveChild(child);
                    this.AddChild(PurpleToad_OK.Instance());

                    break;
                }
                else if(toad is YellowToad && child.Name.Contains($"YellowToad_ALIVE"))
                {
                    this.RemoveChild(child);
                    this.AddChild(YellowToad_OK.Instance());

                    break;
                }
            }  

            if(!_sortingChildrenCall)
            {
                this.CallDeferred(nameof(_sortChildren)); 
                _sortingChildrenCall = true;
            }
        }

        public void _on_Toad_Died(Toad toad, Node killer)
        {
            foreach(Node child in this.GetChildren())
            {
                if(toad is PurpleToad && child.Name.Contains($"PurpleToad_ALIVE"))
                {
                    this.RemoveChild(child);
                    this.AddChild(PurpleToad_DEAD.Instance());

                    break;
                }
                else if(toad is YellowToad && child.Name.Contains($"YellowToad_ALIVE"))
                {
                    this.RemoveChild(child);
                    this.AddChild(YellowToad_DEAD.Instance());

                    break;
                }
            } 

            if(!_sortingChildrenCall)
            {
                this.CallDeferred(nameof(_sortChildren)); 
                _sortingChildrenCall = true;
            }
        }
#endregion
    }

}