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
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using OSL.WPF.ViewModel;
using OSL.WPF.ViewModel.Scaffholding;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using static OSL.WPF.ViewModel.MainWindowVM;

namespace OSL.WPF.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowVM _VM;
        private bool _IsClosing = false;

        public MainWindow()
        {
            InitializeComponent();
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            _VM = DataContext as MainWindowVM;
            if (_VM == null) return;
            _VM.PropertyChanged += _OnPropertyChanged;
            Closing += _OnWindowClosing;
            Messenger.Default.Register<CloseDialogMessage>(this, m =>
            {
                if(!_IsClosing) this.Close();
            });
        }

        private void _OnWindowClosing(object sender, CancelEventArgs e)
        {
            _IsClosing = true;
            _VM.OnWindowClosing(sender, e);
        }

        private void _OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(_VM.IsProgressbarVisible))
            {
                DispatcherHelper.CheckBeginInvokeOnUI(
                    () => { rowProgressbar.Height = _VM.IsProgressbarVisible ? new GridLength(20) : new GridLength(0); }
                );

            }
        }
    }
}
