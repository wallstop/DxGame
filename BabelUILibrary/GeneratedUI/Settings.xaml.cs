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
        
        private TextBlock e_1;
        
        private ComboBox e_2;
        
        private TextBlock e_3;
        
        private CheckBox e_4;
        
        private TextBlock e_5;
        
        private ComboBox e_6;
        
        private Button e_7;
        
        private Button e_8;
        
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
            row_e_0_4.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_4);
            RowDefinition row_e_0_5 = new RowDefinition();
            row_e_0_5.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_5);
            RowDefinition row_e_0_6 = new RowDefinition();
            row_e_0_6.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_6);
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
            ColumnDefinition col_e_0_4 = new ColumnDefinition();
            col_e_0_4.Width = new GridLength(1F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_4);
            ColumnDefinition col_e_0_5 = new ColumnDefinition();
            col_e_0_5.Width = new GridLength(1F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_5);
            // e_1 element
            this.e_1 = new TextBlock();
            this.e_0.Children.Add(this.e_1);
            this.e_1.Name = "e_1";
            this.e_1.VerticalAlignment = VerticalAlignment.Center;
            this.e_1.Text = "Resolution";
            // e_2 element
            this.e_2 = new ComboBox();
            this.e_0.Children.Add(this.e_2);
            this.e_2.Name = "e_2";
            this.e_2.Margin = new Thickness(2F, 2F, 2F, 2F);
            this.e_2.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(this.e_2, 1);
            Grid.SetRow(this.e_2, 0);
            Grid.SetColumnSpan(this.e_2, 2);
            Binding binding_e_2_ItemsSource = new Binding("AvailableResolutions");
            this.e_2.SetBinding(ComboBox.ItemsSourceProperty, binding_e_2_ItemsSource);
            Binding binding_e_2_SelectedIndex = new Binding("SelectedResolutionIndex");
            this.e_2.SetBinding(ComboBox.SelectedIndexProperty, binding_e_2_SelectedIndex);
            // e_3 element
            this.e_3 = new TextBlock();
            this.e_0.Children.Add(this.e_3);
            this.e_3.Name = "e_3";
            this.e_3.VerticalAlignment = VerticalAlignment.Center;
            this.e_3.Text = "VSync";
            Grid.SetColumn(this.e_3, 3);
            Grid.SetRow(this.e_3, 0);
            // e_4 element
            this.e_4 = new CheckBox();
            this.e_0.Children.Add(this.e_4);
            this.e_4.Name = "e_4";
            this.e_4.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(this.e_4, 4);
            Grid.SetRow(this.e_4, 0);
            Binding binding_e_4_IsChecked = new Binding("VSync");
            this.e_4.SetBinding(CheckBox.IsCheckedProperty, binding_e_4_IsChecked);
            // e_5 element
            this.e_5 = new TextBlock();
            this.e_0.Children.Add(this.e_5);
            this.e_5.Name = "e_5";
            this.e_5.VerticalAlignment = VerticalAlignment.Center;
            this.e_5.Text = "WindowMode";
            Grid.SetColumn(this.e_5, 0);
            Grid.SetRow(this.e_5, 1);
            // e_6 element
            this.e_6 = new ComboBox();
            this.e_0.Children.Add(this.e_6);
            this.e_6.Name = "e_6";
            this.e_6.Margin = new Thickness(2F, 2F, 2F, 2F);
            this.e_6.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(this.e_6, 1);
            Grid.SetRow(this.e_6, 1);
            Grid.SetColumnSpan(this.e_6, 2);
            Binding binding_e_6_ItemsSource = new Binding("AvailableWindowModes");
            this.e_6.SetBinding(ComboBox.ItemsSourceProperty, binding_e_6_ItemsSource);
            Binding binding_e_6_SelectedIndex = new Binding("SelectedWindowModeIndex");
            this.e_6.SetBinding(ComboBox.SelectedIndexProperty, binding_e_6_SelectedIndex);
            // e_7 element
            this.e_7 = new Button();
            this.e_0.Children.Add(this.e_7);
            this.e_7.Name = "e_7";
            this.e_7.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_7.VerticalAlignment = VerticalAlignment.Center;
            this.e_7.Content = "Apply";
            Grid.SetColumn(this.e_7, 2);
            Grid.SetRow(this.e_7, 5);
            Binding binding_e_7_Command = new Binding("SaveCommand");
            this.e_7.SetBinding(Button.CommandProperty, binding_e_7_Command);
            // e_8 element
            this.e_8 = new Button();
            this.e_0.Children.Add(this.e_8);
            this.e_8.Name = "e_8";
            this.e_8.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_8.VerticalAlignment = VerticalAlignment.Center;
            this.e_8.Content = "Back";
            Grid.SetColumn(this.e_8, 3);
            Grid.SetRow(this.e_8, 5);
            Binding binding_e_8_Command = new Binding("BackCommand");
            this.e_8.SetBinding(Button.CommandProperty, binding_e_8_Command);
        }
    }
}
