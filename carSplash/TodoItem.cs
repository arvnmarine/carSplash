using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace carSplash
{
	public class TodoItem
	{
		string id;
		string name;
		bool done;

		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return id; }
			set { id = value;}
		}

		[JsonProperty(PropertyName = "text")]
		public string Name
		{
			get { return name; }
			set { name = value;}
		}

		[JsonProperty(PropertyName = "complete")]
		public bool Done
		{
			get { return done; }
			set { done = value;}
		}

        [Version]
        public string Version { get; set; }
	}

    public class AccountModel
    {

        
        public string Username { get; set; }
        public string Password { get; set; }
        public double RateClient { get; set; }
        public int RateClientCount { get; set; }
        public double RateServer { get; set; }
        public int RateServerCount { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [CreatedAt]
        public DateTimeOffset CreatedAt { get; set; }

        [UpdatedAt]
        public DateTimeOffset UpdatedAt { get; set; }

        [Version]
        public string AzureVersion { get; set; }

        [Deleted]
        public bool IsDeleted { get; set; }
    }

    public class JobModel
    {
        
        public string Client { get; set; }
        public string Staff { get; set; }
        public string VehicleInfo { get; set; }
        public string Addr { get; set; }
        public string Date { get; set; }
        public string Package { get; set; }
        public double RateClient { get; set; }
        public int RateClientCount { get; set; }

        public bool ClientRateGiven { get; set; }
        public bool ServiceRateGiven { get; set; }
        public bool Done { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [CreatedAt]
        public DateTimeOffset CreatedAt { get; set; }

        [UpdatedAt]
        public DateTimeOffset UpdatedAt { get; set; }

        [Version]
        public string AzureVersion { get; set; }

        [Deleted]
        public bool IsDeleted { get; set; }
    }

    
}

