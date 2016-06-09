using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DXGame.Core;

namespace DxCore.Core.GraphicsWidgets.HUD
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