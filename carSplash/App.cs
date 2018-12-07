using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace carSplash
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            MainPage = new NavigationPage(new AuthPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts

            AppCenter.Start("android=5e7bd1aa-6588-4968-bb45-ae1d79a352dc;" +
                  "uwp=e823a502-4ba6-44b3-85bc-e3ff42efc2a8;" +
                  "ios=b0ad7515-97c9-4acb-9ca0-4de2d9ce1718;",
                  typeof(Analytics), typeof(Crashes));

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

