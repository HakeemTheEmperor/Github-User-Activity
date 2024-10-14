using Newtonsoft.Json;

namespace Github_User_Activity
{
    internal class Program
    {
        public class GitEvent
        {
            public required string Type { get; set; }
            public DateTime Created_At { get; set; }
            public required GitEventActor Actor { get; set; }
            public required GitRepo Repo { get; set; }

            public class GitEventActor
            {
                public required string Login { get; set; }
                public required string Url { get; set; }
            }

            public class GitRepo
            {
                public required string Name { get; set; }
                public required string Url { get; set; }
            }
        }

        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Invalid Format. Correct format is '{AppName} <username>' ");
                return;
            }

            var username = args[0];
            var url = $"https://api.github.com/users/{username}/events";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "C# Console App");

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    var events = JsonConvert.DeserializeObject<GitEvent[]>(responseBody);


                    if (events?.Length < 1)
                    {
                        Console.WriteLine($"{username} has a private status and cannot be viewed");
                        return;
                    }

                    Console.WriteLine($"Events for user: {username}");
                    Console.WriteLine(events?.Length);

                    if (events != null)
                        foreach (var githubEvent in events)
                        {
                            Console.WriteLine($"Event Type: {githubEvent.Type}");
                            Console.WriteLine($"Created At: {githubEvent.Created_At}");

                            // Check for null before accessing the Actor property
                            Console.WriteLine(githubEvent.Actor != null
                                ? $"Actor: {githubEvent.Actor.Login}"
                                : "Actor: Not available");

                            // Check for null before accessing the Repo property
                            Console.WriteLine(githubEvent.Repo != null
                                ? $"Repo: {githubEvent.Repo.Name}"
                                : "Repo: Not available");

                            Console.WriteLine($"------------------------");
                        }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine(e.Message.Contains("404")
                        ? $"User {username} not found. Confirm that a user with that username exists"
                        // Handle other request errors
                        : $"Request error: {e.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An unexpected error occured. {e.Message}");
                }
            }
        }
    }
}
