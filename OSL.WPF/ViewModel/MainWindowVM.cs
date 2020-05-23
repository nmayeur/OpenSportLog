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
using System.Windows.Threading;
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
        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger();
        public class CloseNotificationEventArgs : EventArgs
        {
            public CloseNotificationEventArgs()
            {
            }
        }
        public class SavingNotificationEventArgs : EventArgs
        {
            public SavingNotificationEventArgs(bool IsSaving)
            {
                this.IsSaving = IsSaving;
            }
            public bool IsSaving { get; protected set; }
        }

        public event EventHandler<CloseNotificationEventArgs> CloseApp;
        public event EventHandler<SavingNotificationEventArgs> SavingApp;
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
                    IsAthleteSelected = message.Content != null;
                    IsImportEnabled = message.Content != null;
                }
            });

            SavingApp += _SavingApp;
        }

        #region Data
        private bool _IsAthleteSelected = false;
        public bool IsAthleteSelected
        {
            get => _IsAthleteSelected;
            set
            {
                Set(() => IsAthleteSelected, ref _IsAthleteSelected, value);
            }
        }

        private bool _IsFileOpened = false;
        public bool IsFileOpened
        {
            get => _IsFileOpened;
            set
            {
                Set(() => IsFileOpened, ref _IsFileOpened, value);
            }
        }

        private bool _IsSaveFileEnabled = false;
        public bool IsSaveFileEnabled
        {
            get => _IsSaveFileEnabled;
            set
            {
                Set(() => IsSaveFileEnabled, ref _IsSaveFileEnabled, value && IsFileOpened);
            }
        }

        private bool _IsNewAthleteEnabled = false;
        public bool IsNewAthleteEnabled
        {
            get => _IsNewAthleteEnabled;
            set
            {
                Set(() => IsNewAthleteEnabled, ref _IsNewAthleteEnabled, value && IsFileOpened);
            }
        }

        private bool _IsImportEnabled = false;
        public bool IsImportEnabled
        {
            get => _IsImportEnabled;
            set
            {
                Set(() => IsImportEnabled, ref _IsImportEnabled, value && IsAthleteSelected);
            }
        }

        private bool _IsNewFileEnabled = true;
        public bool IsNewFileEnabled
        {
            get => _IsNewFileEnabled;
            set
            {
                Set(() => IsNewFileEnabled, ref _IsNewFileEnabled, value);
            }
        }

        private bool _IsOpenFileEnabled = true;
        public bool IsOpenFileEnabled
        {
            get => _IsOpenFileEnabled;
            set
            {
                Set(() => IsOpenFileEnabled, ref _IsOpenFileEnabled, value);
            }
        }

        private bool _IsExitEnabled = true;
        public bool IsExitEnabled
        {
            get => _IsExitEnabled;
            set
            {
                Set(() => IsExitEnabled, ref _IsExitEnabled, value);
            }
        }

        private bool _IsProgressbarVisible = false;
        public bool IsProgressbarVisible
        {
            get => _IsProgressbarVisible;
            set
            {
                Set(() => IsProgressbarVisible, ref _IsProgressbarVisible, value);
            }
        }

        private string _ProgressbarText = "Work in progress...";
        public string ProgressbarText
        {
            get => _ProgressbarText;
            set
            {
                Set(() => ProgressbarText, ref _ProgressbarText, value);
            }
        }
        #endregion

        #region Saving mode management
        private void _SavingApp(object sender, SavingNotificationEventArgs args)
        {
            if (args.IsSaving)
            {
                _Logger.Debug("Application enters saving mode");
            }
            else
            {
                _Logger.Debug("Application exits saving mode");
            }

            IsNewAthleteEnabled = IsImportEnabled = IsExitEnabled = IsSaveFileEnabled = IsOpenFileEnabled = IsNewFileEnabled = !args.IsSaving;
        }
        #endregion

        #region LoadInitialDataCommand
        private RelayCommand _LoadInitialDataCommand;
        public RelayCommand LoadInitialDataCommand
        {
            get
            {
                return _LoadInitialDataCommand ??
                    (_LoadInitialDataCommand = new RelayCommand(
                        () => { Task.Run(() => _LoadInitialData()); }
                        ));
            }
        }

        private void _LoadInitialData()
        {
            var lastOpenedFile = Settings.Default.LastOpenedFile;
            if (!string.IsNullOrEmpty(lastOpenedFile))
            {
                _OpenFile(lastOpenedFile);
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
                        () => { Task.Run(() => _OpenFileDialog()); }
                        ));
            }
        }

        private void _OpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "SQLite (*.db,*.sqlite,*.sqlite3)|*.db;*.sqlite;*.sqlite3"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                _OpenFile(path);
            }

        }

        private void _OpenFile(string path)
        {
            ProgressbarText = "Loading file...";
            IsProgressbarVisible = true;
            _DbAccess.OpenDatabase(path);
            Messenger.Default.Send(new NotificationMessage<IList<AthleteEntity>>(_DbAccess.GetAthletes(), MessengerNotifications.LOADED));
            IsFileOpened = true;
            IsSaveFileEnabled = true;
            IsNewAthleteEnabled = true;
            IsImportEnabled = false;
            IsProgressbarVisible = false;
            Settings.Default.LastOpenedFile = path;
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
                        () => { _NewFileDialog(); }
                        ));
            }
        }

        private void _NewFileDialog()
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
                IsFileOpened = true;
                IsSaveFileEnabled = true;
                IsNewAthleteEnabled = true;
                IsImportEnabled = false;
                Settings.Default.LastOpenedFile = path;
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
                        () => { _NewAthleteDialog(); }
                        ));
            }
        }

        private void _NewAthleteDialog()
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
                            _ExitDialog();
                        }
                        ));
            }
        }

        private void _ExitDialog()
        {
            var response = MessageBox.Show("Do you want to quit application?", "Quit confirmation", MessageBoxButton.YesNo);
            if (response == MessageBoxResult.Yes)
            {
                Settings.Default.Save();
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
                        () => { Task.Run(() => _Save()); }
                        ));
            }
        }

        private void _Save()
        {
            try
            {
                ProgressbarText = "Saving file...";
                IsProgressbarVisible = true;
                SavingApp?.Invoke(this, new SavingNotificationEventArgs(IsSaving: true));
                _DbAccess.SaveData();
                MessageBox.Show("Data saved", "Saving");
                IsProgressbarVisible = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                SavingApp?.Invoke(this, new SavingNotificationEventArgs(IsSaving: false));
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
                        () => { Task.Run(() => _ImportFitLogDialogAsync()); }
                        ));
            }
        }

        private void _ImportFitLogDialogAsync()
        {
            Messenger.Default.Send(new NotificationMessage<IMPORT_TYPE>(IMPORT_TYPE.FITLOG, MessengerNotifications.IMPORT));
        }
        #endregion

    }
}