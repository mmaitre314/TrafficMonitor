using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TrafficReport.Extensions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//
// To generate assets (tiles, etc.) from SVG:
// Vector http://apps.microsoft.com/windows/en-us/app/vector/b9ccde1d-99c9-41c7-867f-f249c545af8c
//

namespace TrafficReport
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        Button ButtonRecord;
        Button ButtonCancel;
        TextBlock TextLog;
        ComboBox ComboBoxEndPoints;

        bool m_recording;

        public HubPage()
        {
            this.InitializeComponent();
        }

        // Use the Loaded event instead of OnNavigatedTo to be able to grab buttons from under HubSection
        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            // HubSection.ContentTemplate breaks control names, so we need to go get them ourselves
            ButtonRecord = (Button)HubSectionRecord.FindDescendantByName("ButtonRecord");
            ButtonCancel = (Button)HubSectionRecord.FindDescendantByName("ButtonCancel");
            TextLog = (TextBlock)HubSectionRecord.FindDescendantByName("TextLog");
            ComboBoxEndPoints = (ComboBox)HubSectionRecord.FindDescendantByName("ComboBoxEndPoints");

            m_recording = App.LocalSettings.Values.Get("Recording", false);

            // TODO: move that to some data source (Azure Table?)
            ComboBoxEndPoints.Items.Add(new ComboxBoxItemEndPoints("HomeToMicrosoft", "Home to Microsoft"));
            ComboBoxEndPoints.Items.Add(new ComboxBoxItemEndPoints("MicrosoftToHome", "Microsoft to Home"));

            // Select the default pair of end-points
            string id = App.LocalSettings.Values.Get("ComboxBoxItemEndPoints", "HomeToMicrosoft");
            for (int i = 0; i < ComboBoxEndPoints.Items.Count; i++)
            {
                if (((ComboxBoxItemEndPoints)ComboBoxEndPoints.Items[i]).Id == id)
                {
                    ComboBoxEndPoints.SelectedIndex = i;
                    break;
                }
            }

            // Queue the remaining part of the work because Authentication.AuthenticateAsync()
            // may not be called in the Loaded event handler itself
            var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    TextLog.Text += "Signing in\n";

                    // Note: on Phone this call does not always return and instead the app may get reactivated
                    if (!await Authentication.AuthenticateAsync())
                    {
                        TextLog.Text += "Failed to sign in\n";
                        return;
                    }

                    TextLog.Text += String.Format("Signed in as {0}\n", App.MobileServices.CurrentUser.UserId);

                    ButtonRecord.IsEnabled = true;
                    ButtonCancel.IsEnabled = true;

                    UpdateButtons();
                });
        }

        private void UpdateButtons()
        {
            ButtonRecord.Content = App.ResourceLoader.GetString(m_recording ? "End trip" : "Begin trip");
            ButtonCancel.Visibility = m_recording ? Visibility.Visible : Visibility.Collapsed;
            ComboBoxEndPoints.IsEnabled = !m_recording;
        }

        private async void ButtonRecord_Click(object sender, RoutedEventArgs e)
        {
            m_recording = !m_recording;
            App.LocalSettings.Values["Recording"] = m_recording;

            DateTime time = DateTime.Now;
            
            if (m_recording)
            {
                // Store the route start time
                // ApplicationDataContainer is supposed to support DateTime according to MSDN but throws
                App.LocalSettings.Values["RecordingStartTime"] = time.ToBinary();

                TextLog.Text += String.Format("Route start time: {0}\n", time);
            }
            else
            {
                // Upload the route to Azure Table

                ButtonRecord.IsEnabled = false;
                ButtonCancel.IsEnabled = false;

                var route = new ManualRoute
                {
                    EndPointsId = (string)App.LocalSettings.Values["ComboxBoxItemEndPoints"], 
                    StartTime = DateTime.FromBinary((long)App.LocalSettings.Values["RecordingStartTime"]),
                    EndTime = time
                };
                App.LocalSettings.Values.Remove("RecordingStartTime");

                TextLog.Text += String.Format("Route end time: {0}\n", time);
                TextLog.Text += "Uploading\n";

                try
                {
                    await App.MobileServices.GetTable<ManualRoute>().InsertAsync(route);
                    TextLog.Text += "Done\n";
                }
                catch (Exception ex)
                {
                    TextLog.Text += String.Format("Failed to upload route: {0}\n", ex.Message);
                }

                ButtonRecord.IsEnabled = true;
                ButtonCancel.IsEnabled = true;
            }

            UpdateButtons();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            m_recording = false;
            App.LocalSettings.Values.Remove("RecordingStartTime");
            App.LocalSettings.Values["Recording"] = m_recording;
            UpdateButtons();
        }

        private void ComboBoxEndPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.LocalSettings.Values["ComboxBoxItemEndPoints"] = 
                ((ComboxBoxItemEndPoints)ComboBoxEndPoints.Items[ComboBoxEndPoints.SelectedIndex]).Id;
        }

    }
}
