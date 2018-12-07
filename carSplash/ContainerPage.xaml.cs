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
    public partial class ContainerPage : TabbedPage
    {
        public ContainerPage (string username, double clientStar, int clientStarCount, double serviceStar, int serviceStarCount, TodoItemManager manager)
        {

            InitializeComponent();
            var navigationPage1 = new PersonalPage(username, clientStar, clientStarCount, serviceStar, serviceStarCount, manager);
            navigationPage1.Icon = "personal.png";
            navigationPage1.Title = "Personal";
            Children.Add(navigationPage1);

            
            
            var navigationPage3 = new MyWashPage(username, clientStar, clientStarCount, manager);
            navigationPage3.Icon = "mywash.png";
            navigationPage3.Title = "My Wash";
            Children.Add(navigationPage3);

            var navigationPage2 = new MarketplacePage(username, clientStar, clientStarCount, serviceStar, serviceStarCount, manager);
            navigationPage2.Icon = "marketplace.png";
            navigationPage2.Title = "Market";
            Children.Add(navigationPage2);


            var navigationPage4 = new MyJobPage(username, manager);
            navigationPage4.Icon = "myjob.png";
            navigationPage4.Title = "My Job";
            Children.Add(navigationPage4);


        }

        
    }
}