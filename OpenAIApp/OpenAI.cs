using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;

namespace OpenAIApp
{
    public class OpenAI
    {
        public static string? AskQuestion(int tokens, string input, string engine, double temperature, int topP, int frequencyPenalty, int presencePenalty, IConfiguration configuration)
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

        public static List<string> CreateImage(string prompt, int imagesNumber, string size, IConfiguration configuration)
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
    }
}
