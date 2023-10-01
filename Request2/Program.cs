namespace Request1;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks.Sources;

public class Program1
{
    static async Task Main(string[] args)
    {
        string base_url = "https://sef.podkolzin.consulting/api/users/lastSeen";
        int offset = 0;
        int page_size = 20;

        while (true)
        {
            string api_url = $"{base_url}?offset={offset}";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(api_url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to retrieve data. Status code: {response.StatusCode}");
                    break;
                }

                string json_data = await response.Content.ReadAsStringAsync();
                UserDataResponse userDataResponse = JsonConvert.DeserializeObject<UserDataResponse>(json_data);



                foreach (UserData user in userDataResponse.data)
                {
                    Console.WriteLine("User Information:");
                    Console.WriteLine($"Name: {user.firstName}");
                    Console.WriteLine($"Nickname: {user.nickname}");
                    Console.WriteLine($"Was: {user.lastSeenDate}");
                    string lastSeenStatus = GetLastSeenStatus(user);
                    Console.WriteLine($"Status: {lastSeenStatus}");
                    Console.WriteLine();
                }

                if (userDataResponse.data.Count < page_size)
                {
                    break;
                }
                offset += page_size;
            }
        }
    }

    public static string GetLastSeenStatus(UserData user)
    {
        DateTime current_time = DateTime.UtcNow;
        bool isOnline = user.isOnline;

        if (isOnline)
        {
            return "online";
        }
        else
        {
            return " offline";
        }

    }

    public class UserDataResponse
    {
        public List<UserData> data { get; set; }
    }

    public class UserData
    {
        public string firstName { get; set; }
        public string nickname { get; set; }
        public bool isOnline { get; set; }
        public string lastSeenDate { get; set; }
    }
}
