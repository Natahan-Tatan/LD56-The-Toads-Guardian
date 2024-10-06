using Godot;
using System;

namespace Game
{
    public class YellowToad : Toad
    {
        protected override void _EntityEntered(Node entity, bool near)
        {
            base._EntityEntered(entity, near);
        }

        protected override void _EntityExited(Node entity, bool near)
        {
            base._EntityExited(entity, near);
        }
    }
}
