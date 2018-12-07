using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace carSplash
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MyWashPage : ContentPage
	{

        TodoItemManager manager;
        string username;
        double clientStar;
        int clientStarCount;

        public ObservableCollection<JobModel> Jobs { get; set; }
        public MyWashPage (string _username, double _clientStar, int _clientStarCount, TodoItemManager _manager)
		{
			InitializeComponent ();
            manager = _manager;
            username = _username;
            clientStar = _clientStar;
            clientStarCount = _clientStarCount;
        }
        

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Set syncItems to true in order to synchronize the data on startup when running in offline mode
            await RefreshItems(syncItems: true);
        }

        public async Task UpdateItem(JobModel item)
        {
            await manager.SaveTaskAsync(item);

        }

        async void Handle_CancelButton(object sender, System.EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var job = (JobModel)menuItem.CommandParameter;

            
            if (job.Staff != "")
            {
                var answer = await DisplayAlert("Cancel?", "Your wash has been assigned to a washman, cancelling will cause 1 bad rating", "Yes", "No");
                Debug.WriteLine("Answer: " + answer);
                if (answer)
                {
                    await manager.RateClient(job.Client, 0);

                    job.Staff = "";
                    job.Done = true;
                    await UpdateItem(job);
                    await RefreshItems(syncItems: true);
                }
            }
            else
            {
                //Cancel
                job.Done = true;
                await UpdateItem(job);
                await RefreshItems(syncItems: true);
            }
        }

        async void RefrBtn_clicked()
        {
            await RefreshItems(syncItems: true);
        }
        private async Task RefreshItems(bool syncItems)
        {
            Jobs = await manager.GetJobModelAsync();
            var jobsDisplay = new ObservableCollection<JobModel>();
            foreach (JobModel item in Jobs)
            {
                if ((((item.Client == username) && (item.Done)) && !item.ServiceRateGiven) && (item.Staff != "")) //give feedback for staff
                {
                    var action = await DisplayActionSheet("Rate Service " + item.VehicleInfo, "Later", null, "1 Star", "2 Stars", "3 Stars", "4 Stars", "5 Stars");
                    Debug.WriteLine("Action: " + action);
                    switch (action)
                    {
                        case "1 Star":
                            await manager.RateStaff(item.Staff, 1);
                            break;
                        case "2 Stars":
                            await manager.RateStaff(item.Staff, 2);
                            break;
                        case "3 Stars":
                            await manager.RateStaff(item.Staff, 3);
                            break;
                        case "4 Stars":
                            await manager.RateStaff(item.Staff, 4);
                            break;
                        case "5 Stars":
                            await manager.RateStaff(item.Staff, 5);
                            break;
                        default:
                            break;
                    }
                    item.ServiceRateGiven = true;
                    await manager.SaveTaskAsync(item);
                }
                if ((item.Client == username) && (!item.Done))
                {
                    jobsDisplay.Add(item);
                }
            }
            MarketView.ItemsSource = jobsDisplay;
        }

        async void PostBtn_clicked(object sender, EventArgs e)
        {
            PostBtn.IsEnabled = false;

            await Navigation.PushAsync(new NewPost(username, clientStar, clientStarCount, manager));
            PostBtn.IsEnabled = true;
            return;
        }
    }
}