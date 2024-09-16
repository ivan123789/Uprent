using Azure.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleAppEntityLibrary.DTOs;
using System.Net.Http;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Forms;
public class HttpService
{
    private readonly HttpClient _httpClient;

    public HttpService(string baseUrl)
    {
        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new ArgumentNullException(nameof(baseUrl), "Base URL cannot be null or empty");
        }

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"API call failed: {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        // Log the raw JSON response to verify what is coming from the API
        Console.WriteLine($"API Response: {jsonResponse}");

        return JsonConvert.DeserializeObject<T>(jsonResponse);
    }

    public async Task<HttpResponseMessage> PutAsync(string requestUri, object data)
    {
        // Convert the object data to JSON and create HttpContent
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Use the already initialized _httpClient to send the request
        var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"API call failed: {response.StatusCode}");
        }

        return response;
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUri, object data)
    {
        // Serialize the data object into JSON
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Make the POST request using the initialized _httpClient
        var response = await _httpClient.PostAsync(requestUri, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"API call failed: {response.StatusCode}");
        }

        return response;
    }


}