/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=620342
 */
//#define OFFLINE_SYNC_ENABLED

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
#endif

namespace carSplash
{
    public partial class TodoItemManager
    {
        static TodoItemManager defaultInstance = new TodoItemManager();
        MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<TodoItem> todoTable;
#else
        IMobileServiceTable<TodoItem> todoTable;
        IMobileServiceTable<AccountModel> accountTable;
        IMobileServiceTable<JobModel> jobTable;
#endif

        const string offlineDbPath = @"localstore.db";

        private TodoItemManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<TodoItem>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.todoTable = client.GetSyncTable<TodoItem>();
#else
            this.todoTable = client.GetTable<TodoItem>();
            this.accountTable = client.GetTable<AccountModel>();
            this.jobTable = client.GetTable<JobModel>();
#endif
        }

        public static TodoItemManager DefaultManager
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        public MobileServiceClient CurrentClient
        {
            get { return client; }
        }

        public bool IsOfflineEnabled
        {
            get { return todoTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<TodoItem>; }
        }

        public async Task<ObservableCollection<TodoItem>> GetTodoItemsAsync(bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                IEnumerable<TodoItem> items = await todoTable
                    .Where(todoItem => !todoItem.Done)
                    .ToEnumerableAsync();

                return new ObservableCollection<TodoItem>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe.Message });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e.Message });
            }
            return null;
        }

        public async Task<ObservableCollection<JobModel>> GetJobModelAsync(bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                IEnumerable<JobModel> items = await jobTable
                    .Where(Item => !Item.IsDeleted)
                    .ToEnumerableAsync();

                return new ObservableCollection<JobModel>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe.Message });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e.Message });
            }
            return null;
        }

        public async Task SaveTaskAsync(TodoItem item)
        {
            try
            {
                if (item.Id == null)
                {
                    await todoTable.InsertAsync(item);
                }
                else
                {
                    await todoTable.UpdateAsync(item);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Save error: {0}", new[] { e.Message });
            }
        }

        public async Task SaveTaskAsync(AccountModel item)
        {
            try
            {
                if (item.Id == null)
                {
                    await accountTable.InsertAsync(item);
                }
                else
                {
                    await accountTable.UpdateAsync(item);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Save error: {0}", new[] { e.Message });
            }
        }

        public async Task SaveTaskAsync(JobModel item)
        {
            try
            {
                if (item.Id == null)
                {
                    await jobTable.InsertAsync(item);
                }
                else
                {
                    await jobTable.UpdateAsync(item);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Save error: {0}", new[] { e.Message });
            }
        }


        public async Task<bool> IsUsernameExist(string username)
        {

            var names = await accountTable
                  .Where(t => t.Username == username)
                  .Select(t => t.Username)
                  .ToCollectionAsync();
            foreach (string item in names)
            {
                if (item == username)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> IsMatchPw(string username, string pw)
        {

            var pws = await accountTable
                  .Where(t => t.Username == username)
                  .Select(t => t.Password)
                  .ToCollectionAsync();
            foreach (string item in pws)
            {
                if (item == pw)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<int> GetClientRateCount(string username)
        {

            var tmp = await accountTable
                  .Where(t => t.Username == username)
                  .Select(t => t.RateClientCount)
                  .ToCollectionAsync();

            foreach (int item in tmp)
            {
                return item;
            }
            return 0;
        }

        public async Task<double> GetClientRate(string username)
        {

            var tmp = await accountTable
                  .Where(t => t.Username == username)
                  .Select(t => t.RateClient)
                  .ToCollectionAsync();

            foreach (double item in tmp)
            {
                return item;
            }
            return 0;
        }

        public async Task<int> GetServiceRateCount(string username)
        {

            var tmp = await accountTable
                  .Where(t => t.Username == username)
                  .Select(t => t.RateServerCount)
                  .ToCollectionAsync();

            foreach (int item in tmp)
            {
                return item;
            }
            return 0;
        }

        public async Task<double> GetServiceRate(string username)
        {

            var tmp = await accountTable
                  .Where(t => t.Username == username)
                  .Select(t => t.RateServer)
                  .ToCollectionAsync();

            foreach (double item in tmp)
            {
                return item;
            }
            return 0;
        }

        public async Task RateClient(string username, int rate)
        {
            IEnumerable<AccountModel> accounts = await accountTable
                    .Where(Item => Item.Username == username)
                    .ToEnumerableAsync();
            foreach (AccountModel acc in accounts)
            {
                if (acc.Username == username)
                {
                    acc.RateClient = (acc.RateClient * acc.RateClientCount + rate) / (acc.RateClientCount + 1);
                    acc.RateClientCount += 1;
                    await SaveTaskAsync(acc);
                    return;
                }
            }

        }

        public async Task RateStaff(string username, int rate)
        {
            IEnumerable<AccountModel> accounts = await accountTable
                    .Where(Item => Item.Username == username)
                    .ToEnumerableAsync();
            foreach (AccountModel acc in accounts)
            {
                if (acc.Username == username)
                {
                    acc.RateServer = (acc.RateServer * acc.RateServerCount + rate) / (acc.RateServerCount + 1);
                    acc.RateServerCount += 1;
                    await SaveTaskAsync(acc);
                    return;
                }
            }

        }


    }
        

#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.todoTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allTodoItems",
                    this.todoTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }

            // Simple error/conflict handling. A real application would handle the various errors like network conditions,
            // server conflicts and others via the IMobileServiceSyncHandler.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        //Update failed, reverting to server's copy.
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change.
                        await error.CancelAndDiscardItemAsync();
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
#endif
    
}
