﻿/* Copyright 2020 Nicolas Mayeur

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    https://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using GalaSoft.MvvmLight.Messaging;
using GeoSports.WPF.ViewModel;
using GeoSports.WPF.ViewModel.Scaffholding;
using System.Windows;
using System.Windows.Navigation;
using static GeoSports.WPF.ViewModel.MainVM;

namespace GeoSports.WPF.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CloseApp : NavigationWindow
    {
        public CloseApp()
        {
            InitializeComponent();
            var vm = DataContext as MainVM; 
            if (vm == null) return; 
            vm.CloseApp += _CloseApp;
        }

        private void _CloseApp(object sender, CloseNotificationEventArgs args)
        {
            this.Close();
        }
    }
}
