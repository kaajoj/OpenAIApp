using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

IConfiguration configuration = new ConfigurationBuilder()
    .AddUserSecrets("d9a5cd4e-7c74-46da-a5d5-7321a7d4ae95")
    .Build();

do
{
    // Console.WriteLine("Ask a Question:");
    // var question = Console.ReadLine();
    //
    // var answer = AskQuestion(250, question, "text-davinci-002", 0.7, 1, 0, 0, configuration);
    // Console.WriteLine(answer);
    // Console.WriteLine("Do you want to ask another question? (y/n)");

    Console.WriteLine("Create new image:");
    var prompt = Console.ReadLine();
    var imagesNumber = 2; // hardcoded

    var urls = CreateImage(prompt, imagesNumber, "512x512", configuration);

    foreach (var url in urls)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });

        Console.WriteLine(url);
    }
    Console.WriteLine("Do you want to generate new image? (y/n)");
} while (Console.ReadLine() != "n");


static string? AskQuestion(int tokens, string input, string engine, double temperature, int topP, int frequencyPenalty, int presencePenalty, IConfiguration configuration)
{
    var openAiKey = configuration["API_KEY"];

    // https://beta.openai.com/docs/api-reference/introduction
    // TODO: The Engines endpoints are deprecated. Please use their replacement, Models, instead.
    var apiCall = "https://api.openai.com/v1/engines/" + engine + "/completions";

    try
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), apiCall))
            {
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + openAiKey);
                request.Content = new StringContent("{\n  \"prompt\": \"" + input + "\",\n  \"temperature\": " +
                                                    temperature.ToString(CultureInfo.InvariantCulture) + ",\n  \"max_tokens\": " + tokens + ",\n  \"top_p\": " + topP +
                                                    ",\n  \"frequency_penalty\": " + frequencyPenalty + ",\n  \"presence_penalty\": " + presencePenalty + "\n}");

                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var response = httpClient.SendAsync(request).Result;
                var json = response.Content.ReadAsStringAsync().Result;

                dynamic dynObj = JsonConvert.DeserializeObject(json);

                if (dynObj != null)
                {
                    return dynObj.choices[0].text.ToString();
                }

            }
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    return null;
}

static List<string> CreateImage(string prompt, int imagesNumber, string size, IConfiguration configuration)
{
    var openAiKey = configuration["API_KEY"];
    var apiCall = "https://api.openai.com/v1/images/generations";

    var urls = new List<string>();

    try
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), apiCall))
            {
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + openAiKey);
                request.Content = new StringContent("{\"prompt\":\"" + prompt + "\",\"n\": " + imagesNumber + ",\"size\":\"" + size + "\"}");
                
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var response = httpClient.SendAsync(request).Result;
                var json = response.Content.ReadAsStringAsync().Result;

                var dynamicObject = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

                if (dynamicObject != null)
                {
                    foreach (var element in dynamicObject["data"])
                    {
                        urls.Add(element["url"].ToString());
                    }
                    return urls;
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    return null;
}