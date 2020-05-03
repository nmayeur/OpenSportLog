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
using GalaSoft.MvvmLight.Threading;
using OSL.WPF.View;
using System;
using System.ComponentModel;
using System.Threading;

namespace OSL.WPF.WPFUtils
{
    public class ViewsHelper
    {
        public static void ExecuteWithSpinner(Action work, string message)
        {
            SpinnerDialog spinnerDialog = null;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                spinnerDialog = new SpinnerDialog();
                spinnerDialog.txtMessage.Text = message;
                spinnerDialog.Show();
            });

            var doneEvent = new AutoResetEvent(false);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                try
                {
                    work();
                }
                finally
                {
                    doneEvent.Set();
                }
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    doneEvent.WaitOne();
                    spinnerDialog.Close();
                });
            };
            worker.RunWorkerAsync();
        }
    }
}
