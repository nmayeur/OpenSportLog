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
