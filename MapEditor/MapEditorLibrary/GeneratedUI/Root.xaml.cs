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
        
        private Slider e_3;
        
        private TextBlock e_4;
        
        private TextBlock e_5;
        
        private Slider e_6;
        
        private TextBlock e_7;
        
        private TextBlock e_8;
        
        private Slider e_9;
        
        private TextBlock e_10;
        
        private StackPanel e_11;
        
        private TabControl e_12;
        
        private Grid e_29;
        
        private Button e_30;
        
        private Button e_31;
        
        private Button e_32;
        
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
            this.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this.Background.Opacity = 0F;
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
            this.e_0.ColumnDefinitions.Add(col_e_0_0);
            ColumnDefinition col_e_0_1 = new ColumnDefinition();
            this.e_0.ColumnDefinitions.Add(col_e_0_1);
            ColumnDefinition col_e_0_2 = new ColumnDefinition();
            this.e_0.ColumnDefinitions.Add(col_e_0_2);
            // e_1 element
            this.e_1 = new Grid();
            this.e_0.Children.Add(this.e_1);
            this.e_1.Name = "e_1";
            this.e_1.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_1.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_1.VerticalAlignment = VerticalAlignment.Center;
            this.e_1.Opacity = 0.9F;
            this.e_1.Background = new SolidColorBrush(new ColorW(0, 0, 0, 255));
            RowDefinition row_e_1_0 = new RowDefinition();
            this.e_1.RowDefinitions.Add(row_e_1_0);
            RowDefinition row_e_1_1 = new RowDefinition();
            this.e_1.RowDefinitions.Add(row_e_1_1);
            RowDefinition row_e_1_2 = new RowDefinition();
            this.e_1.RowDefinitions.Add(row_e_1_2);
            ColumnDefinition col_e_1_0 = new ColumnDefinition();
            col_e_1_0.Width = new GridLength(2F, GridUnitType.Star);
            this.e_1.ColumnDefinitions.Add(col_e_1_0);
            ColumnDefinition col_e_1_1 = new ColumnDefinition();
            col_e_1_1.Width = new GridLength(3F, GridUnitType.Star);
            this.e_1.ColumnDefinitions.Add(col_e_1_1);
            ColumnDefinition col_e_1_2 = new ColumnDefinition();
            col_e_1_2.Width = new GridLength(1F, GridUnitType.Star);
            this.e_1.ColumnDefinitions.Add(col_e_1_2);
            Grid.SetColumn(this.e_1, 0);
            Grid.SetRow(this.e_1, 0);
            // e_2 element
            this.e_2 = new TextBlock();
            this.e_1.Children.Add(this.e_2);
            this.e_2.Name = "e_2";
            this.e_2.HorizontalAlignment = HorizontalAlignment.Right;
            this.e_2.VerticalAlignment = VerticalAlignment.Center;
            this.e_2.Text = "Map Tile Size";
            Grid.SetColumn(this.e_2, 0);
            Grid.SetRow(this.e_2, 0);
            // e_3 element
            this.e_3 = new Slider();
            this.e_1.Children.Add(this.e_3);
            this.e_3.Name = "e_3";
            this.e_3.Width = 180F;
            this.e_3.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_3.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_3.VerticalAlignment = VerticalAlignment.Center;
            this.e_3.Minimum = 20F;
            this.e_3.Maximum = 100F;
            this.e_3.IsSnapToTickEnabled = true;
            this.e_3.TickFrequency = 5F;
            Grid.SetColumn(this.e_3, 1);
            Grid.SetRow(this.e_3, 0);
            Binding binding_e_3_Value = new Binding("TileSize");
            this.e_3.SetBinding(Slider.ValueProperty, binding_e_3_Value);
            // e_4 element
            this.e_4 = new TextBlock();
            this.e_1.Children.Add(this.e_4);
            this.e_4.Name = "e_4";
            this.e_4.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_4.HorizontalAlignment = HorizontalAlignment.Left;
            this.e_4.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(this.e_4, 2);
            Grid.SetRow(this.e_4, 0);
            Binding binding_e_4_Text = new Binding("TileSize");
            this.e_4.SetBinding(TextBlock.TextProperty, binding_e_4_Text);
            // e_5 element
            this.e_5 = new TextBlock();
            this.e_1.Children.Add(this.e_5);
            this.e_5.Name = "e_5";
            this.e_5.HorizontalAlignment = HorizontalAlignment.Right;
            this.e_5.VerticalAlignment = VerticalAlignment.Center;
            this.e_5.Text = "Map Width";
            Grid.SetColumn(this.e_5, 0);
            Grid.SetRow(this.e_5, 1);
            // e_6 element
            this.e_6 = new Slider();
            this.e_1.Children.Add(this.e_6);
            this.e_6.Name = "e_6";
            this.e_6.Width = 180F;
            this.e_6.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_6.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_6.VerticalAlignment = VerticalAlignment.Center;
            this.e_6.Minimum = 10F;
            this.e_6.Maximum = 150F;
            this.e_6.IsSnapToTickEnabled = true;
            this.e_6.TickFrequency = 5F;
            Grid.SetColumn(this.e_6, 1);
            Grid.SetRow(this.e_6, 1);
            Binding binding_e_6_Value = new Binding("MapWidth");
            this.e_6.SetBinding(Slider.ValueProperty, binding_e_6_Value);
            // e_7 element
            this.e_7 = new TextBlock();
            this.e_1.Children.Add(this.e_7);
            this.e_7.Name = "e_7";
            this.e_7.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_7.HorizontalAlignment = HorizontalAlignment.Left;
            this.e_7.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(this.e_7, 2);
            Grid.SetRow(this.e_7, 1);
            Binding binding_e_7_Text = new Binding("MapWidth");
            this.e_7.SetBinding(TextBlock.TextProperty, binding_e_7_Text);
            // e_8 element
            this.e_8 = new TextBlock();
            this.e_1.Children.Add(this.e_8);
            this.e_8.Name = "e_8";
            this.e_8.HorizontalAlignment = HorizontalAlignment.Right;
            this.e_8.VerticalAlignment = VerticalAlignment.Center;
            this.e_8.Text = "Map Height";
            Grid.SetColumn(this.e_8, 0);
            Grid.SetRow(this.e_8, 2);
            // e_9 element
            this.e_9 = new Slider();
            this.e_1.Children.Add(this.e_9);
            this.e_9.Name = "e_9";
            this.e_9.Width = 180F;
            this.e_9.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_9.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_9.VerticalAlignment = VerticalAlignment.Center;
            this.e_9.Minimum = 10F;
            this.e_9.Maximum = 150F;
            this.e_9.IsSnapToTickEnabled = true;
            this.e_9.TickFrequency = 5F;
            Grid.SetColumn(this.e_9, 1);
            Grid.SetRow(this.e_9, 2);
            Binding binding_e_9_Value = new Binding("MapHeight");
            this.e_9.SetBinding(Slider.ValueProperty, binding_e_9_Value);
            // e_10 element
            this.e_10 = new TextBlock();
            this.e_1.Children.Add(this.e_10);
            this.e_10.Name = "e_10";
            this.e_10.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_10.HorizontalAlignment = HorizontalAlignment.Left;
            this.e_10.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(this.e_10, 2);
            Grid.SetRow(this.e_10, 2);
            Binding binding_e_10_Text = new Binding("MapHeight");
            this.e_10.SetBinding(TextBlock.TextProperty, binding_e_10_Text);
            // e_11 element
            this.e_11 = new StackPanel();
            this.e_0.Children.Add(this.e_11);
            this.e_11.Name = "e_11";
            this.e_11.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_11.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.e_11.VerticalAlignment = VerticalAlignment.Center;
            this.e_11.Opacity = 0.9F;
            this.e_11.Background = new SolidColorBrush(new ColorW(0, 0, 0, 255));
            this.e_11.Orientation = Orientation.Horizontal;
            Grid.SetColumn(this.e_11, 0);
            Grid.SetRow(this.e_11, 1);
            // e_12 element
            this.e_12 = new TabControl();
            this.e_11.Children.Add(this.e_12);
            this.e_12.Name = "e_12";
            this.e_12.Margin = new Thickness(2F, 2F, 2F, 2F);
            this.e_12.ItemsSource = Get_e_12_Items();
            // e_29 element
            this.e_29 = new Grid();
            this.e_11.Children.Add(this.e_29);
            this.e_29.Name = "e_29";
            this.e_29.Margin = new Thickness(2F, 2F, 2F, 2F);
            RowDefinition row_e_29_0 = new RowDefinition();
            row_e_29_0.Height = new GridLength(1F, GridUnitType.Star);
            this.e_29.RowDefinitions.Add(row_e_29_0);
            RowDefinition row_e_29_1 = new RowDefinition();
            row_e_29_1.Height = new GridLength(1F, GridUnitType.Star);
            this.e_29.RowDefinitions.Add(row_e_29_1);
            RowDefinition row_e_29_2 = new RowDefinition();
            row_e_29_2.Height = new GridLength(1F, GridUnitType.Star);
            this.e_29.RowDefinitions.Add(row_e_29_2);
            RowDefinition row_e_29_3 = new RowDefinition();
            row_e_29_3.Height = new GridLength(1F, GridUnitType.Star);
            this.e_29.RowDefinitions.Add(row_e_29_3);
            RowDefinition row_e_29_4 = new RowDefinition();
            row_e_29_4.Height = new GridLength(1F, GridUnitType.Star);
            this.e_29.RowDefinitions.Add(row_e_29_4);
            RowDefinition row_e_29_5 = new RowDefinition();
            row_e_29_5.Height = new GridLength(1F, GridUnitType.Star);
            this.e_29.RowDefinitions.Add(row_e_29_5);
            // e_30 element
            this.e_30 = new Button();
            this.e_29.Children.Add(this.e_30);
            this.e_30.Name = "e_30";
            this.e_30.Margin = new Thickness(2F, 2F, 2F, 2F);
            this.e_30.Content = "Load Map";
            Grid.SetRow(this.e_30, 0);
            Binding binding_e_30_Command = new Binding("LoadMapCommand");
            this.e_30.SetBinding(Button.CommandProperty, binding_e_30_Command);
            // e_31 element
            this.e_31 = new Button();
            this.e_29.Children.Add(this.e_31);
            this.e_31.Name = "e_31";
            this.e_31.Margin = new Thickness(2F, 2F, 2F, 2F);
            this.e_31.Content = "Save Map";
            Grid.SetRow(this.e_31, 1);
            Binding binding_e_31_Command = new Binding("SaveMapCommand");
            this.e_31.SetBinding(Button.CommandProperty, binding_e_31_Command);
            // e_32 element
            this.e_32 = new Button();
            this.e_29.Children.Add(this.e_32);
            this.e_32.Name = "e_32";
            this.e_32.Margin = new Thickness(2F, 2F, 2F, 2F);
            this.e_32.Content = "Reset";
            Grid.SetRow(this.e_32, 2);
            Binding binding_e_32_Command = new Binding("ResetCommand");
            this.e_32.SetBinding(Button.CommandProperty, binding_e_32_Command);
        }
        
        private static System.Collections.ObjectModel.ObservableCollection<object> Get_e_12_Items() {
            System.Collections.ObjectModel.ObservableCollection<object> items = new System.Collections.ObjectModel.ObservableCollection<object>();
            // e_13 element
            TabItem e_13 = new TabItem();
            e_13.Name = "e_13";
            e_13.Margin = new Thickness(2F, 2F, 2F, 2F);
            e_13.Header = "Block";
            // e_14 element
            Grid e_14 = new Grid();
            e_13.Content = e_14;
            e_14.Name = "e_14";
            RowDefinition row_e_14_0 = new RowDefinition();
            row_e_14_0.Height = new GridLength(1F, GridUnitType.Star);
            e_14.RowDefinitions.Add(row_e_14_0);
            RowDefinition row_e_14_1 = new RowDefinition();
            row_e_14_1.Height = new GridLength(5F, GridUnitType.Star);
            e_14.RowDefinitions.Add(row_e_14_1);
            // e_15 element
            Grid e_15 = new Grid();
            e_14.Children.Add(e_15);
            e_15.Name = "e_15";
            ColumnDefinition col_e_15_0 = new ColumnDefinition();
            e_15.ColumnDefinitions.Add(col_e_15_0);
            ColumnDefinition col_e_15_1 = new ColumnDefinition();
            e_15.ColumnDefinitions.Add(col_e_15_1);
            // e_16 element
            Button e_16 = new Button();
            e_15.Children.Add(e_16);
            e_16.Name = "e_16";
            e_16.Margin = new Thickness(5F, 5F, 5F, 5F);
            e_16.Content = "Load";
            Grid.SetRow(e_16, 0);
            Binding binding_e_16_Command = new Binding("LoadBlockCommand");
            e_16.SetBinding(Button.CommandProperty, binding_e_16_Command);
            // e_17 element
            ScrollViewer e_17 = new ScrollViewer();
            e_14.Children.Add(e_17);
            e_17.Name = "e_17";
            Grid.SetRow(e_17, 1);
            // e_18 element
            ListBox e_18 = new ListBox();
            e_17.Content = e_18;
            e_18.Name = "e_18";
            Func<UIElement, UIElement> e_18_iptFunc = e_18_iptMethod;
            ControlTemplate e_18_ipt = new ControlTemplate(e_18_iptFunc);
            e_18.ItemsPanel = e_18_ipt;
            Func<UIElement, UIElement> e_18_dtFunc = e_18_dtMethod;
            e_18.ItemTemplate = new DataTemplate(e_18_dtFunc);
            Binding binding_e_18_ItemsSource = new Binding("Blocks");
            e_18.SetBinding(ListBox.ItemsSourceProperty, binding_e_18_ItemsSource);
            Binding binding_e_18_SelectedItem = new Binding("SelectedBlock");
            binding_e_18_SelectedItem.Mode = BindingMode.OneWayToSource;
            e_18.SetBinding(ListBox.SelectedItemProperty, binding_e_18_SelectedItem);
            items.Add(e_13);
            // e_21 element
            TabItem e_21 = new TabItem();
            e_21.Name = "e_21";
            e_21.Margin = new Thickness(2F, 2F, 2F, 2F);
            e_21.Header = "Platform";
            // e_22 element
            Grid e_22 = new Grid();
            e_21.Content = e_22;
            e_22.Name = "e_22";
            RowDefinition row_e_22_0 = new RowDefinition();
            row_e_22_0.Height = new GridLength(1F, GridUnitType.Star);
            e_22.RowDefinitions.Add(row_e_22_0);
            RowDefinition row_e_22_1 = new RowDefinition();
            row_e_22_1.Height = new GridLength(5F, GridUnitType.Star);
            e_22.RowDefinitions.Add(row_e_22_1);
            // e_23 element
            Grid e_23 = new Grid();
            e_22.Children.Add(e_23);
            e_23.Name = "e_23";
            ColumnDefinition col_e_23_0 = new ColumnDefinition();
            e_23.ColumnDefinitions.Add(col_e_23_0);
            ColumnDefinition col_e_23_1 = new ColumnDefinition();
            e_23.ColumnDefinitions.Add(col_e_23_1);
            // e_24 element
            Button e_24 = new Button();
            e_23.Children.Add(e_24);
            e_24.Name = "e_24";
            e_24.Margin = new Thickness(5F, 5F, 5F, 5F);
            e_24.Content = "Load";
            Grid.SetRow(e_24, 0);
            Binding binding_e_24_Command = new Binding("LoadPlatformCommand");
            e_24.SetBinding(Button.CommandProperty, binding_e_24_Command);
            // e_25 element
            ScrollViewer e_25 = new ScrollViewer();
            e_22.Children.Add(e_25);
            e_25.Name = "e_25";
            Grid.SetRow(e_25, 1);
            // e_26 element
            ListBox e_26 = new ListBox();
            e_25.Content = e_26;
            e_26.Name = "e_26";
            Func<UIElement, UIElement> e_26_iptFunc = e_26_iptMethod;
            ControlTemplate e_26_ipt = new ControlTemplate(e_26_iptFunc);
            e_26.ItemsPanel = e_26_ipt;
            Func<UIElement, UIElement> e_26_dtFunc = e_26_dtMethod;
            e_26.ItemTemplate = new DataTemplate(e_26_dtFunc);
            Binding binding_e_26_ItemsSource = new Binding("Platforms");
            e_26.SetBinding(ListBox.ItemsSourceProperty, binding_e_26_ItemsSource);
            Binding binding_e_26_SelectedItem = new Binding("SelectedPlatform");
            binding_e_26_SelectedItem.Mode = BindingMode.OneWayToSource;
            e_26.SetBinding(ListBox.SelectedItemProperty, binding_e_26_SelectedItem);
            items.Add(e_21);
            return items;
        }
        
        private static UIElement e_18_iptMethod(UIElement parent) {
            // e_19 element
            WrapPanel e_19 = new WrapPanel();
            e_19.Parent = parent;
            e_19.Name = "e_19";
            e_19.Height = float.NaN;
            e_19.MaxWidth = 180F;
            e_19.IsItemsHost = true;
            return e_19;
        }
        
        private static UIElement e_18_dtMethod(UIElement parent) {
            // e_20 element
            Image e_20 = new Image();
            e_20.Parent = parent;
            e_20.Name = "e_20";
            e_20.MaxHeight = 50F;
            e_20.MinHeight = 50F;
            e_20.MaxWidth = 50F;
            e_20.MinWidth = 50F;
            e_20.Margin = new Thickness(5F, 5F, 5F, 5F);
            Binding binding_e_20_Source = new Binding("Tile");
            e_20.SetBinding(Image.SourceProperty, binding_e_20_Source);
            return e_20;
        }
        
        private static UIElement e_26_iptMethod(UIElement parent) {
            // e_27 element
            WrapPanel e_27 = new WrapPanel();
            e_27.Parent = parent;
            e_27.Name = "e_27";
            e_27.Height = float.NaN;
            e_27.MaxWidth = 180F;
            e_27.IsItemsHost = true;
            return e_27;
        }
        
        private static UIElement e_26_dtMethod(UIElement parent) {
            // e_28 element
            Image e_28 = new Image();
            e_28.Parent = parent;
            e_28.Name = "e_28";
            e_28.MaxHeight = 50F;
            e_28.MinHeight = 50F;
            e_28.MaxWidth = 50F;
            e_28.MinWidth = 50F;
            e_28.Margin = new Thickness(5F, 5F, 5F, 5F);
            Binding binding_e_28_Source = new Binding("Tile");
            e_28.SetBinding(Image.SourceProperty, binding_e_28_Source);
            return e_28;
        }
    }
}
