﻿using BCSH2_BDAS2_Armadni_Informacni_System.ViewModels;
using System;
using System.Windows.Controls;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Views
{
    /// <summary>
    /// Interakční logika pro DatabaseObjectsView.xaml
    /// </summary>
    public partial class DatabaseObjectsView : Page
    {
        public DatabaseObjectsView()
        {
            InitializeComponent();
            DataContext = new DatabaseObjectsViewModel();
        }
    }
}
