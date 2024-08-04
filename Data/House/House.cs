using HtmlAgilityPack;

namespace CongressionalTradeScanner.Data.House;

public class House
{
    public Dictionary<string, Commitee> Commitees;

    public House()
    {
        Commitees = new Dictionary<string, Commitee>();
    }

    public async Task Build()
    {
        var url = "https://clerk.house.gov/committees/AG00";
        using var client = new HttpClient();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(responseBody);
        var rightDiv = doc.DocumentNode.SelectNodes(".//div[contains(@class, 'dropdown-menu_left')]//li[contains(@role, 'menuitem')]");

        var commiteeClient = new HttpClient()
        {
            BaseAddress = new Uri("https://clerk.house.gov")
        };
        
        if (rightDiv != null)
        {
            var listItems = rightDiv.Descendants("a").Where(x => x.Attributes["href"].Value.Contains("/committees/")).ToList();
            
            await Parallel.ForEachAsync(listItems, async (node, token) =>
            {
                var title = node.InnerText;
                var commitee = new Commitee(node.Attributes["href"].Value, commiteeClient)
                {
                    Title = title
                };
                await commitee.Generate();
                Commitees.Add(title, commitee);
            });

            Trades t = new Trades();
            await t.DownloadTrades(2024);
        }
        else
        {
            Console.WriteLine("Right div not found.");
        }
    }
}