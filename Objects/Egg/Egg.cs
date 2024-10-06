using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Egg : StaticBody2D
    {
        public class SetInfos
        {
            public int ToadIndex;
            public int Quantity;
            public float Time;

            public override string ToString()
            {
                return $"{ToadIndex};{Quantity};{Time}";
            }
        }

#region Signals
        [Signal]
        public delegate void ToadSpawning(IEnumerable<Toad> toads);
#endregion

#region Exported Properties
        /// <summary>
        /// The list of eligibles toads type to spawn
        /// </summary>
        [Export]
        public List<PackedScene> Toads {get;set;}
        /// <summary>
        /// String represent a toads group. The format is "<Toad index in array>;<Quantity>;<Time to spawn>".
        /// E.g; "1;3;2.5" means spawn 3 times the 2nd (index starts 0) toads of the list "Toads" in 2.5 seconds
        /// </summary>
        [Export]
        public List<string> Groups
        {
            get => _groups?.Select(s => s.ToString()).ToList();
            
            set => _groups = new Queue<SetInfos>(
                value?.Select(str => {
                    var arr = str.Split(';');

                    return new SetInfos
                    {
                        ToadIndex = int.Parse(arr[0]),
                        Quantity = int.Parse(arr[1]),
                        Time = float.Parse(arr[2])
                    };
                })
            );
        }
        private Queue<SetInfos> _groups;
#endregion

#region Public Properties
        public const int EGG_STEPS = 7;
#endregion

#region Internal Properties
        private Timer _spawnTimer;
        private AnimatedSprite _animations;
        private SetInfos _currentSet;
        private Random _rand = new Random();

#endregion

#region Public Methods
        public override void _Ready()
        {
            base._Ready();

            _spawnTimer = this.GetNode<Timer>("SpawnTimer");
            _animations = this.GetNode<AnimatedSprite>("AnimatedSprite");

            if(Groups == null || Groups.Count <= 0)
            {
                _animations.Frame = EGG_STEPS - 1;
            }
            else
            {
                _on_SpawnTimer_timeout();
            }
        }
#endregion

#region Internal Methods
#endregion

#region Signals Hooks
        public void _on_SpawnTimer_timeout()
        {
            if(_currentSet != null)
            {
                var toads = new List<Toad>(_currentSet.Quantity);

                for(int i=0; i<_currentSet.Quantity; ++i)
                {
                    var toad = Toads[_currentSet.ToadIndex].Instance<Toad>();
                    toad.GlobalPosition = this.GlobalPosition - _rand.NextVector2(15); 
                    toads.Add(toad);
                }

                EmitSignal(nameof(ToadSpawning), toads);

                _currentSet = null;
            }

            if(_groups.Count > 0)
            {
                _currentSet = _groups.Dequeue();
                _animations.Frame = 0;
                _animations.SpeedScale = (float)EGG_STEPS / _currentSet.Time;
                _animations.Playing = true;

                _spawnTimer.WaitTime = _currentSet.Time;
                _spawnTimer.Start();
            }
        }
#endregion
    }

}