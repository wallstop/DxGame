using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Main;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    /*
        Simple base class for all HUD components that properly deals with assigning draw priority
    */
    [Serializable]
    [DataContract]
    public abstract class HudComponent : DrawableComponent
    {
        protected HudComponent()
        {
            DrawPriority = DrawPriority.HUD_LAYER;
        }
    }
}