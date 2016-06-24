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
    
    
    [GeneratedCodeAttribute("Empty Keys UI Generator", "2.2.0.0")]
    public partial class Root : UIRoot {
        
        private Grid e_0;
        
        private StackPanel e_1;
        
        private TabControl e_2;
        
        private StackPanel e_7;
        
        private Button e_8;
        
        private Button e_9;
        
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
            this.e_0.RowDefinitions.Add(row_e_0_0);
            RowDefinition row_e_0_1 = new RowDefinition();
            this.e_0.RowDefinitions.Add(row_e_0_1);
            RowDefinition row_e_0_2 = new RowDefinition();
            this.e_0.RowDefinitions.Add(row_e_0_2);
            ColumnDefinition col_e_0_0 = new ColumnDefinition();
            this.e_0.ColumnDefinitions.Add(col_e_0_0);
            ColumnDefinition col_e_0_1 = new ColumnDefinition();
            this.e_0.ColumnDefinitions.Add(col_e_0_1);
            ColumnDefinition col_e_0_2 = new ColumnDefinition();
            this.e_0.ColumnDefinitions.Add(col_e_0_2);
            // e_1 element
            this.e_1 = new StackPanel();
            this.e_0.Children.Add(this.e_1);
            this.e_1.Name = "e_1";
            this.e_1.Margin = new Thickness(5F, 5F, 5F, 5F);
            this.e_1.HorizontalAlignment = HorizontalAlignment.Left;
            this.e_1.Opacity = 0.95F;
            this.e_1.Orientation = Orientation.Horizontal;
            Grid.SetColumn(this.e_1, 0);
            Grid.SetRow(this.e_1, 1);
            // e_2 element
            this.e_2 = new TabControl();
            this.e_1.Children.Add(this.e_2);
            this.e_2.Name = "e_2";
            this.e_2.ItemsSource = Get_e_2_Items();
            // e_7 element
            this.e_7 = new StackPanel();
            this.e_1.Children.Add(this.e_7);
            this.e_7.Name = "e_7";
            this.e_7.Orientation = Orientation.Vertical;
            // e_8 element
            this.e_8 = new Button();
            this.e_7.Children.Add(this.e_8);
            this.e_8.Name = "e_8";
            this.e_8.Content = "Load";
            Binding binding_e_8_Command = new Binding("LoadCommand");
            this.e_8.SetBinding(Button.CommandProperty, binding_e_8_Command);
            // e_9 element
            this.e_9 = new Button();
            this.e_7.Children.Add(this.e_9);
            this.e_9.Name = "e_9";
            this.e_9.Content = "Delete";
            Binding binding_e_9_Command = new Binding("DeleteCommand");
            this.e_9.SetBinding(Button.CommandProperty, binding_e_9_Command);
        }
        
        private static System.Collections.ObjectModel.ObservableCollection<object> Get_e_2_Items() {
            System.Collections.ObjectModel.ObservableCollection<object> items = new System.Collections.ObjectModel.ObservableCollection<object>();
            // e_3 element
            TabItem e_3 = new TabItem();
            e_3.Name = "e_3";
            e_3.Header = "Block";
            // blockData element
            ListBox blockData = new ListBox();
            e_3.Content = blockData;
            blockData.Name = "blockData";
            Func<UIElement, UIElement> blockData_dtFunc = blockData_dtMethod;
            blockData.ItemTemplate = new DataTemplate(blockData_dtFunc);
            DragDrop.SetIsDragSource(blockData, true);
            DragDrop.SetIsDropTarget(blockData, true);
            Binding binding_blockData_ItemsSource = new Binding("Blocks");
            blockData.SetBinding(ListBox.ItemsSourceProperty, binding_blockData_ItemsSource);
            items.Add(e_3);
            // e_5 element
            TabItem e_5 = new TabItem();
            e_5.Name = "e_5";
            e_5.Header = "Platform";
            // platformData element
            ListBox platformData = new ListBox();
            e_5.Content = platformData;
            platformData.Name = "platformData";
            Func<UIElement, UIElement> platformData_dtFunc = platformData_dtMethod;
            platformData.ItemTemplate = new DataTemplate(platformData_dtFunc);
            DragDrop.SetIsDragSource(platformData, true);
            DragDrop.SetIsDropTarget(platformData, true);
            Binding binding_platformData_ItemsSource = new Binding("Platforms");
            platformData.SetBinding(ListBox.ItemsSourceProperty, binding_platformData_ItemsSource);
            items.Add(e_5);
            return items;
        }
        
        private static UIElement blockData_dtMethod(UIElement parent) {
            // e_4 element
            Image e_4 = new Image();
            e_4.Parent = parent;
            e_4.Name = "e_4";
            e_4.MaxHeight = 50F;
            e_4.MinHeight = 50F;
            e_4.MaxWidth = 50F;
            e_4.MinWidth = 50F;
            Binding binding_e_4_Source = new Binding("Tile");
            e_4.SetBinding(Image.SourceProperty, binding_e_4_Source);
            return e_4;
        }
        
        private static UIElement platformData_dtMethod(UIElement parent) {
            // e_6 element
            Image e_6 = new Image();
            e_6.Parent = parent;
            e_6.Name = "e_6";
            Binding binding_e_6_Source = new Binding("Tile");
            e_6.SetBinding(Image.SourceProperty, binding_e_6_Source);
            return e_6;
        }
    }
}
