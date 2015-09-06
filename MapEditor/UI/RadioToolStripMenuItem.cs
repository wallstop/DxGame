﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using DXGame.Core.Utils;

namespace MapEditor.UI
{

    public delegate void Action();
    /*
        https://msdn.microsoft.com/en-us/library/ms404318.aspx
    */
    public class RadioToolStripMenuItem : ToolStripMenuItem
    {
        private readonly Action action_;
        public RadioToolStripMenuItem(Action action)
        {
            Validate.IsNotNull(action, StringUtils.GetFormattedNullOrDefaultMessage(this, action));
            action_ = action;
            Initialize();
        }

        private void Initialize()
        {
            CheckOnClick = true;
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);

            // If this item is no longer in the checked state or if its  
            // parent has not yet been initialized, do nothing. 
            if (!Checked || this.Parent == null) return;

            /* Execute whatever action corresponds to us */
            action_();

            // Clear the checked state for all siblings.  
            foreach (ToolStripItem item in Parent.Items)
            {
                RadioToolStripMenuItem radioItem =
                    item as RadioToolStripMenuItem;
                if (radioItem != null && radioItem != this && radioItem.Checked)
                {
                    radioItem.Checked = false;

                    // Only one item can be selected at a time,  
                    // so there is no need to continue. 
                    return;
                }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            // If the item is already in the checked state, do not call  
            // the base method, which would toggle the value.  
            if (Checked) return;

            base.OnClick(e);
        }

        // Let the item paint itself, and then paint the RadioButton 
        // where the check mark is normally displayed. 
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image != null)
            {
                // If the client sets the Image property, the selection behavior 
                // remains unchanged, but the RadioButton is not displayed and the 
                // selection is indicated only by the selection rectangle.  
                base.OnPaint(e);
                return;
            }
            else
            {
                // If the Image property is not set, call the base OnPaint method  
                // with the CheckState property temporarily cleared to prevent 
                // the check mark from being painted.
                CheckState currentState = this.CheckState;
                this.CheckState = CheckState.Unchecked;
                base.OnPaint(e);
                this.CheckState = currentState;
            }

            // Determine the correct state of the RadioButton.
            RadioButtonState buttonState = RadioButtonState.UncheckedNormal;
            if (Enabled)
            {
                if (mouseDownState)
                {
                    if (Checked) buttonState = RadioButtonState.CheckedPressed;
                    else buttonState = RadioButtonState.UncheckedPressed;
                }
                else if (mouseHoverState)
                {
                    if (Checked) buttonState = RadioButtonState.CheckedHot;
                    else buttonState = RadioButtonState.UncheckedHot;
                }
                else
                {
                    if (Checked) buttonState = RadioButtonState.CheckedNormal;
                }
            }
            else
            {
                if (Checked) buttonState = RadioButtonState.CheckedDisabled;
                else buttonState = RadioButtonState.UncheckedDisabled;
            }

            // Calculate the position at which to display the RadioButton.
            Int32 offset = (ContentRectangle.Height -
                RadioButtonRenderer.GetGlyphSize(
                e.Graphics, buttonState).Height) / 2;
            Point imageLocation = new Point(
                ContentRectangle.Location.X + 4,
                ContentRectangle.Location.Y + offset);

            // Paint the RadioButton. 
            RadioButtonRenderer.DrawRadioButton(
                e.Graphics, imageLocation, buttonState);
        }

        private bool mouseHoverState = false;

        protected override void OnMouseEnter(EventArgs e)
        {
            mouseHoverState = true;

            // Force the item to repaint with the new RadioButton state.
            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            mouseHoverState = false;
            base.OnMouseLeave(e);
        }

        private bool mouseDownState = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            mouseDownState = true;

            // Force the item to repaint with the new RadioButton state.
            Invalidate();

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseDownState = false;
            base.OnMouseUp(e);
        }
    }
}