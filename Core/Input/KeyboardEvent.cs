using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Input
{
    public enum EventType
    {
        KeyDown,
        KeyUp
    }

    public class KeyboardEvent : InputEvent
    {
        public static readonly Dictionary<Keys, char> KeyCharacters = new Dictionary<Keys, char>
        {
            {Keys.A, 'a'},
            {Keys.B, 'b'},
            {Keys.C, 'c'},
            {Keys.D, 'd'},
            {Keys.E, 'e'},
            {Keys.F, 'f'},
            {Keys.G, 'g'},
            {Keys.H, 'h'},
            {Keys.I, 'i'},
            {Keys.J, 'j'},
            {Keys.K, 'k'},
            {Keys.L, 'l'},
            {Keys.M, 'm'},
            {Keys.N, 'n'},
            {Keys.O, 'o'},
            {Keys.P, 'p'},
            {Keys.Q, 'q'},
            {Keys.R, 'r'},
            {Keys.S, 's'},
            {Keys.T, 't'},
            {Keys.U, 'u'},
            {Keys.V, 'v'},
            {Keys.W, 'w'},
            {Keys.X, 'x'},
            {Keys.Y, 'y'},
            {Keys.Z, 'z'}
        };

        public Keys Key { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan StartTime { get; set; }
    }
}