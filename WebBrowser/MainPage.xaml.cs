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
            wvMain.Navigate(uri);
            //wvMain.Navigate(new Uri("https://google.com"));
        }

        private void bBack_Click(object sender, RoutedEventArgs e)
        {
            //wvMain.Navigate(new System.Uri(prevURL));
            wvMain.GoBack();
        }

        private void bForward_Click(object sender, RoutedEventArgs e)
        {
            wvMain.GoForward();
        }
    }
}
