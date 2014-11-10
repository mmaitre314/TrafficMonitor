using TrafficReport.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.Storage;

namespace TrafficReport.Common
{
    public static class PropertySetExtensions
    {
        public static T Get<T>(this IPropertySet set, string key, T defaultValue)
        {
            return set.ContainsKey(key) ? (T)set[key] : defaultValue;
        }
    }
}

namespace TrafficReport
{
    public sealed partial class PivotPage : Page
    {
        bool m_recording;

        public PivotPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            m_recording = App.LocalSettings.Values.Get("Recording", false);
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            ButtonRecord.Content = App.ResourceLoader.GetString(m_recording ? "End trip" : "Begin trip");
            ButtonCancel.Visibility = m_recording ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ButtonRecord_Click(object sender, RoutedEventArgs e)
        {
            m_recording = !m_recording;
            App.LocalSettings.Values["Recording"] = m_recording;
            UpdateButtons();

            // TODO: select trip (hard-coded drop-down box, remember/restore selection)
            // TODO: store time, upload
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            m_recording = false;
            App.LocalSettings.Values["Recording"] = m_recording;
            UpdateButtons();
        }
    }
}
