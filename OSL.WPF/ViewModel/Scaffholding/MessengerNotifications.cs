/* Copyright 2021 Nicolas Mayeur

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
namespace OSL.WPF.ViewModel.Scaffholding
{
    public class MessengerNotifications
    {
        public readonly static string SELECTED = "Selected";
        public readonly static string LOADED = "Loaded";
        public readonly static string NEW = "New";
        public readonly static string IMPORT = "Import";

        public readonly static string ASK_FOR_ACTION = "AskForAction";
        public enum ACTION_TYPE { DELETE_SELECTED_ACTIVITIES }
    }
}
