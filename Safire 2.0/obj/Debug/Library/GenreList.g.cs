﻿#pragma checksum "..\..\..\Library\GenreList.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4620B735B859453B3D1639BAF874B4EC82939D87"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MahApps.Metro.Controls;
using Safire.Controls;
using Safire.Core;
using Safire.Library;
using Safire.Library.Imaging;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Automation;
using Xceed.Wpf.DataGrid.Controls;
using Xceed.Wpf.DataGrid.Converters;
using Xceed.Wpf.DataGrid.FilterCriteria;
using Xceed.Wpf.DataGrid.Markup;
using Xceed.Wpf.DataGrid.ValidationRules;
using Xceed.Wpf.DataGrid.Views;


namespace Safire.Library {
    
    
    /// <summary>
    /// GenreList
    /// </summary>
    public partial class GenreList : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 15 "..\..\..\Library\GenreList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Safire.Library.GenreList UserControl;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\Library\GenreList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LayoutRoot;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\Library\GenreList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock noresults;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\Library\GenreList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.DataGrid.DataGridControl lst;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\Library\GenreList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock duration;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\Library\GenreList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock artist;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\Library\GenreList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.DataGrid.DataGridControl tracklist;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\Library\GenreList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid selArtist;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Safire 2.0;component/library/genrelist.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Library\GenreList.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.UserControl = ((Safire.Library.GenreList)(target));
            return;
            case 2:
            this.LayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.noresults = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.lst = ((Xceed.Wpf.DataGrid.DataGridControl)(target));
            
            #line 43 "..\..\..\Library\GenreList.xaml"
            this.lst.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.lst_PreviewMouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.duration = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.artist = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.tracklist = ((Xceed.Wpf.DataGrid.DataGridControl)(target));
            
            #line 67 "..\..\..\Library\GenreList.xaml"
            this.tracklist.PreviewMouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.tracklist_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 8:
            this.selArtist = ((System.Windows.Controls.Grid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

