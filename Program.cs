namespace lektion15;

public class RequestThread {
    public async static Task<APIQuote> Run() {
        // En "stopwatch" som används för att räkna ut
        // hur lång tid det tar att köra denna metod.
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // Denna instans behövs för att kunna skicka iväg en HTTP request
        HttpClient httpClient = new HttpClient {
            BaseAddress = new Uri("https://dummyjson.com/")
        };

        // Denna skapar själva meddelandet som skall skickas
        HttpRequestMessage message = new HttpRequestMessage(
            HttpMethod.Get,
            "quotes/random"
        );

        // https://dummyjson.com/quotes/random

        // Skicka iväg själva meddelandet och få tillbaka ett svar
        HttpResponseMessage response = await httpClient.SendAsync(message);

        // Man måste läsa av innehållet (body) av svaret genom en StreamReader
        StreamReader reader = new StreamReader(response.Content.ReadAsStream());
    
        // Anropa koden som faktiskt hämtar ut innehållet i form av en sträng
        string body = reader.ReadToEnd();

        // Omvandla strängen (som är i JSON format) till ett objekt
        APIQuote quote = System.Text.Json.JsonSerializer.Deserialize<APIQuote>(
            body, 
            new System.Text.Json.JsonSerializerOptions {
                PropertyNameCaseInsensitive = true
            }
        )!;

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Console.WriteLine("Took " + elapsedMs + "ms");
        
        return quote;
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        
        // Starta igång alla funktioner samtidigt
        Task<APIQuote> task1 = RequestThread.Run();
        Task<APIQuote> task2 = RequestThread.Run();
        Task<APIQuote> task3 = RequestThread.Run();

        // Vänta på att alla funktioner blir klara.
        // Detta får effekten av att skicka 3 HTTP meddelanden samtidigt.
        await Task.WhenAll(task1, task2, task3);

        Console.WriteLine(task1.Result.Author);
        Console.WriteLine(task2.Result.Author);
        Console.WriteLine(task3.Result.Author);

        var elapsedMs = watch.ElapsedMilliseconds;
        Console.WriteLine("Everything took " + elapsedMs + "ms");
    }
}

public class APIQuote {
    public int Id { get; set; }
    public string Quote { get; set; }
    public string Author { get; set; }
}