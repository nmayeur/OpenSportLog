﻿/* Copyright 2021 Nicolas Mayeur

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
using OSL.Common.Model;
using System;
using System.Linq;
using System.Windows;

namespace OSL.WPF.View
{
    /// <summary>
    /// Interaction logic for ImportSportsMatchingDialog.xaml
    /// </summary>
    public partial class ImportSportsMatchingDialog : Window
    {
        public ImportSportsMatchingDialog()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
