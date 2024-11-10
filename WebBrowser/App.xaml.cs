using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Core;  // Ensure this is included
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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
            Window.Current.Closed += Current_Closed;
        }

        private async void Current_Closed(object sender, CoreWindowEventArgs e)
        {
            // Show confirmation dialog before exiting
            ContentDialogResult result = await ShowExitConfirmationDialogAsync();

            if (result == ContentDialogResult.Primary) // User clicked "Yes"
            {
                Application.Current.Exit(); // Exit the application
            }
            else
            {
                e.Handled = true; // Cancel the close event if "No" is clicked
            }
        }

        /*private async void CoreWindow_Closed(CoreWindow sender, CoreWindowEventArgs args)
        {
            // Show confirmation dialog before exiting
            ContentDialogResult result = await ShowExitConfirmationDialogAsync();

            if (result == ContentDialogResult.Primary) // User clicked "Yes"
            {
                Application.Current.Exit(); // Exit the application
            }
            else
            {
                args.Handled = true; // Cancel close event if "No" is clicked
            }
        }*/

        private async Task<ContentDialogResult> ShowExitConfirmationDialogAsync()
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
