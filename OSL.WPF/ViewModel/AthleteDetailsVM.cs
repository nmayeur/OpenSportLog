﻿/* Copyright 2021 Nicolas Mayeur

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
using OSL.WPF.Properties;
using OSL.WPF.View;
using OSL.WPF.ViewModel.Scaffholding;
using OSL.WPF.WPFUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using static OSL.WPF.ViewModel.Scaffholding.MessengerNotifications;

namespace OSL.WPF.ViewModel
{
    public class AthleteDetailsVM : OSLViewModel
    {

        private readonly IDataAccessService _DbAccess;
        private readonly FitLogImporter _FitLogImporter;
        private readonly GpxImporter _GpxImporter;
        public AthleteDetailsVM(IDataAccessService DbAccess, FitLogImporter FitLogImporter, GpxImporter GpxImporter)
        {
            _Logger = NLog.LogManager.GetCurrentClassLogger();
            _DbAccess = DbAccess;
            _FitLogImporter = FitLogImporter;
            _GpxImporter = GpxImporter;
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
                }
            });
            Messenger.Default.Register<NotificationMessage<ImporterTypeEnum>>(this, message =>
            {
                if (message.Notification == MessengerNotifications.IMPORT)
                {
                    ImportDialog(message.Content);
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
            Messenger.Default.Register<NotificationMessage<ACTION_TYPE>>(this, message =>
            {
                if (message.Notification == MessengerNotifications.ASK_FOR_ACTION && message.Content == ACTION_TYPE.DELETE_SELECTED_ACTIVITIES)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        var toRemove = new List<ActivityEntity>(SelectedActivities);
                        foreach (var activity in toRemove) _SelectedAthlete.Activities.Remove(activity);
                    });
                }
            });
        }

        #region Data
        ObservableCollection<AthleteEntity> _Athletes = new ObservableCollection<AthleteEntity>() { };

        public ObservableCollection<AthleteEntity> Athletes
        {
            get => _Athletes;
            private set
            {
                Set(() => Athletes, ref _Athletes, value);
                if (Athletes.Count == 1)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        SelectedAthlete = Athletes[0];
                    });
                }
                else if (Settings.Default.LastOpenedAthleteId >= 0)
                {
                    var defaultAthlete = _Athletes.FirstOrDefault(a => a.Id == Settings.Default.LastOpenedAthleteId);
                    if (defaultAthlete != null) SelectedAthlete = defaultAthlete;
                }
                else
                {
                    SelectedAthlete = null;
                }
            }
        }

        private AthleteEntity _SelectedAthlete;
        public AthleteEntity SelectedAthlete
        {
            get => _SelectedAthlete;
            set
            {
                Set(() => SelectedAthlete, ref _SelectedAthlete, value);
                Settings.Default.LastOpenedAthleteId = _SelectedAthlete?.Id ?? -1;
                Messenger.Default.Send(new NotificationMessage<AthleteEntity>(_SelectedAthlete, MessengerNotifications.SELECTED));
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Activities.Clear();
                    if (_SelectedAthlete != null)
                    {
                        foreach (var activity in _DbAccess.GetActivitiesForAthlete(_SelectedAthlete))
                        {
                            Activities.Add(activity);
                        }
                    }
                    //Messenger.Default.Send(new NotificationMessage<IList<ActivityEntity>>(Activities, MessengerNotifications.LOADED));
                });
                if (_SelectedAthlete != null)
                {
                    _SelectedAthlete.Activities.CollectionChanged += (sender, e) =>
                    {
                        switch (e.Action)
                        {
                            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                                foreach (ActivityEntity a in e.NewItems) Activities.Add(a);
                                break;
                            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                                foreach (ActivityEntity a in e.OldItems)
                                {
                                    SelectedActivity = null;
                                    Activities.Remove(a);
                                }
                                break;
                            case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                                foreach (ActivityEntity a in e.OldItems) Activities.Remove(a);
                                foreach (ActivityEntity a in e.NewItems) Activities.Add(a);
                                break;
                            case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                                Activities.Clear();
                                break;
                        }
                    };
                    Messenger.Default.Send(new NotificationMessage<IList<ActivityEntity>>(Activities, MessengerNotifications.LOADED));
                }
            }
        }

        /// <summary>
        /// Last selected activity in the datagrid (which is multi-selectable)
        /// </summary>
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

        /// <summary>
        /// All selected activities in the datagrid
        /// </summary>
        ObservableCollection<ActivityEntity> _SelectedActivities = new ObservableCollection<ActivityEntity>() { };

        public ObservableCollection<ActivityEntity> SelectedActivities
        {
            get => _SelectedActivities;
            set { Set(() => SelectedActivities, ref _SelectedActivities, value); }
        }

        ObservableCollection<ActivityEntity> _Activities = new ObservableCollection<ActivityEntity>() { };

        public ObservableCollection<ActivityEntity> Activities
        {
            get => _Activities;
            private set
            {
                Set(() => Activities, ref _Activities, value);
            }
        }
        #endregion

        #region Import Fitlog
        private void ImportDialog(ImporterTypeEnum ImportType)
        {
            IActivitiesImporter importer;
            switch (ImportType)
            {
                case ImporterTypeEnum.FITLOG:
                    importer = _FitLogImporter;
                    break;
                case ImporterTypeEnum.GPX:
                    importer = _GpxImporter;
                    break;
                default:
                    _Logger.Error("Invalid type for importer");
                    return;
            }
            var openFileDialog = new OpenFileDialog()
            {
                Filter = ImportType == ImporterTypeEnum.FITLOG ? Resources.AthleteDetails_FitlogFilter : Resources.AthleteDetails_GpxFilter
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
                        foreach (var sport in importer.GetSports(fs))
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
                            int duplicates = 0;
                            using (FileStream fs = File.OpenRead(path))
                            {
                                foreach (var activity in importer.ImportActivitiesStream(fs, config))
                                {
                                    cnt++;
                                    if (_SelectedAthlete.Activities.Where(act => act.OriginId == activity.OriginId).Count() > 0)
                                    {
                                        duplicates++;
                                    }
                                    else
                                    {
                                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                        {
                                            _SelectedAthlete.Activities.Add(activity);
                                        });
                                    }
                                }
                            }
                            cnt -= duplicates;
                            string message;
                            if (cnt > 1)
                            {
                                message = string.Format(Resources.AthleteDetails_ImportDonePlural, cnt);
                            }
                            else
                            {
                                message = string.Format(Resources.AthleteDetails_ImportDoneSingular, cnt);
                            }
                            if (duplicates > 0)
                            {
                                message += "\n" + string.Format(Resources.AthleteDetails_ImportDuplicatesFound, duplicates);
                            }
                            MessageBox.Show(message);
                        }, Resources.AthleteDetails_ImportIsRunning);
                    }
                });
            }

        }
        #endregion
    }
}
