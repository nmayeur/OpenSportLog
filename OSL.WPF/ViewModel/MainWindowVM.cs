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
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using OSL.Common.Model;
using OSL.Common.Service;
using OSL.Common.Service.Importer;
using OSL.WPF.Properties;
using OSL.WPF.View;
using OSL.WPF.ViewModel.Scaffholding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static OSL.EF.Service.DataAccessService;
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
        public class SavingNotificationEventArgs : EventArgs
        {
            public SavingNotificationEventArgs(bool IsSaving)
            {
                this.IsSaving = IsSaving;
            }
            public bool IsSaving { get; protected set; }
        }

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


            // Menu items enable/disable
            Messenger.Default.Register<NotificationMessage<AthleteEntity>>(this, message =>
            {
                if (message.Notification == MessengerNotifications.SELECTED)
                {
                    IsAthleteSelected = message.Content != null;
                    IsActivitiesEnabled = message.Content != null;
                }
            });

            SavingApp += _SavingApp;

            Messenger.Default.Register<NotificationMessage<ActivityEntity>>(this, message =>
            {
                if (message.Notification == MessengerNotifications.SELECTED)
                {
                    IsActivitySelected = message.Content != null;
                }
            });

            DbAccess.IsDirtyEvent += ((s, e) => { IsSaveFileEnabled = e.IsDirty; });

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

        private bool _IsActivitySelected = false;
        public bool IsActivitySelected
        {
            get => _IsActivitySelected;
            set
            {
                Set(() => IsActivitySelected, ref _IsActivitySelected, value);
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

        private bool _IsActivitiesEnabled = false;
        public bool IsActivitiesEnabled
        {
            get => _IsActivitiesEnabled;
            set
            {
                Set(() => IsActivitiesEnabled, ref _IsActivitiesEnabled, value && IsAthleteSelected);
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

        private string _ProgressbarText = Resources.MainWindow_WorkInProgress;
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

            IsNewAthleteEnabled = IsActivitiesEnabled = IsExitEnabled = IsOpenFileEnabled = IsNewFileEnabled = !args.IsSaving;
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
                        () => { Task.Run(() => _OpenFileDialog()); },
                        () => { return IsOpenFileEnabled; }
                        ));
            }
        }

        private void _OpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = Resources.MainWindow_SQLiteFilter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                _OpenFile(path);
            }

        }

        private void _OpenFile(string path)
        {
            ProgressbarText = Resources.MainWindow_LoadingFile;
            IsProgressbarVisible = true;
            _DbAccess.OpenDatabase(path);
            IsFileOpened = true;
            IsNewAthleteEnabled = true;
            IsActivitiesEnabled = false;
            IsProgressbarVisible = false;
            Settings.Default.LastOpenedFile = path;
            Messenger.Default.Send(new NotificationMessage<IList<AthleteEntity>>(_DbAccess.GetAthletes(), MessengerNotifications.LOADED));
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
                        () => { _NewFileDialog(); },
                        () => { return IsNewFileEnabled; }
                        ));
            }
        }

        private void _NewFileDialog()
        {
            var openFileDialog = new SaveFileDialog()
            {
                Filter = Resources.MainWindow_SQLiteFilter,
                AddExtension = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                _DbAccess.OpenDatabase(path, true);
                Messenger.Default.Send(new NotificationMessage<IList<AthleteEntity>>(null, MessengerNotifications.LOADED));
                IsFileOpened = true;
                IsSaveFileEnabled = true;
                IsActivitiesEnabled = false;
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
                        () => { _NewAthleteDialog(); },
                        () => { return IsNewAthleteEnabled; }
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
                        },
                        () => { return IsExitEnabled; }
                        )
                );
            }
        }

        private bool _IsClosing = false;
        public void _ExitDialog()
        {
            if (_IsClosing) return;
            if (IsSaveFileEnabled)
            {
                var response = MessageBox.Show(Resources.MainWindow_QuitConfirm, Resources.MainWindow_QuitConfirmHeader, MessageBoxButton.YesNoCancel);
                if (response != MessageBoxResult.Yes && response != MessageBoxResult.No)
                {
                    return;
                }
                if (response == MessageBoxResult.Yes)
                {
                    _Save();
                }
            }
            _IsClosing = true;
            NLog.LogManager.Shutdown();
            Settings.Default.Save();
            MessengerInstance.Send(new CloseDialogMessage(this));
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _ExitDialog();
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
                        () => { Task.Run(() => _Save()); },
                        () => { return IsSaveFileEnabled; }
                        ));
            }
        }

        private void _Save()
        {
            try
            {
                ProgressbarText = Resources.MainWindow_SavingFileProgressMessage;
                IsProgressbarVisible = true;
                SavingApp?.Invoke(this, new SavingNotificationEventArgs(IsSaving: true));
                _DbAccess.SaveData();
                MessageBox.Show(Resources.MainWindow_DataSaved, Resources.MainWindow_DataSavedHeader);
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

        #region DeleteActivitiesCommand
        private RelayCommand _DeleteActivitiesCommand;
        public RelayCommand DeleteActivitiesCommand
        {
            get
            {
                return _DeleteActivitiesCommand ??
                    (_DeleteActivitiesCommand = new RelayCommand(
                        () => { Task.Run(() => _DeleteActivities()); },
                        () => { return IsActivitySelected; }
                        ));
            }
        }

        private void _DeleteActivities()
        {
            ProgressbarText = Resources.MainWindow_DeletingActivitiesProgressMessage;
            IsProgressbarVisible = true;
            Messenger.Default.Send(new NotificationMessage<ACTION_TYPE>(ACTION_TYPE.DELETE_SELECTED_ACTIVITIES, ASK_FOR_ACTION));
            IsProgressbarVisible = false;
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
            Messenger.Default.Send(new NotificationMessage<ImporterTypeEnum>(ImporterTypeEnum.FITLOG, MessengerNotifications.IMPORT));
        }
        #endregion

        #region ImportGpxCommand
        private RelayCommand _ImportGpxCommand;
        public RelayCommand ImportGpxCommand
        {
            get
            {
                return _ImportGpxCommand ??
                    (_ImportGpxCommand = new RelayCommand(
                        () => { Task.Run(() => _ImportGpxDialogAsync()); }
                        ));
            }
        }

        private void _ImportGpxDialogAsync()
        {
            Messenger.Default.Send(new NotificationMessage<ImporterTypeEnum>(ImporterTypeEnum.GPX, MessengerNotifications.IMPORT));
        }
        #endregion
    }
}