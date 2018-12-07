using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace carSplash
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PersonalPage : ContentPage
	{
        TodoItemManager manager;
        string username;
        double clientStar,serviceStar;
        int clientStarCount,serviceStarCount;
        public PersonalPage (string _username, double _clientStar, int _clientStarCount, double _serviceStar, int _serviceStarCount, TodoItemManager _manager)
		{
			InitializeComponent ();
            manager = _manager;
            username = _username;
            clientStar = _clientStar;
            serviceStar = _serviceStar;
            clientStarCount = _clientStarCount;
            serviceStarCount = _serviceStarCount;

            
            usernameLbl.Text = username;
            ClientRateLbl.Text = clientStar.ToString("0.#");
            ClientRateCountLbl.Text = clientStarCount.ToString();
            ServiceRateLbl.Text = serviceStar.ToString("0.#");
            ServiceRateCountLbl.Text = serviceStarCount.ToString();
            
        }



        protected override async void OnAppearing()
        {
            base.OnAppearing();


            clientStarCount = await manager.GetClientRateCount(username);
            serviceStarCount = await manager.GetServiceRateCount(username);
            clientStar = await manager.GetClientRate(username);
            serviceStar = await manager.GetServiceRate(username);
               

                
            

            usernameLbl.Text = username;
            ClientRateLbl.Text = clientStar.ToString("0.#");
            ClientRateCountLbl.Text = clientStarCount.ToString();
            ServiceRateLbl.Text = serviceStar.ToString("0.#");
            ServiceRateCountLbl.Text = serviceStarCount.ToString();
        }

        

    }
}