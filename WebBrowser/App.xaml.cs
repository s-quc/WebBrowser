using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

namespace WebBrowser
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Load previous state if applicable
                }

                Window.Current.Content = rootFrame;
            }

            if (!e.PrelaunchActivated)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                Window.Current.Activate();
            }

            // Subscribe to window close event to show confirmation dialog
            Window.Current.Closed += OnWindowClosed;
        }

        private async void OnWindowClosed(object sender, CoreWindowEventArgs e)
        {
            Debug.WriteLine("Window close event triggered.");

            try
            {
                // Show the exit confirmation dialog
                ContentDialogResult result = await ShowExitConfirmationDialogAsync();

                Debug.WriteLine("Dialog result: " + result.ToString());

                // If "Yes" is clicked, exit the application; otherwise, cancel the close
                if (result == ContentDialogResult.Primary)
                {
                    Debug.WriteLine("Exiting the app...");
                    Application.Current.Exit(); // Exit the app
                }
                else
                {
                    Debug.WriteLine("Canceling close action.");
                    e.Handled = true; // Prevent the app from closing (cancel the close)
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that may be causing the issue
                Debug.WriteLine("Error during exit confirmation: " + ex.Message);
                e.Handled = false; // Make sure the close event happens if there's an error
            }
        }

        private async Task<ContentDialogResult> ShowExitConfirmationDialogAsync()
        {
            try
            {
                // Create and configure the confirmation dialog
                ContentDialog exitDialog = new ContentDialog
                {
                    Title = "Are you sure?",
                    Content = "Do you really want to exit the app?",
                    PrimaryButtonText = "Yes",
                    SecondaryButtonText = "No",
                    DefaultButton = ContentDialogButton.Secondary // Default to "No"
                };

                return await exitDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                // Log the exception if dialog creation fails
                Debug.WriteLine("Error creating exit dialog: " + ex.Message);
                throw;  // Rethrow the exception after logging
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
