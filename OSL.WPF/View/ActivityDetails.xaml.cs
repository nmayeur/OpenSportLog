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
using CefSharp;
using CefSharp.SchemeHandler;
using CefSharp.Wpf;
using OSL.Common.Model.ECharts;
using OSL.WPF.ViewModel;
using System.ComponentModel;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;

namespace OSL.WPF.View
{
    /// <summary>
    /// Interaction logic for ActivityDetails.xaml
    /// </summary>
    public partial class ActivityDetails : UserControl
    {
        private readonly ActivityDetailsVM _VM;

        public ActivityDetails()
        {
            InitializeComponent();
            BrowserActivityCharts.JavascriptMessageReceived += _OnBrowserJavascriptMessageReceived;
            _VM = DataContext as ActivityDetailsVM;
        }


        private void _OnBrowserJavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            dynamic msg = e.ConvertMessageTo<ExpandoObject>();
            switch (msg.type)
            {
                case "datazoom":
                    var datazoom = e.ConvertMessageTo<DataZoomModel>();
                    _VM.OnDataZoom(datazoom, new CancelEventArgs() { Cancel = false });
                    break;
            }

        }
    }
}
