using System.Diagnostics;
using DXGame.Core.Components.Basic;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    public enum PlayerState
    {
        /* These can be expanded on pretty easily, WallClimbing, Running, etc */
        None,
        Walking,
        Jumping
    }

    public class PlayerStateComponent : Component
    {
        public PlayerState State { get; set; }

        public PlayerStateComponent(GameObject parent = null)
            : base(parent)
        {
            State = PlayerState.None;
        }

        public PlayerStateComponent WithState(PlayerState state)
        {
            State = state;
            return this;
        }
    }
}