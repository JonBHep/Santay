﻿#pragma checksum "..\..\..\VeloMainWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "576C17C68FF282D6B76C13A2C2588D5172453E9B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Santay {
    
    
    /// <summary>
    /// VeloMainWindow
    /// </summary>
    public partial class VeloMainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\..\VeloMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock PlotTextBlock;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\VeloMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button PlotButton;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\VeloMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer ChartScrollViewer;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\VeloMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas ChartCanvas;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\VeloMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock CountTbkVelo;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\VeloMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock MaximumTbkVelo;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\VeloMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock MeanTbkVelo;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\VeloMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock KmPerDayTbkVelo;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\VeloMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Ellipse PerMonthFlagVelo;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\VeloMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock KmPerMonthTbkVelo;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\..\VeloMainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TodayTextBlock;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.6.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Santay;component/velomainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\VeloMainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.6.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 7 "..\..\..\VeloMainWindow.xaml"
            ((Santay.VeloMainWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\VeloMainWindow.xaml"
            ((Santay.VeloMainWindow)(target)).ContentRendered += new System.EventHandler(this.Window_ContentRendered);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\VeloMainWindow.xaml"
            ((Santay.VeloMainWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.PlotTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.PlotButton = ((System.Windows.Controls.Button)(target));
            
            #line 18 "..\..\..\VeloMainWindow.xaml"
            this.PlotButton.Click += new System.Windows.RoutedEventHandler(this.PlotButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ChartScrollViewer = ((System.Windows.Controls.ScrollViewer)(target));
            
            #line 26 "..\..\..\VeloMainWindow.xaml"
            this.ChartScrollViewer.ScrollChanged += new System.Windows.Controls.ScrollChangedEventHandler(this.ChartScrollViewer_ScrollChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.ChartCanvas = ((System.Windows.Controls.Canvas)(target));
            return;
            case 6:
            this.CountTbkVelo = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.MaximumTbkVelo = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.MeanTbkVelo = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.KmPerDayTbkVelo = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.PerMonthFlagVelo = ((System.Windows.Shapes.Ellipse)(target));
            return;
            case 11:
            this.KmPerMonthTbkVelo = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 12:
            this.TodayTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 13:
            
            #line 105 "..\..\..\VeloMainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CloseButton_Click);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 106 "..\..\..\VeloMainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.RatiosButton_Click);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 107 "..\..\..\VeloMainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DistanceButton_Click);
            
            #line default
            #line hidden
            return;
            case 16:
            
            #line 108 "..\..\..\VeloMainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.WeeklyButton_Click);
            
            #line default
            #line hidden
            return;
            case 17:
            
            #line 109 "..\..\..\VeloMainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DailyButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

