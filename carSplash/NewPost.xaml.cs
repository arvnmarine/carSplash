using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Net.Http;
using System.Diagnostics;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace carSplash
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewPost : ContentPage
	{
        public string username;
        public double clientStar;
        public int clientStarCount;
        TodoItemManager manager;

        public NewPost (string _username, double _clientStar, int _clientStarCount, TodoItemManager _manager)
		{

			InitializeComponent ();
            manager = _manager;


            NavigationPage.SetHasNavigationBar(this, false);

            myDatePicker.MinimumDate = DateTime.Now;

            MakeEntry.Text = "";
            ModelEntry.Text = "";
            ColorEntry.Text = "";
            AddrEntry.Text = "";


            username = _username;
            clientStar = _clientStar;
            clientStarCount = _clientStarCount;

        }

        public async Task AddItem(JobModel item)
        {
            await manager.SaveTaskAsync(item);

        }

        async Task<string> ValidateAddr(string addr)
        {

            AddressWebservice AddressData = new AddressWebservice();
            var client = new HttpClient();
            string apikey = "&key=AIzaSyBJMYY8xFfaRSfuTjdaPyq-NjjQnVuUipE";
            var ApiAddress = "https://maps.googleapis.com/maps/api/geocode/json?address=" +addr + apikey;
            var uri = new Uri(ApiAddress);
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                
                AddressData = JsonConvert.DeserializeObject<AddressWebservice>(jsonContent);
            }
            if (AddressData.Status == "OK")
            {

                return ((AddressData.Results)[0].Formatted_address);
            }
            
            return ("Invalid");
        }


        async void CancelBtn_clicked()
        {
            await Navigation.PopAsync();
        }

        async void SubmitBtn_clicked()
        {
            SubmitBtn.IsEnabled = false;
            
            //validation
            
            if (MakeEntry.Text.ToString().Length * ModelEntry.Text.ToString().Length * ColorEntry.Text.ToString().Length * AddrEntry.Text.ToString().Length * (1+picker.SelectedIndex) == 0)
            {

                await DisplayAlert("Alert", "Please complete all info", "OK");
                SubmitBtn.IsEnabled = true;
                return;
            } else
            {
                var validAddr = await ValidateAddr(AddrEntry.Text);
                if (validAddr == "Invalid")
                {
                    await DisplayAlert("Invalid address", "Please enter a valid address", "OK");
                    SubmitBtn.IsEnabled = true;
                    return;
                }

                var answer = await DisplayAlert("Is this your correct address", validAddr, "Yes", "No");
                Debug.WriteLine("Answer: " + answer);
                if (!answer)
                {
                    SubmitBtn.IsEnabled = true;
                    return;
                }
                AddrEntry.Text = validAddr;

                string _pack = "Luxury $50";
                if (picker.SelectedIndex == 0)
                {
                    _pack = "Basic $15";
                } else if (picker.SelectedIndex == 1)
                {
                    _pack = "First Class $30";
                } 
                var newJob = new JobModel
                {
                    Client = username,
                    Staff = "",
                    VehicleInfo = ColorEntry.Text + " " + MakeEntry.Text + " " + ModelEntry.Text ,
                    Addr = AddrEntry.Text,
                    Date = myDatePicker.Date.ToString().Substring(0,10),
                    Package = _pack,
                    RateClient = clientStar,
                    RateClientCount = clientStarCount,
                    IsDeleted = false
                };
                using (var scope = new ActivityIndicatorScope(syncIndicator, true))
                {
                    await AddItem(newJob);
       
                }

                
                Analytics.TrackEvent("Car model", new Dictionary<string, string> {
                    { "Car", MakeEntry.Text + " " + ModelEntry.Text },
                    { "Color", ColorEntry.Text},
                    { "Wash Package", _pack }
                });
                

                await DisplayAlert("Alert", "Your Wash has been posted", "OK");
                SubmitBtn.IsEnabled = true;
                await Navigation.PopAsync();
            }
            

            SubmitBtn.IsEnabled = true;
            return;
        }

        private class ActivityIndicatorScope : IDisposable
        {
            private bool showIndicator;
            private ActivityIndicator indicator;
            private Task indicatorDelay;

            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                this.indicator = indicator;
                this.showIndicator = showIndicator;

                if (showIndicator)
                {
                    indicatorDelay = Task.Delay(2000);
                    SetIndicatorActivity(true);
                }
                else
                {
                    indicatorDelay = Task.FromResult(0);
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                this.indicator.IsVisible = isActive;
                this.indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (showIndicator)
                {
                    indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

    }

    
}