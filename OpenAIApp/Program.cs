﻿// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

Console.WriteLine("Ask a Question:");
var question = Console.ReadLine();

IConfiguration _configuration = new ConfigurationBuilder()
    .AddUserSecrets("d9a5cd4e-7c74-46da-a5d5-7321a7d4ae95")
    .Build();

//call the open ai
var answer = callOpenAI(250, question, "text-davinci-002", 0.7, 1, 0, 0, _configuration);
Console.WriteLine(answer);

static string? callOpenAI(int tokens, string input, string engine, double temperature, int topP, int frequencyPenalty, int presencePenalty, IConfiguration _configuration)
{
    var openAiKey = _configuration["API_KEY"];

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