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
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Win32;
using OSL.Common.Model;
using OSL.Common.Service;
using OSL.Common.Service.Importer;
using OSL.WPF.View;
using OSL.WPF.ViewModel.Scaffholding;
using OSL.WPF.WPFUtils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using static OSL.WPF.ViewModel.Scaffholding.MessengerNotifications;

namespace OSL.WPF.ViewModel
{
    public class AthleteDetailsVM : ViewModelBase
    {

        private readonly IDataAccessService _DbAccess;
        private readonly FitLogImporter _FitLogImporter;
        public AthleteDetailsVM(IDataAccessService DbAccess, FitLogImporter FitLogImporter)
        {
            _DbAccess = DbAccess;
            _FitLogImporter = FitLogImporter;
            Messenger.Default.Register<NotificationMessage<IList<AthleteEntity>>>(this, message =>
            {
                if (message.Notification == MessengerNotifications.LOADED)
                {
                    if (message.Content == null)
                    {
                        Athletes = new ObservableCollection<AthleteEntity>();
                    }
                    else
                    {
                        Athletes = new ObservableCollection<AthleteEntity>(message.Content);
                    }
                    SelectedAthlete = null;
                }
            });
            Messenger.Default.Register<NotificationMessage<IMPORT_TYPE>>(this, message =>
            {
                if (message.Notification == MessengerNotifications.IMPORT)
                {
                    if (message.Content == IMPORT_TYPE.FITLOG)
                    {
                        ImportFitLogDialog();
                    }
                }
            });
            Messenger.Default.Register<NotificationMessage<AthleteEntity>>(this, message =>
            {
                if (message.Notification == MessengerNotifications.NEW)
                {
                    var athlete = message.Content;
                    Athletes.Add(athlete);
                    SelectedAthlete = athlete;
                }
            });

        }

        #region Data
        ObservableCollection<AthleteEntity> _Athletes = new ObservableCollection<AthleteEntity>() { };

        public ObservableCollection<AthleteEntity> Athletes
        {
            get => _Athletes;
            private set { Set(() => Athletes, ref _Athletes, value); }
        }

        private AthleteEntity _SelectedAthlete;
        public AthleteEntity SelectedAthlete
        {
            get => _SelectedAthlete;
            set
            {
                Set(() => SelectedAthlete, ref _SelectedAthlete, value);
                Activities = _SelectedAthlete?.Activities;
                Messenger.Default.Send(new NotificationMessage<AthleteEntity>(_SelectedAthlete, MessengerNotifications.SELECTED));
            }
        }

        private ActivityEntity _SelectedActivity;
        public ActivityEntity SelectedActivity
        {
            get => _SelectedActivity;
            set
            {
                Set(() => SelectedActivity, ref _SelectedActivity, value);
                Messenger.Default.Send(new NotificationMessage<ActivityEntity>(_SelectedActivity, MessengerNotifications.SELECTED));
            }
        }

        ObservableCollection<ActivityEntity> _Activities = new ObservableCollection<ActivityEntity>() { };

        public ObservableCollection<ActivityEntity> Activities
        {
            get => _Activities;
            private set { Set(() => Activities, ref _Activities, value); }
        }
        #endregion

        #region Import Fitlog
        private void ImportFitLogDialog()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "FitLog SportTracks (*.fitlog)|*.fitlog"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ImportSportsMatchingDialog dialog = new ImportSportsMatchingDialog();
                    var dialogDataContext = (ImportSportsMatchingDialogVM)dialog.DataContext;
                    dialogDataContext.ImportSportsMatchingEntries.Clear();
                    using (FileStream fs = File.OpenRead(path))
                    {
                        foreach (var sport in _FitLogImporter.GetSports(fs))
                        {
                            dialogDataContext.ImportSportsMatchingEntries.Add(new ImportSportsMatchingEntryVM
                            {
                                ImportId = sport.Key,
                                ImportLabel = sport.Value
                            });
                        }
                    }
                    if (dialog.ShowDialog() ?? false)
                    {
                        var config = new Dictionary<string, ACTIVITY_SPORT>();
                        foreach (var listItem in dialog.lstMatchings.Items)
                        {
                            var entry = (ImportSportsMatchingEntryVM)listItem;
                            config.Add(entry.ImportId, entry.OslSport);
                        }
                        ViewsHelper.ExecuteWithSpinner(() =>
                        {
                            int cnt = 0;
                            using (FileStream fs = File.OpenRead(path))
                            {
                                foreach (var activity in _FitLogImporter.ImportActivitiesStream(fs, config))
                                {
                                    cnt++;
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        _SelectedAthlete.Activities.Add(activity);
                                    });
                                }
                            }
                            MessageBox.Show($"Imported {cnt} activit{(cnt > 1 ? "ies" : "y")}");
                        }, "Import is running...");
                    }
                });
            }

        }
        #endregion
    }
}
