// -----------------------------------------------------------
//  
//  This file was generated, please do not modify.
//  
// -----------------------------------------------------------
namespace EmptyKeys.UserInterface.Generated {
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using EmptyKeys.UserInterface;
    using EmptyKeys.UserInterface.Charts;
    using EmptyKeys.UserInterface.Data;
    using EmptyKeys.UserInterface.Controls;
    using EmptyKeys.UserInterface.Controls.Primitives;
    using EmptyKeys.UserInterface.Input;
    using EmptyKeys.UserInterface.Interactions.Core;
    using EmptyKeys.UserInterface.Interactivity;
    using EmptyKeys.UserInterface.Media;
    using EmptyKeys.UserInterface.Media.Animation;
    using EmptyKeys.UserInterface.Media.Imaging;
    using EmptyKeys.UserInterface.Shapes;
    using EmptyKeys.UserInterface.Renderers;
    using EmptyKeys.UserInterface.Themes;
    
    
    [GeneratedCodeAttribute("Empty Keys UI Generator", "2.3.0.0")]
    public partial class Settings : UserControl {
        
        private Grid e_0;
        
        private ComboBox Resolution;
        
        public Settings() {
            Style style = UserControlStyle.CreateUserControlStyle();
            style.TargetType = this.GetType();
            this.Style = style;
            this.InitializeComponent();
        }
        
        private void InitializeComponent() {
            // e_0 element
            this.e_0 = new Grid();
            this.Content = this.e_0;
            this.e_0.Name = "e_0";
            // Resolution element
            this.Resolution = new ComboBox();
            this.e_0.Children.Add(this.Resolution);
            this.Resolution.Name = "Resolution";
            Binding binding_Resolution_ItemsSource = new Binding("AvailableResolutions");
            this.Resolution.SetBinding(ComboBox.ItemsSourceProperty, binding_Resolution_ItemsSource);
            Binding binding_Resolution_SelectedIndex = new Binding("SelectedResolutionIndex");
            this.Resolution.SetBinding(ComboBox.SelectedIndexProperty, binding_Resolution_SelectedIndex);
        }
    }
}
