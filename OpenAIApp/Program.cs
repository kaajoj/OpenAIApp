using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using OpenAIApp;

IConfiguration configuration = new ConfigurationBuilder()
    .AddUserSecrets("d9a5cd4e-7c74-46da-a5d5-7321a7d4ae95")
    .Build();

do
{
    Console.WriteLine("Ask a Question:");
    var question = Console.ReadLine();
    
    var answer = OpenAI.AskQuestion(250, question, "text-davinci-002", 0.7, 1, 0, 0, configuration);
    Console.WriteLine(answer);
    Console.WriteLine("Do you want to ask another question? (y/n)");

    Console.WriteLine("Create new image:");
    var prompt = Console.ReadLine();
    var imagesNumber = 2; // hardcoded

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
    Console.WriteLine("Do you want to generate new image? (y/n)");
} while (Console.ReadLine() != "n");


