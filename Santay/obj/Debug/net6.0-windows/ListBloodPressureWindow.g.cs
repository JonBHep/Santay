﻿#pragma checksum "..\..\..\ListBloodPressureWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "B48458F3F72B123F8DE890BD0F43E0E1F45978CB"
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
    /// ListBloodPressureWindow
    /// </summary>
    public partial class ListBloodPressureWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 39 "..\..\..\ListBloodPressureWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonAdd;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\ListBloodPressureWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonEdit;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\ListBloodPressureWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonDelete;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\ListBloodPressureWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox TensionListBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.9.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Santay;component/listbloodpressurewindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ListBloodPressureWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.9.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 7 "..\..\..\ListBloodPressureWindow.xaml"
            ((Santay.ListBloodPressureWindow)(target)).ContentRendered += new System.EventHandler(this.ListBloodPressureWindow_OnContentRendered);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\ListBloodPressureWindow.xaml"
            ((Santay.ListBloodPressureWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.ListBloodPressureWindow_OnClosing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ButtonAdd = ((System.Windows.Controls.Button)(target));
            
            #line 39 "..\..\..\ListBloodPressureWindow.xaml"
            this.ButtonAdd.Click += new System.Windows.RoutedEventHandler(this.ButtonAdd_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ButtonEdit = ((System.Windows.Controls.Button)(target));
            
            #line 40 "..\..\..\ListBloodPressureWindow.xaml"
            this.ButtonEdit.Click += new System.Windows.RoutedEventHandler(this.ButtonEdit_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ButtonDelete = ((System.Windows.Controls.Button)(target));
            
            #line 41 "..\..\..\ListBloodPressureWindow.xaml"
            this.ButtonDelete.Click += new System.Windows.RoutedEventHandler(this.ButtonDelete_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 42 "..\..\..\ListBloodPressureWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonCloseClick);
            
            #line default
            #line hidden
            return;
            case 6:
            this.TensionListBox = ((System.Windows.Controls.ListBox)(target));
            
            #line 44 "..\..\..\ListBloodPressureWindow.xaml"
            this.TensionListBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ListviewSelectionChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

