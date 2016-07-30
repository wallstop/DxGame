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
    public partial class MainMenu : UserControl {
        
        private Grid e_0;
        
        private Button e_1;
        
        private Button e_2;
        
        private Button e_3;
        
        public MainMenu() {
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
            RowDefinition row_e_0_0 = new RowDefinition();
            row_e_0_0.Height = new GridLength(2F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_0);
            RowDefinition row_e_0_1 = new RowDefinition();
            row_e_0_1.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_1);
            RowDefinition row_e_0_2 = new RowDefinition();
            row_e_0_2.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_2);
            RowDefinition row_e_0_3 = new RowDefinition();
            row_e_0_3.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_3);
            RowDefinition row_e_0_4 = new RowDefinition();
            row_e_0_4.Height = new GridLength(3F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_4);
            ColumnDefinition col_e_0_0 = new ColumnDefinition();
            col_e_0_0.Width = new GridLength(1F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_0);
            ColumnDefinition col_e_0_1 = new ColumnDefinition();
            col_e_0_1.Width = new GridLength(1F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_1);
            ColumnDefinition col_e_0_2 = new ColumnDefinition();
            col_e_0_2.Width = new GridLength(1F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_2);
            ColumnDefinition col_e_0_3 = new ColumnDefinition();
            col_e_0_3.Width = new GridLength(1F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_3);
            // e_1 element
            this.e_1 = new Button();
            this.e_0.Children.Add(this.e_1);
            this.e_1.Name = "e_1";
            this.e_1.Foreground = new SolidColorBrush(new ColorW(0, 0, 139, 255));
            this.e_1.TabIndex = 0;
            this.e_1.Content = "Play";
            Grid.SetColumn(this.e_1, 1);
            Grid.SetRow(this.e_1, 1);
            Grid.SetColumnSpan(this.e_1, 2);
            Binding binding_e_1_Command = new Binding("PlayCommand");
            this.e_1.SetBinding(Button.CommandProperty, binding_e_1_Command);
            this.e_1.SetResourceReference(Button.StyleProperty, "ButtonStyle");
            this.e_1.SetResourceReference(Button.FontFamilyProperty, "VisitorFont");
            // e_2 element
            this.e_2 = new Button();
            this.e_0.Children.Add(this.e_2);
            this.e_2.Name = "e_2";
            this.e_2.Foreground = new SolidColorBrush(new ColorW(0, 0, 139, 255));
            this.e_2.TabIndex = 1;
            this.e_2.Content = "Settings";
            Grid.SetColumn(this.e_2, 1);
            Grid.SetRow(this.e_2, 2);
            Grid.SetColumnSpan(this.e_2, 2);
            Binding binding_e_2_Command = new Binding("SettingsCommand");
            this.e_2.SetBinding(Button.CommandProperty, binding_e_2_Command);
            this.e_2.SetResourceReference(Button.StyleProperty, "ButtonStyle");
            this.e_2.SetResourceReference(Button.FontFamilyProperty, "VisitorFont");
            // e_3 element
            this.e_3 = new Button();
            this.e_0.Children.Add(this.e_3);
            this.e_3.Name = "e_3";
            this.e_3.Foreground = new SolidColorBrush(new ColorW(0, 0, 139, 255));
            this.e_3.TabIndex = 2;
            this.e_3.Content = "Quit";
            Grid.SetColumn(this.e_3, 1);
            Grid.SetRow(this.e_3, 3);
            Grid.SetColumnSpan(this.e_3, 2);
            Binding binding_e_3_Command = new Binding("QuitCommand");
            this.e_3.SetBinding(Button.CommandProperty, binding_e_3_Command);
            this.e_3.SetResourceReference(Button.StyleProperty, "ButtonStyle");
            this.e_3.SetResourceReference(Button.FontFamilyProperty, "VisitorFont");
        }
    }
}
