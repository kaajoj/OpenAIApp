using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using OpenAIApp;

IConfiguration configuration = new ConfigurationBuilder()
    .AddUserSecrets("d9a5cd4e-7c74-46da-a5d5-7321a7d4ae95")
    .Build();

do
{
    Console.WriteLine("Press 1 to ask question");
    Console.WriteLine("Press 2 to generate image");
    var userSelection = Console.ReadLine();

    switch (userSelection)
    {
        case "1":
        {
            Console.WriteLine("Ask a Question:");
            var question = Console.ReadLine();

            var answer = OpenAI.AskQuestion(250, question, "text-davinci-002", 0.7, 1, 0, 0, configuration);
            Console.WriteLine(answer);
            break;
        }
        case "2":
        {
            Console.WriteLine("Input image description to create new image:");
            var prompt = Console.ReadLine();
            var imagesNumber = 5; // hardcoded

            var urls = OpenAI.CreateImage(prompt, imagesNumber, "512x512", configuration);

            foreach (var url in urls)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });

                Console.WriteLine(url);
            }
            break;
        }
    }
    Console.WriteLine("Do you want to ask another question or generate new image? (y/n)");
} while (Console.ReadLine() != "n");


