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
using OSL.Common.Model;
using OSL.WPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OSL.WPF.View
{
    /// <summary>
    /// Interaction logic for ActivitiesList.xaml
    /// </summary>
    public partial class AthleteDetails : UserControl
    {
        private readonly AthleteDetailsVM _VM;
        public AthleteDetails()
        {
            InitializeComponent();
            _VM = DataContext as AthleteDetailsVM;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<ActivityEntity> toRemove = new List<ActivityEntity>();
            foreach (var activity in _VM.SelectedActivities)
            {
                if (!dtg_Activities.SelectedItems.Contains(activity)) toRemove.Add(activity);
            }
            toRemove.ForEach(a => _VM.SelectedActivities.Remove(a));

            for (var i = 0; i < dtg_Activities.SelectedItems.Count; i++)
            {
                if (!(dtg_Activities.SelectedItems[i] is ActivityEntity)) continue;
                var activity = dtg_Activities.SelectedItems[i] as ActivityEntity;
                if (!_VM.SelectedActivities.Contains(activity)) _VM.SelectedActivities.Add(activity);
            }
        }
    }
}
