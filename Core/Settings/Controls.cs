using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Settings
{
    /**

        <summary>
            Captures player controls (every single key/event that can possibly have any in-game effect for the player should be mapped here)
        </summary>
    */
    [DataContract]
    [Serializable]
    public class Controls : AbstractSettings<Controls>
    {
        /*
            TODO: Change to generic Input Events (Keyboard/Mouse) instead of only Keyboard
        */

        [DataMember]
        public Keys Left { get; set; }

        [DataMember]
        public Keys Right { get; set; }

        [DataMember]
        public Keys Jump { get; set; }

        [DataMember]
        public Keys Down { get; set; }

        [DataMember]
        public Keys Attack { get; set; }

        [DataMember]
        public Keys Interact { get; set; }

        [DataMember]
        public Keys Movement { get; set; }

        [DataMember]
        public Keys Ability1 { get; set; }

        [DataMember]
        public Keys Ability2 { get; set; }

        [DataMember]
        public Keys Ability3 { get; set; }

        [DataMember]
        public Keys Ability4 { get; set; }

        public static Controls DefaultControls
            =>
                new Controls
                {
                    Left = Keys.Left,
                    Right = Keys.Right,
                    Jump = Keys.Up,
                    Down = Keys.Down,
                    Attack = Keys.Space,
                    Interact = Keys.F,
                    Movement = Keys.T,
                    Ability1 = Keys.Q,
                    Ability2 = Keys.W,
                    Ability3 = Keys.E,
                    Ability4 = Keys.R
                };

        public static string ControlSettingsPath => "Controls.json";
        public override string Path => ControlSettingsPath;
        public override Controls DefaultSettings => DefaultControls;
        public override Controls CurrentSettings => this;
    }
}