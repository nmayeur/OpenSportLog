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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using OSL.Common.Model;
using OSL.Common.Service;
using OSL.WPF.View;
using OSL.WPF.ViewModel.Scaffholding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using static OSL.WPF.ViewModel.Scaffholding.MessengerNotifications;

namespace OSL.WPF.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainWindowVM : ViewModelBase
    {
        public class CloseNotificationEventArgs : EventArgs
        {
            public CloseNotificationEventArgs()
            {

            }
            public string Message { get; protected set; }
        }

        public event EventHandler<CloseNotificationEventArgs> CloseApp;
        private readonly IDataAccessService _DbAccess;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainWindowVM(IDataAccessService DbAccess)
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            _DbAccess = DbAccess;
            Messenger.Default.Register<CloseDialogMessage>(this, m =>
            {
                CloseApp?.Invoke(this, new CloseNotificationEventArgs());
            });


            // Menu items enable/disable
            Messenger.Default.Register<NotificationMessage<AthleteEntity>>(this, message =>
            {
                if (message.Notification == MessengerNotifications.SELECTED)
                {
                    IsImportEnabled = message.Content != null;
                }
            });
        }

        #region Data
        private bool _IsOslFileOpened = false;
        public bool IsOslFileOpened
        {
            get => _IsOslFileOpened;
            set
            {
                Set(() => IsOslFileOpened, ref _IsOslFileOpened, value);
            }
        }

        private bool _IsImportEnabled = false;
        public bool IsImportEnabled
        {
            get => _IsOslFileOpened;
            set
            {
                Set(() => IsImportEnabled, ref _IsImportEnabled, value);
            }
        }
        #endregion

        #region OpenFileCommand
        private RelayCommand _OpenFileCommand;
        public RelayCommand OpenFileCommand
        {
            get
            {
                return _OpenFileCommand ??
                    (_OpenFileCommand = new RelayCommand(
                        () => { Task.Run(() => OpenFileDialogAsync()); }
                        ));
            }
        }

        private void OpenFileDialogAsync()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "SQLite (*.db,*.sqlite,*.sqlite3)|*.db;*.sqlite;*.sqlite3"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                _DbAccess.OpenDatabase(path);
                Messenger.Default.Send(new NotificationMessage<IList<AthleteEntity>>(_DbAccess.GetAthletes(), MessengerNotifications.LOADED));
                IsOslFileOpened = true;
                IsImportEnabled = false;
            }

        }
        #endregion

        #region NewFileCommand
        private RelayCommand _NewFileCommand;
        public RelayCommand NewFileCommand
        {
            get
            {
                return _NewFileCommand ??
                    (_NewFileCommand = new RelayCommand(
                        () => { NewFileDialog(); }
                        ));
            }
        }

        private void NewFileDialog()
        {
            var openFileDialog = new SaveFileDialog()
            {
                Filter = "SQLite (*.db,*.sqlite,*.sqlite3)|*.db;*.sqlite;*.sqlite3",
                AddExtension = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                _DbAccess.OpenDatabase(path, true);
                Messenger.Default.Send(new NotificationMessage<IList<AthleteEntity>>(null, MessengerNotifications.LOADED));
                IsOslFileOpened = true;
                IsImportEnabled = false;
            }

        }
        #endregion

        #region NewAthleteCommand
        private RelayCommand _NewAthleteCommand;
        public RelayCommand NewAthleteCommand
        {
            get
            {
                return _NewAthleteCommand ??
                    (_NewAthleteCommand = new RelayCommand(
                        () => { NewAthleteDialog(); }
                        ));
            }
        }

        private void NewAthleteDialog()
        {

            NewAthleteDialog dialog = new NewAthleteDialog();
            var dialogDataContext = (NewAthleteVM)dialog.DataContext;
            if (dialog.ShowDialog() ?? false)
            {
                string name = dialogDataContext.Name;
                var athlete = new AthleteEntity { Name = name };
                _DbAccess.AddAthlete(athlete);
                Messenger.Default.Send(new NotificationMessage<AthleteEntity>(athlete, MessengerNotifications.NEW));
            }
        }
        #endregion

        #region ExitCommand
        private RelayCommand _ExitCommand;
        public RelayCommand ExitCommand
        {
            get
            {
                return _ExitCommand ??
                    (_ExitCommand = new RelayCommand(
                        () =>
                        {
                            ExitDialog();
                        }
                        ));
            }
        }

        private void ExitDialog()
        {
            var response = MessageBox.Show("Do you want to quit application?", "Quit confirmation", MessageBoxButton.YesNo);
            if (response == MessageBoxResult.Yes)
            {
                MessengerInstance.Send(new CloseDialogMessage(this));
            }
        }
        #endregion

        #region SaveCommand
        private RelayCommand _SaveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return _SaveCommand ??
                    (_SaveCommand = new RelayCommand(
                        () => { Task.Run(() => Save()); }
                        ));
            }
        }

        private void Save()
        {
            try
            {
                _DbAccess.SaveData();
                MessageBox.Show("Data saved", "Saving");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        #endregion

        #region ImportFitLogCommand
        private RelayCommand _ImportFitLogCommand;
        public RelayCommand ImportFitLogCommand
        {
            get
            {
                return _ImportFitLogCommand ??
                    (_ImportFitLogCommand = new RelayCommand(
                        () => { Task.Run(() => ImportFitLogDialogAsync()); }
                        ));
            }
        }

        private void ImportFitLogDialogAsync()
        {
            Messenger.Default.Send(new NotificationMessage<IMPORT_TYPE>(IMPORT_TYPE.FITLOG, MessengerNotifications.IMPORT));
        }
        #endregion

    }
}