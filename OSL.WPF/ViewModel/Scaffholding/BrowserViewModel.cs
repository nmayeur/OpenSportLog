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
                browser.ExecuteScriptAsync(s);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while executing Javascript: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
