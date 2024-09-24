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
        //private List<String> removeFav = new List<String>();
        public String prevURL = "";
        public Uri uri = new System.Uri("https://google.com");
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void WebView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            prevURL = uri.AbsoluteUri;
        }

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
            Button favoriteButton = new Button
            {
                Content = uri.AbsoluteUri,
                Tag = uri
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


    }
}
