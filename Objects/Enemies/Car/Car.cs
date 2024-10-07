using Godot;
using System;

namespace Game
{
    public class Car : PathFollow2D
    {
#region Signals
#endregion

#region Exported Properties
        [Export(PropertyHint.Range,"5,20,0.1")]
        public float Speed {get;set;} = 5;
#endregion

#region Public Properties
#endregion

#region Internal Properties
        private bool _sndPlayed = false;
#endregion

#region Public Methods
        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if(!_sndPlayed && GlobalPosition.y > 24 && GlobalPosition.y < 360 && GlobalPosition.x > 0 && GlobalPosition.x < 640)
            {
                this.GetNode<AudioStreamPlayer2D>("AppearSound").Play();
                _sndPlayed = true;
            }

            this.Offset += Speed * delta * 10;

            if(this.UnitOffset >= 1f)
            {
                QueueFree();
            }
        }
#endregion

#region Internal Methods
#endregion

#region Signals Hooks
        public void _on_Area2D_body_entered(Node body)
        {
            if(body is Toad toad)
            {
                toad.TriggerDead(this);
            }
        }
#endregion
    }
}
