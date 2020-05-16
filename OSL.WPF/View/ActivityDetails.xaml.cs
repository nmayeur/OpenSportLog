/* Copyright 2020 Nicolas Mayeur

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
using System.Windows.Controls;

namespace OSL.WPF.View
{
    /// <summary>
    /// Interaction logic for ActivityDetails.xaml
    /// </summary>
    public partial class ActivityDetails : UserControl
    {
        public ActivityDetails()
        {
            var settings = new CefSettings();
            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "http",
                DomainName = "internal",
                IsLocal = true,
                SchemeHandlerFactory = new FolderSchemeHandlerFactory(
                    rootFolder: @"WebResources",
                    hostName: "internal",
                    defaultPage: "index.html"
                ),
            });
#if DEBUG
            settings.RemoteDebuggingPort = 8088;
#endif
            Cef.Initialize(settings);
            InitializeComponent();
        }
    }
}
