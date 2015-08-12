using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Settings
{
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
        public Keys Attack { get; set; }

        public static Controls DefaultControls
            => new Controls {Left = Keys.Left, Right = Keys.Right, Jump = Keys.Up, Attack = Keys.Space};

        public static string ControlSettingsPath => "Controls.json";
        public override string Path => ControlSettingsPath;
        public override Controls DefaultSettings => DefaultControls;
        public override Controls CurrentSettings => this;

        protected override void CopySettings(Controls other)
        {
            Validate.IsNotNull(other, $"Cannot copy Settings for a null {nameof(Controls)}");
            // TODO: Is there a way to do this with reflection?
            Left = other.Left;
            Right = other.Right;
            Jump = other.Jump;
            Attack = other.Attack;
        }
    }
}