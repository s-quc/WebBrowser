using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Windows.Storage;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WebBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    
    public sealed partial class MainPage : Page
    {
        private List<Uri> favorites = new List<Uri>();
        public List<Uri> history = new List<Uri>();
        //private List<String> removeFav = new List<String>();
        public String prevURL = "";
        public Uri uri = new System.Uri("https://google.com");
        public String prevText = "";
        public String space = "                                                                        ";
        public double currentZoomFactor = 1.0;

        DispatcherTimer dt = new DispatcherTimer();
        public MainPage()
        {
            this.InitializeComponent();

            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += UpdateCurrentTime;
            dt.Start();

            InitializeWebView();
            this.KeyDown += MainPage_KeyDown;
        }

        private async void InitializeWebView()
        {
            await wvMain.EnsureCoreWebView2Async();
            wvMain.CoreWebView2.NavigationCompleted += WvMain_NavigationCompleted;
        }
        

        private void ShowErrorMessage(string message)
        {
            // Display the error message in a UI element, e.g., a TextBlock
            tbErrorMessage.Text = message;
            tbErrorMessage.Visibility = Visibility.Visible;
        }

        private void SaveFavorites(List<string> favorites)
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            // Convert the list to a comma-separated string
            string favoritesString = string.Join(",", favorites);
            localSettings.Values["Favorites"] = favoritesString;
        }



        public void UpdateFavoritesUI(List<string> favorites)
        {
            // Clear existing favorites UI
            FavoritesStack.Children.Clear();

            // Create buttons for each favorite and add them to the StackPanel
            foreach (var favorite in favorites)
            {
                Button favoriteButton = new Button
                {
                    Content = favorite,
                    Tag = favorite
                };

                // When a favorite button is clicked, load the URL in the WebView2 control
                favoriteButton.Click += (s, e) =>
                {
                    wvMain.Source = new Uri(favorite);
                };

                FavoritesStack.Children.Add(favoriteButton);
            }
        }
        private void SaveFavorites(List<string> favorites)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            string favoritesString = string.Join(",", favorites);
            localSettings.Values["Favorites"] = favoritesString;
        }

        private async void btnChangeZoom_Click(object sender, RoutedEventArgs e)
        {
            // Show a dialog to enter the custom zoom percentage
            ContentDialog dialog = new ContentDialog
            {
                Title = "Enter Zoom Percentage",
                PrimaryButtonText = "Set Zoom",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary
            };

            // Create a TextBox for input
            TextBox zoomInputBox = new TextBox
            {
                Width = 200,
                PlaceholderText = "Enter Zoom % (e.g., 100, 120)",
                Text = currentZoomFactor.ToString("F0"), // Show current zoom percentage
                Margin = new Windows.UI.Xaml.Thickness(20)
            };

            // Add the TextBox to the ContentDialog
            dialog.Content = zoomInputBox;

            // Show the dialog and wait for the user to submit or cancel
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // If the user pressed "Set Zoom", update the zoom factor
                if (int.TryParse(zoomInputBox.Text, out int zoomPercentage) && zoomPercentage >= 10 && zoomPercentage <= 500)
                {
                    SetZoom(zoomPercentage);
                }
                else
                {
                    // If the input is invalid, show an error message
                    ShowErrorMessage("Invalid zoom percentage. Please enter a value between 10% and 500%.");
                }
            }
        }

        private async void SetZoom(int zoomPercentage)
        {
            currentZoomFactor = zoomPercentage / 100.0; // Convert percentage to decimal (e.g., 100% -> 1.0, 120% -> 1.2)
            await wvMain.CoreWebView2.ExecuteScriptAsync($"document.body.style.zoom='{currentZoomFactor}'");

            // Optionally, display the current zoom percentage in a TextBlock
            tbZoomPercentage.Text = $"Current Zoom: {zoomPercentage}%";
        }



        private void MainPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Tab) 
            {
                if (SearchBar.IsEnabled)
                {
                    SearchBar.Focus(FocusState.Keyboard);
                }
                //SearchBar.Text = " ";
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    SearchBar.SelectAll();
                });

                e.Handled = true;
            }
            if (e.Key == Windows.System.VirtualKey.Add) // '+' key
            {
                ZoomIn();
            }
            else if (e.Key == Windows.System.VirtualKey.Subtract) // '-' key
            {
                ZoomOut();
            }
        }

        private async void ZoomIn()
        {
            currentZoomFactor += 0.1; // Increase zoom by 10%
            await wvMain.CoreWebView2.ExecuteScriptAsync($"document.body.style.zoom='{currentZoomFactor}'");
        }

        private async void ZoomOut()
        {
            if (currentZoomFactor > 0.1) // Prevent zooming out too much
            {
                currentZoomFactor -= 0.1; // Decrease zoom by 10%
                await wvMain.CoreWebView2.ExecuteScriptAsync($"document.body.style.zoom='{currentZoomFactor}'");
            }
        }

        private void WvMain_NavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                tbErrorMessage.Visibility = Visibility.Collapsed;
                Uri currentUri = new Uri(sender.Source);
                if (!history.Contains(currentUri))
                {
                    history.Insert(0, currentUri);
                    CreateHistoryButton(currentUri);
                }
            }
        }
        private void CreateHistoryButton(Uri uri)
        {
            string content = uri.AbsoluteUri;
            if (content.Contains("https://www.google.com/search?q="))
            {
                content = "google.com/" + prevText;
                if (content.Length > 30)
                {
                    content = content.Substring(0, 30);
                }
                else 
                {
                    int needed = 30 - content.Length;
                    content = content + space.Substring(0, needed);
                }
            }
            else 
            {
                if (content.IndexOf(".com") != -1)
                {
                    content = content.Substring(0, content.IndexOf(".com") + 4);
                    content = content.Substring(12);
                }
                else if (content.IndexOf(".org") != -1)
                {
                    content = content.Substring(0, content.IndexOf(".org") + 4);
                    content = content.Substring(12);
                }
                else if (content.IndexOf(".gov") != -1)
                {
                    content = content.Substring(0, content.IndexOf(".gov") + 4);
                    content = content.Substring(12);
                }
                else if (content.IndexOf(".net") != -1)
                {
                    content = content.Substring(0, content.IndexOf(".net") + 4);
                    content = content.Substring(12);
                }
                else if (content.IndexOf(".edu") != -1)
                {
                    content = content.Substring(0, content.IndexOf(".edu") + 4);
                    content = content.Substring(12);
                }
            }
           
            Button historyPage = new Button
            {
                Content = content,
                Tag = uri
            };

            historyPage.Click += (s, e) =>
            {
                wvMain.Source = uri;
            };

            spHistory.Children.Insert(0, historyPage);
        }

       

        private void UpdateCurrentTime(object sender, object e)
        {
            tbTime.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        /*private void WebView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            //prevURL = uri.AbsoluteUri;
        }*/

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //wvMain.Navigate(uri);
            wvMain.Source = (new Uri("https://google.com"));
        }

        private void bBack_Click(object sender, RoutedEventArgs e)
        {
            //wvMain.Navigate(new System.Uri(prevURL));
            if (wvMain.CanGoBack)
            {
                wvMain.GoBack();
            }

            /*String url = wvMain.Source.ToString();
            string url = wvMain.Source.AbsoluteUri;
            wvMain.Source = new Uri(url);
            wvMain.Navigate(new System.Uri(url));*/
        }

        private void bForward_Click(object sender, RoutedEventArgs e)
        {
            if (wvMain.CanGoForward) 
            {
                wvMain.GoForward();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (wvMain.Source is Uri currentUri)
            {
                if (!favorites.Contains(currentUri))
                {
                    favorites.Add(currentUri);
                    //removeFav.Add("Remove");
                    CreateFavoriteButton(currentUri);
                }
            }
        }


        private void CreateFavoriteButton(Uri uri)
        {
            //String display = uri.AbsoluteUri;
            //display.Substring(display.IndexOf(:/

            string content = uri.AbsoluteUri;

            if (content.Contains("https://www.google.com/search?q="))
            {
                content = "google.com/" + prevText;
                if (content.Length > 30)
                {
                    content = content.Substring(0, 30);
                }
                else
                {
                    int needed = 30 - content.Length;
                    content = content + space.Substring(0, needed);
                }
            }
            else
            {
                if (content.IndexOf(".com") != -1)
                {
                    content = content.Substring(0, content.IndexOf(".com") + 4);
                    content = content.Substring(12);
                }
                else if (content.IndexOf(".org") != -1)
                {
                    content = content.Substring(0, content.IndexOf(".org") + 4);
                    content = content.Substring(12);
                }
                else if (content.IndexOf(".gov") != -1)
                {
                    content = content.Substring(0, content.IndexOf(".gov") + 4);
                    content = content.Substring(12);
                }
                else if (content.IndexOf(".net") != -1)
                {
                    content = content.Substring(0, content.IndexOf(".net") + 4);
                    content = content.Substring(12);
                }
                else if (content.IndexOf(".edu") != -1)
                {
                    content = content.Substring(0, content.IndexOf(".edu") + 4);
                    content = content.Substring(12);
                }
            }



            Button favoriteButton = new Button
            {
                Content = content,
                Tag = uri,
            };

            
            favoriteButton.Click += FavoriteButton_Click;

            Button removeButton = new Button
            {
                Content = "Remove",
                Tag = uri
            };

            removeButton.Click += (s, e) =>
            {
                FavoritesStack.Children.Remove(favoriteButton);
                RemoveStack.Children.Remove(removeButton);
                favorites.Remove(uri);
                //removeFav.Remove("Remove");
            };      

            FavoritesStack.Children.Add(favoriteButton);
            RemoveStack.Children.Add(removeButton);
            
            

        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Uri uri)
            {
                wvMain.Source = uri;
            }
        }

        private void bRefreshHist_Click(object sender, RoutedEventArgs e)
        {
            spHistory.Children.Clear();
            history.Clear();
        }

       

        private void SearchBar_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            String text = SearchBar.Text.Trim();
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (text.Contains("."))
                {
                    try
                    {
                        wvMain.Source = new Uri("https://www." + text);
                    }
                    catch (Exception ex)
                    {
                        string googleSearchUrl = "https://www.google.com/search?q=" + Uri.EscapeDataString(text);
                        wvMain.Source = new Uri(googleSearchUrl);
                    }
                }
                else
                {
                    string googleSearchUrl = "https://www.google.com/search?q=" + Uri.EscapeDataString(text);
                    wvMain.Source = new Uri(googleSearchUrl);
                }
                prevText = SearchBar.Text;
                SearchBar.Text = "";
            }




        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e) 
        { 
        
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //refresh button
            if (!prevText.StartsWith("https://"))
            {
                string googleSearchUrl = "https://www.google.com/search?q=" + Uri.EscapeDataString(prevText);
                wvMain.Source = new Uri(googleSearchUrl);
            }
            else
            {
                try
                {
                    wvMain.Source = new Uri(prevText);
                }
                catch (Exception ex)
                {
                    prevText = prevText.Substring(8);
                    string googleSearchUrl = "https://www.google.com/search?q=" + Uri.EscapeDataString(prevText);
                    wvMain.Source = new Uri(googleSearchUrl);
                }
            }
        }

        private void RevertText_Click(object sender, RoutedEventArgs e)
        {
            SearchBar.Text = prevText;
        }
    }

    public class Favorite
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }


}
