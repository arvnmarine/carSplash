using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Connectivity;

namespace carSplash
{
	
	public partial class AuthPage : ContentPage
	{
        TodoItemManager manager;
        public AuthPage ()
		{
            InitializeComponent();
            manager = TodoItemManager.DefaultManager;


            NavigationPage.SetHasNavigationBar(this, false);

            usernameEntry.Text = "";
            pwEntry.Text = "";
            rpwEntry.Text = "";
        }


        public async Task AddItem(AccountModel item)
        {
            await manager.SaveTaskAsync(item);
            
        }

        async void LogInBtn_Clicked(object sender, EventArgs e)
        {
            LogInBtn.IsEnabled = false;
            RegisterBtn.IsEnabled = false;
            if (!CrossConnectivity.Current.IsConnected) {
                await DisplayAlert("Alert", "No Internet, retry later!", "OK");
                LogInBtn.IsEnabled = true;
                return;
            }

            if ((usernameEntry.Text.ToString().Length < 6) || (pwEntry.Text.ToString().Length < 6))
            {
                await DisplayAlert("Alert", "Username and Password require 6 characters minimum", "OK");
            } else
            {
                bool isUsernameExist;
                using (var scope = new ActivityIndicatorScope(syncIndicator, true))
                {
                    
                    var _isUsernameExist = await manager.IsUsernameExist(usernameEntry.Text);
                    
                    isUsernameExist = _isUsernameExist;
                }
                
                if (!isUsernameExist)
                {
                    await DisplayAlert("Alert", "Username does not exist", "OK");
                } else
                {
                    bool isMatchPw;
                    using (var scope = new ActivityIndicatorScope(syncIndicator, true))
                    {
                        
                        var _isMatchPw = await manager.IsMatchPw(usernameEntry.Text, pwEntry.Text);
                        
                        isMatchPw = _isMatchPw;
                    }
                    if (isMatchPw)
                    {

                        //await DisplayAlert("Alert", "Welcome back!", "OK");
                        double clientStar, serviceStar;
                        int clientStarCount, serviceStarCount;
                        using (var scope = new ActivityIndicatorScope(syncIndicator, true))
                        {

                            var _clientStarCount = await manager.GetClientRateCount(usernameEntry.Text);
                            var _serviceStarCount = await manager.GetServiceRateCount(usernameEntry.Text);
                            var _clientStar = await manager.GetClientRate(usernameEntry.Text);
                            var _serviceStar = await manager.GetServiceRate(usernameEntry.Text);

                            clientStar = _clientStar;
                            serviceStar = _serviceStar;
                            clientStarCount = _clientStarCount;
                            serviceStarCount = _serviceStarCount;
                        }
                        await Navigation.PushAsync(new ContainerPage(usernameEntry.Text, clientStar, clientStarCount, serviceStar, serviceStarCount, manager));
                    } else
                    {
                        await DisplayAlert("Alert", "Password is not correct", "OK");
                        
                    }
                }
            }
            LogInBtn.IsEnabled = true;
            RegisterBtn.IsEnabled = true;
        }

        async void RegisterBtn_Clicked(object sender, EventArgs e)
        {
            LogInBtn.IsEnabled = false;
            RegisterBtn.IsEnabled = false;
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Alert", "No Internet, retry later!", "OK");
                RegisterBtn.IsEnabled = true;
                return;
            }


            if ((usernameEntry.Text.ToString().Length < 6) || (pwEntry.Text.ToString().Length < 6))
            {
                await DisplayAlert("Alert", "Username and Password require 6 characters minimum", "OK");

            }
            else
            {
                if (rpwEntry.Text.ToString().Length == 0)
                {
                    await DisplayAlert("Alert", "Please re-enter password", "OK");

                }
                else
                {
                    if (rpwEntry.Text != pwEntry.Text)
                    {
                        await DisplayAlert("Alert", "Passwords are not matched", "OK");
                    }
                    else
                    {
                        //await DisplayAlert("Alert", "Passwords are matched", "OK");
                        bool isUsernameExist;
                        using (var scope = new ActivityIndicatorScope(syncIndicator, true))
                        {
                            
                            var _isUsernameExist = await manager.IsUsernameExist(usernameEntry.Text);
                            
                            isUsernameExist = _isUsernameExist;
                        }

                        if (isUsernameExist)
                        {
                            await DisplayAlert("Alert", "Please choose another username", "OK");
                        }
                        else
                        {

                            var newAccount = new AccountModel
                            {
                                Username = usernameEntry.Text,
                                Password = pwEntry.Text,
                                RateClient = 0,
                                RateClientCount = 0,
                                RateServer = 0,
                                RateServerCount = 0
                            };
                            using (var scope = new ActivityIndicatorScope(syncIndicator, true))
                            {
                                await AddItem(newAccount);
                            }
                            await DisplayAlert("Alert", "You are registered!", "OK");

                            double clientStar, serviceStar;
                            int clientStarCount, serviceStarCount;
                            using (var scope = new ActivityIndicatorScope(syncIndicator, true))
                            {

                                var _clientStarCount = await manager.GetClientRateCount(usernameEntry.Text);
                                var _serviceStarCount = await manager.GetServiceRateCount(usernameEntry.Text);
                                var _clientStar = await manager.GetClientRate(usernameEntry.Text);
                                var _serviceStar = await manager.GetServiceRate(usernameEntry.Text);

                                clientStar = _clientStar;
                                serviceStar = _serviceStar;
                                clientStarCount = _clientStarCount;
                                serviceStarCount = _serviceStarCount;
                            }
                            await Navigation.PushAsync(new ContainerPage(usernameEntry.Text, clientStar, clientStarCount, serviceStar, serviceStarCount, manager));
                        }

                        



                    }
                }

            }
            RegisterBtn.IsEnabled = true;
            LogInBtn.IsEnabled = true;
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