using System.Diagnostics;
using DXGame.Core.Components.Basic;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    public class PlayerStateComponent : Component
    {
        public enum PlayerState
        {
            /* These can be expanded on pretty easily, WallClimbing, Running, etc */
            None,
            Walking,
            Jumping
        }

        public PlayerState State { get; set; }

        public PlayerStateComponent(GameObject parent = null)
            : base(parent)
        {
            
        }

        public PlayerStateComponent WithState(PlayerState state)
        {
            State = state;
            return this;
        }
    }
}