namespace Request1;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;



public interface IHttpClientWrapper
{
    Task<HttpResponseMessage> GetAsync(string requestUri);
}

public class HttpClientWrapper : IHttpClientWrapper
{
    private readonly HttpClient _httpClient;

    public HttpClientWrapper()
    {
        _httpClient = new HttpClient();
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        return await _httpClient.GetAsync(requestUri);
    }
}

public class Program1
{
    private readonly IHttpClientWrapper _httpClientWrapper;

    public Program1(IHttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }

    public static async Task Main(string[] args)
    {
        var program = new Program1(new HttpClientWrapper());
        await program.ProcessUserInformationAsync();
    }

    public async Task ProcessUserInformationAsync()
    {
        string base_url = "https://sef.podkolzin.consulting/api/users/lastSeen";
        int offset = 0;
        int pageSize = 20;

        while (true)
        {
            UserDataResponse userDataResponse = await FetchUserDataAsync(base_url, offset);
            if (userDataResponse == null)
            {
                Console.WriteLine("Failed to retrieve data.");
                break;
            }

            foreach (UserData user in userDataResponse.data)
            {
                DisplayUserInfo(user);
            }

            if (userDataResponse.data.Count < pageSize)
            {
                break;
            }
            offset += pageSize;
        }
    }

    public async Task<UserDataResponse?> FetchUserDataAsync(string baseUrl, int offset)
    {
        try
        {
            string apiUrl = $"{baseUrl}?offset={offset}";
            HttpResponseMessage response = await _httpClientWrapper.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string json_data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserDataResponse>(json_data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return null;
    }

    public void DisplayUserInfo(UserData user)
    {
        Console.WriteLine("User Information:");
        Console.WriteLine($"Name: {user.firstName}");
        Console.WriteLine($"Nickname: {user.nickname}");
        string lastSeenStatus = GetLastSeenStatus(user);
        Console.WriteLine($"Status: {lastSeenStatus}");
        Console.WriteLine();
    }


    public static string GetLastSeenStatus(UserData user)
    {
        DateTime current_time = DateTime.UtcNow;
        bool isOnline = user.isOnline;

        if (isOnline)
        {
            return "online";
        }

        string lastSeenStr = user.lastSeenDate;

        if (string.IsNullOrEmpty(lastSeenStr))
        {
            return "N/A";
        }


        DateTime lastSeenTime = DateTime.Parse(lastSeenStr).ToUniversalTime();
        TimeSpan timeElapsed = current_time - lastSeenTime;

        if (timeElapsed < TimeSpan.FromSeconds(30))
        {
            return "just now";
        }
        else if (timeElapsed < TimeSpan.FromMinutes(1))
        {
            return "less than a minute ago";
        }
        else if (timeElapsed < TimeSpan.FromMinutes(60))
        {
            return "a couple of minutes ago";
        }
        else if (timeElapsed < TimeSpan.FromMinutes(120))
        {
            return "an hour ago";
        }
        else if (timeElapsed < TimeSpan.FromDays(1))
        {
            return "today";
        }
        else if (timeElapsed < TimeSpan.FromDays(2))
        {
            return "yesterday";
        }
        else if (timeElapsed < TimeSpan.FromDays(7))
        {
            return "this week";
        }
        else
        {
            return "a long time ago";
        }
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
