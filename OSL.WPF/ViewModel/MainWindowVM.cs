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
using GeoSports.Common.Model;
using GeoSports.Common.Service.Importer;
using GeoSports.WPF.Service;
using GeoSports.WPF.ViewModel.Scaffholding;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace GeoSports.WPF.ViewModel
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
        private IDataAccessService _DbAccess;
        private FitLogImporter _FitLogImporter;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainWindowVM(IDataAccessService DbAccess, FitLogImporter FitLogImporter)
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
            _FitLogImporter = FitLogImporter;
            Messenger.Default.Register<CloseDialogMessage>(this, nm =>
            {
                CloseApp?.Invoke(this, new CloseNotificationEventArgs());
            });
        }

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

        #region ImportFirLogCommand
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
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "FitLog SportTracks (*.fitlog)|*.fitlog"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                using (FileStream fs = File.OpenRead(path))
                {
                    _FitLogImporter.ImportActivitiesStream(fs, new Dictionary<string, ActivityEntity.ACTIVITY_SPORT> {
                { "e41b80e4-fa5f-48e3-95be-d0e66b72ab7c", ActivityEntity.ACTIVITY_SPORT.BIKING},
                { "eca38408-cb82-42ed-b242-166b43b785a6",ActivityEntity.ACTIVITY_SPORT.RUNNING},
                { "6f2fdaf9-4c5a-4c2c-a4fa-5be42e9733dd",ActivityEntity.ACTIVITY_SPORT.SWIMMING} });
                }
            }

        }
        #endregion

    }
}