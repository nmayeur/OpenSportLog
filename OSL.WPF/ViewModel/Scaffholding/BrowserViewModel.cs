/* Copyright 2021 Nicolas Mayeur

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
using CefSharp.Wpf;
using System;
using System.Windows;

namespace OSL.WPF.ViewModel.Scaffholding
{
    public abstract class BrowserViewModel : OSLViewModel
    {
        protected void ExecuteJavaScript(IWpfWebBrowser browser, string s)
        {
            try
            {
                _Logger.Debug($"Execute Javascript {s}");
                if (browser.IsBrowserInitialized)
                {
                    browser.ExecuteScriptAsync(s);
                }
                else
                {
                    browser.LoadingStateChanged += (sender, args) =>
                    {
                        //Wait for the Page to finish loading
                        if (args.IsLoading == false)
                        {
                            browser.ExecuteScriptAsync(s);
                        }
                    };
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while executing Javascript: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
