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

/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:GeoSports.WPF"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using OSL.Common.Service;
using OSL.Common.Service.Importer;
using OSL.EF.Service;
using OSL.WPF.Service;

namespace OSL.WPF.ViewModel.Scaffholding
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainWindowVM>();
            SimpleIoc.Default.Register<AthleteDetailsVM>();
            SimpleIoc.Default.Register<ActivityDetailsVM>();
            SimpleIoc.Default.Register<ImportSportsMatchingDialogVM>();
            SimpleIoc.Default.Register<NewAthleteVM>();
            SimpleIoc.Default.Register<IDataAccessService, DataAccessService>();
            SimpleIoc.Default.Register<FitLogImporter>();
            SimpleIoc.Default.Register<ILoggerService, LoggerService>();
        }

        public MainWindowVM MainWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainWindowVM>();
            }
        }

        public AthleteDetailsVM AthleteDetails
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AthleteDetailsVM>();
            }
        }

        public ActivityDetailsVM ActivityDetails
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ActivityDetailsVM>();
            }
        }

        public ImportSportsMatchingDialogVM ImportSportsMatchingDialog
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ImportSportsMatchingDialogVM>();
            }
        }

        public NewAthleteVM NewAthleteDialog
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NewAthleteVM>();
            }
        }

        public static void Cleanup()
        {
            // Clear the ViewModels
        }
    }
}