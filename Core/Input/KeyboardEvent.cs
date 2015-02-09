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
        private static readonly TimeSpan HELD_DOWN_TRHESHOLD = TimeSpan.FromSeconds(1.0f / 2.0f);

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
            {Keys.Z, 'z'},
            {Keys.Space, ' '},
            {Keys.D0, '0'},
            {Keys.NumPad0, '0'},
            {Keys.D1, '1'},
            {Keys.NumPad1, '1'},
            {Keys.D2, '2'},
            {Keys.NumPad2, '2'},
            {Keys.D3, '3'},
            {Keys.NumPad3, '3'},
            {Keys.D4, '4'},
            {Keys.NumPad4, '4'},
            {Keys.D5, '5'},
            {Keys.NumPad5, '5'},
            {Keys.D6, '6'},
            {Keys.NumPad6, '6'},
            {Keys.D7, '7'},
            {Keys.NumPad7, '7'},
            {Keys.D8, '8'},
            {Keys.NumPad8, '8'},
            {Keys.D9, '9'},
            {Keys.NumPad9, '9'},
            {Keys.OemComma, ','},
            {Keys.OemPeriod, '.'},
            {Keys.OemQuestion, '?'},
            {Keys.OemPlus, '+'},
            {Keys.OemQuotes, '"'},
            {Keys.OemSemicolon, ';'},
            {Keys.OemOpenBrackets, '{'},
            {Keys.OemTilde, '`'},
            {Keys.OemBackslash, '\\'},
            {Keys.OemMinus, '-'},
            {Keys.OemPipe, '|'}
        };

        public Keys Key { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan StartTime { get; set; }

        public bool HeldDown
        {
            get { return Duration >= HELD_DOWN_TRHESHOLD; }
        }
    }
}