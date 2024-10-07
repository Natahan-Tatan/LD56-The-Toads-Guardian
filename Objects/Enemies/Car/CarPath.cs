using Godot;
using System;

namespace Game
{
    public class CarPath : Path2D
    {
#region Signals
#endregion

#region Exported Properties
        [Export]
        public PackedScene Car {get;set;}

        [Export(PropertyHint.Range,"2,30,0.1")]
        public float Interval {get;set;} = 2;

        [Export(PropertyHint.Range,"5,20,0.1")]
        public float Speed {get;set;} = 5;
#endregion

#region Public Properties
#endregion

#region Internal Properties
        protected Timer _spawnCarTimer;
        #endregion

        #region Public Methods
        public override void _Ready()
        {
            base._Ready();

            _spawnCarTimer = this.GetNode<Timer>("SpawnCarTimer");

            _spawnCarTimer.WaitTime = Interval;
        }
        #endregion

        #region Internal Methods
        #endregion

        #region Signals Hooks
        public void _on_SpawnCarTimer_timeout()
        {
            var car = Car.Instance<Car>();
            car.Speed = Speed;

            this.AddChild(car);
        }
#endregion
    }

}