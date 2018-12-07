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
	public partial class MyJobPage : ContentPage
	{
        TodoItemManager manager;
        string username;

        public ObservableCollection<JobModel> Jobs { get; set; }

        public MyJobPage(string _username, TodoItemManager _manager)
        {
			InitializeComponent ();
            manager = _manager;
            username = _username;
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

            var answer = await DisplayAlert("Cancel?", "Cancelling will cause 1 bad rating", "Yes", "No");
            Debug.WriteLine("Answer: " + answer);
            if (!answer)
            {
                return;
            }
                //Cancel

            await manager.RateStaff(username, 0);
            job.Staff = "";
            await UpdateItem(job);
            await RefreshItems(syncItems: true);
        }

        async void RefrBtn_clicked()
        {
            await RefreshItems(syncItems: true);
        }

        async void Handle_FinishButton(object sender, System.EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var job = (JobModel)menuItem.CommandParameter;


            // request feedback for client
            var action = await DisplayActionSheet("Rate your Client", "Later", null, "1 Star", "2 Stars", "3 Stars", "4 Stars", "5 Stars");
            Debug.WriteLine("Action: " + action);
            switch (action)
            {
                case "1 Star":
                    await manager.RateClient(job.Client,1);
                    break;
                case "2 Stars":
                    await manager.RateClient(job.Client, 2);
                    break;
                case "3 Stars":
                    await manager.RateClient(job.Client, 3);
                    break;
                case "4 Stars":
                    await manager.RateClient(job.Client, 4);
                    break;
                case "5 Stars":
                    await manager.RateClient(job.Client, 5);
                    break;
                default: 
                    break;
            }

            //Cancel
            job.Done = true;
            job.ClientRateGiven = true;
            await UpdateItem(job);
            await RefreshItems(syncItems: true);
        }

        private async Task RefreshItems(bool syncItems)
        {
            Jobs = await manager.GetJobModelAsync();
            var jobsDisplay = new ObservableCollection<JobModel>();
            foreach (JobModel item in Jobs)
            {
                if ((item.Staff == username) && (!item.Done))
                {
                    jobsDisplay.Add(item);
                }
            }
            MarketView.ItemsSource = jobsDisplay;
        }
    }
}