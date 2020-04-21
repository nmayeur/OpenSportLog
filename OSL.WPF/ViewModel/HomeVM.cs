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
using GeoSports.Common.Model;
using GeoSports.EF;
using GeoSports.WPF.ViewModel.Scaffholding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace GeoSports.WPF.ViewModel
{
    public class HomeVM : ViewModelBase
    {
        public HomeVM()
        {
        }

        private AthleteEntity _Athlete;
        public AthleteEntity Athlete
        {
            get { return _Athlete; }
            set { _Athlete = value; }
        }

        #region OpenFile
        private RelayCommand _OpenFileCommand;
        public RelayCommand OpenFileCommand
        {
            get
            {
                return _OpenFileCommand ??
                    (_OpenFileCommand = new RelayCommand(
                        () => { OpenFileDialog(); }
                        ));
            }
        }

        private void OpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "SQLite (*.db,*.sqlite,*.sqlite3)|*.db;*.sqlite;*.sqlite3"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                var connectionString = string.Format("Data Source={0}", path);
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>()
                    {
                        { "ConnectionStrings:Default", connectionString }
                    }).Build();

                var dbContext = new GeoSportsContext(configuration);
                dbContext.Database.Migrate();
            }

        }
        #endregion

        #region NewFile
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
                var connectionString = string.Format("Data Source={0}", path);
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>()
                    {
                        { "ConnectionStrings:Default", connectionString }
                    }).Build();

                var dbContext = new GeoSportsContext(configuration);
                dbContext.Database.Migrate();
            }

        }
        #endregion

        #region Quit
        private RelayCommand _QuitCommand;
        public RelayCommand QuitCommand
        {
            get
            {
                return _QuitCommand ??
                    (_QuitCommand = new RelayCommand(
                        () => {
                            QuitDialog();
                        }
                        ));
            }
        }

        private void QuitDialog()
        {
            var response = MessageBox.Show("Do you want to quit application?", "Quit confirmation", MessageBoxButton.YesNo);
            if (response == MessageBoxResult.Yes)
            {
                MessengerInstance.Send(new CloseDialogMessage(this));
            }
        }
        #endregion
    }
}
