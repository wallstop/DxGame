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
    using EmptyKeys.UserInterface.Media.Effects;
    using EmptyKeys.UserInterface.Media.Animation;
    using EmptyKeys.UserInterface.Media.Imaging;
    using EmptyKeys.UserInterface.Shapes;
    using EmptyKeys.UserInterface.Renderers;
    using EmptyKeys.UserInterface.Themes;
    
    
    [GeneratedCodeAttribute("Empty Keys UI Generator", "2.6.0.0")]
    public partial class Root : UIRoot {
        
        private Grid e_0;
        
        private Grid e_1;
        
        private TextBlock e_2;
        
        private NumericTextBox e_3;
        
        private TextBlock e_4;
        
        private NumericTextBox e_5;
        
        private TextBlock e_6;
        
        private NumericTextBox e_7;
        
        private TextBlock e_8;
        
        private NumericTextBox e_9;
        
        private TextBlock e_10;
        
        private StackPanel e_11;
        
        private RadioButton e_12;
        
        private RadioButton e_13;
        
        public Root() : 
                base() {
            this.Initialize();
        }
        
        public Root(int width, int height) : 
                base(width, height) {
            this.Initialize();
        }
        
        private void Initialize() {
            Style style = RootStyle.CreateRootStyle();
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
            row_e_0_0.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_0);
            RowDefinition row_e_0_1 = new RowDefinition();
            row_e_0_1.Height = new GridLength(2F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_1);
            RowDefinition row_e_0_2 = new RowDefinition();
            row_e_0_2.Height = new GridLength(1F, GridUnitType.Star);
            this.e_0.RowDefinitions.Add(row_e_0_2);
            ColumnDefinition col_e_0_0 = new ColumnDefinition();
            col_e_0_0.Width = new GridLength(1F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_0);
            ColumnDefinition col_e_0_1 = new ColumnDefinition();
            col_e_0_1.Width = new GridLength(1F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_1);
            ColumnDefinition col_e_0_2 = new ColumnDefinition();
            col_e_0_2.Width = new GridLength(1F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_2);
            // e_1 element
            this.e_1 = new Grid();
            this.e_0.Children.Add(this.e_1);
            this.e_1.Name = "e_1";
            RowDefinition row_e_1_0 = new RowDefinition();
            row_e_1_0.Height = new GridLength(1F, GridUnitType.Star);
            this.e_1.RowDefinitions.Add(row_e_1_0);
            RowDefinition row_e_1_1 = new RowDefinition();
            row_e_1_1.Height = new GridLength(1F, GridUnitType.Star);
            this.e_1.RowDefinitions.Add(row_e_1_1);
            RowDefinition row_e_1_2 = new RowDefinition();
            row_e_1_2.Height = new GridLength(1F, GridUnitType.Star);
            this.e_1.RowDefinitions.Add(row_e_1_2);
            RowDefinition row_e_1_3 = new RowDefinition();
            row_e_1_3.Height = new GridLength(1F, GridUnitType.Star);
            this.e_1.RowDefinitions.Add(row_e_1_3);
            RowDefinition row_e_1_4 = new RowDefinition();
            row_e_1_4.Height = new GridLength(1F, GridUnitType.Star);
            this.e_1.RowDefinitions.Add(row_e_1_4);
            ColumnDefinition col_e_1_0 = new ColumnDefinition();
            col_e_1_0.Width = new GridLength(1F, GridUnitType.Star);
            this.e_1.ColumnDefinitions.Add(col_e_1_0);
            ColumnDefinition col_e_1_1 = new ColumnDefinition();
            col_e_1_1.Width = new GridLength(1F, GridUnitType.Star);
            this.e_1.ColumnDefinitions.Add(col_e_1_1);
            Grid.SetColumn(this.e_1, 0);
            Grid.SetRow(this.e_1, 1);
            // e_2 element
            this.e_2 = new TextBlock();
            this.e_1.Children.Add(this.e_2);
            this.e_2.Name = "e_2";
            this.e_2.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_2.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_2.VerticalAlignment = VerticalAlignment.Center;
            this.e_2.Text = "FrameCount";
            Grid.SetColumn(this.e_2, 0);
            Grid.SetRow(this.e_2, 0);
            // e_3 element
            this.e_3 = new NumericTextBox();
            this.e_1.Children.Add(this.e_3);
            this.e_3.Name = "e_3";
            this.e_3.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_3.Minimum = 0F;
            Grid.SetColumn(this.e_3, 1);
            Grid.SetRow(this.e_3, 0);
            Binding binding_e_3_Value = new Binding("FrameCount");
            this.e_3.SetBinding(NumericTextBox.ValueProperty, binding_e_3_Value);
            // e_4 element
            this.e_4 = new TextBlock();
            this.e_1.Children.Add(this.e_4);
            this.e_4.Name = "e_4";
            this.e_4.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_4.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_4.VerticalAlignment = VerticalAlignment.Center;
            this.e_4.Text = "Width";
            Grid.SetColumn(this.e_4, 0);
            Grid.SetRow(this.e_4, 1);
            // e_5 element
            this.e_5 = new NumericTextBox();
            this.e_1.Children.Add(this.e_5);
            this.e_5.Name = "e_5";
            this.e_5.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_5.Minimum = 0F;
            Grid.SetColumn(this.e_5, 1);
            Grid.SetRow(this.e_5, 1);
            Binding binding_e_5_Value = new Binding("FrameWidth");
            this.e_5.SetBinding(NumericTextBox.ValueProperty, binding_e_5_Value);
            // e_6 element
            this.e_6 = new TextBlock();
            this.e_1.Children.Add(this.e_6);
            this.e_6.Name = "e_6";
            this.e_6.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_6.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_6.VerticalAlignment = VerticalAlignment.Center;
            this.e_6.Text = "Height";
            Grid.SetColumn(this.e_6, 0);
            Grid.SetRow(this.e_6, 2);
            // e_7 element
            this.e_7 = new NumericTextBox();
            this.e_1.Children.Add(this.e_7);
            this.e_7.Name = "e_7";
            this.e_7.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_7.Minimum = 0F;
            Grid.SetColumn(this.e_7, 1);
            Grid.SetRow(this.e_7, 2);
            Binding binding_e_7_Value = new Binding("FrameHeight");
            this.e_7.SetBinding(NumericTextBox.ValueProperty, binding_e_7_Value);
            // e_8 element
            this.e_8 = new TextBlock();
            this.e_1.Children.Add(this.e_8);
            this.e_8.Name = "e_8";
            this.e_8.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_8.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_8.VerticalAlignment = VerticalAlignment.Center;
            this.e_8.Text = "FPS";
            Grid.SetColumn(this.e_8, 0);
            Grid.SetRow(this.e_8, 3);
            // e_9 element
            this.e_9 = new NumericTextBox();
            this.e_1.Children.Add(this.e_9);
            this.e_9.Name = "e_9";
            this.e_9.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_9.Minimum = 0F;
            Grid.SetColumn(this.e_9, 1);
            Grid.SetRow(this.e_9, 3);
            Binding binding_e_9_Value = new Binding("FPS");
            this.e_9.SetBinding(NumericTextBox.ValueProperty, binding_e_9_Value);
            // e_10 element
            this.e_10 = new TextBlock();
            this.e_1.Children.Add(this.e_10);
            this.e_10.Name = "e_10";
            this.e_10.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_10.VerticalAlignment = VerticalAlignment.Center;
            this.e_10.Text = "Direction";
            Grid.SetColumn(this.e_10, 0);
            Grid.SetRow(this.e_10, 4);
            // e_11 element
            this.e_11 = new StackPanel();
            this.e_1.Children.Add(this.e_11);
            this.e_11.Name = "e_11";
            this.e_11.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_11.Orientation = Orientation.Horizontal;
            Grid.SetColumn(this.e_11, 1);
            Grid.SetRow(this.e_11, 4);
            // e_12 element
            this.e_12 = new RadioButton();
            this.e_11.Children.Add(this.e_12);
            this.e_12.Name = "e_12";
            this.e_12.Content = "Left";
            this.e_12.GroupName = "Direction";
            Binding binding_e_12_IsChecked = new Binding("FacingLeft");
            this.e_12.SetBinding(RadioButton.IsCheckedProperty, binding_e_12_IsChecked);
            // e_13 element
            this.e_13 = new RadioButton();
            this.e_11.Children.Add(this.e_13);
            this.e_13.Name = "e_13";
            this.e_13.Content = "Right";
            this.e_13.GroupName = "Direction";
            Binding binding_e_13_IsChecked = new Binding("FacingRight");
            this.e_13.SetBinding(RadioButton.IsCheckedProperty, binding_e_13_IsChecked);
        }
    }
}
