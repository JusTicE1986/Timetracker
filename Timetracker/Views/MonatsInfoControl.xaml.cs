﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Timetracker.ViewModels;

namespace Timetracker.Views
{
    /// <summary>
    /// Interaktionslogik für MonatsInfoControl.xaml
    /// </summary>
    public partial class MonatsInfoControl : UserControl
    {
        public MonatsInfoControl()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
                DataContext = new MainViewModel();
        }
    }
}
