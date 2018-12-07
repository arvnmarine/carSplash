using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace carSplash
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MarketplacePage : ContentPage
	{
        public string username;
        public int clientStarCount, serviceStarCount;
        public double clientStar, serviceStar;
        TodoItemManager manager;

        public ObservableCollection<JobModel> Jobs { get; set; }
        public MarketplacePage (string _username, double _clientStar, int _clientStarCount, double _serviceStar, int _serviceStarCount, TodoItemManager _manager)
		{
			InitializeComponent ();
            username = _username;
            clientStar = _clientStar;
            clientStarCount = _clientStarCount;
            serviceStar = _serviceStar;
            serviceStarCount = _serviceStarCount;
            manager = _manager;
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

        

        async void Handle_AcceptButton(object sender, System.EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var job = (JobModel)menuItem.CommandParameter;

            job.Staff = username;
            await UpdateItem(job);
            await RefreshItems(syncItems: true);
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
                if (((item.Client != username) && (item.Staff == "")) && (!item.Done))
                {
                    jobsDisplay.Add(item);
                }
            }
            MarketView.ItemsSource = jobsDisplay;
        }
    }
}