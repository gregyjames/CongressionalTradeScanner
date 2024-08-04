using System.Collections.Concurrent;
using HtmlAgilityPack;

namespace CongressionalTradeScanner.Data.House;

public class Commitee
{
    private readonly HttpClient _client;
    public ConcurrentDictionary<string,Member> _members;
    public ConcurrentBag<SubCommitee> _SubCommitees;
    private static string URL;
    public string Title { get; set; }
    public Commitee(string url, HttpClient commiteeClient)
    {
        _members = new ConcurrentDictionary<string, Member>();
        _SubCommitees = new ConcurrentBag<SubCommitee>();
        URL = url;
        _client = commiteeClient;
    }
    public async Task Generate()
    {
        using var response = await _client.GetAsync(URL);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();

        var doc = new HtmlDocument();
        doc.LoadHtml(responseBody);

        
        var node_titles = new String[2]
        {
            "majority-members",
            "minority-members"
        };
        
        foreach (var title in node_titles)
        {
            var maj = doc.GetElementbyId(title);
            if (maj != null)
            {
                foreach (var li in maj.SelectNodes(".//span[@class='name']"))
                {
                    var text = li.InnerText.Split(" ");
                    var name = text[0].Split(",");

                    var First = name[1].Trim();
                    var Last = name[0].Trim();
                    var State = text[1].Trim();
                    var m = new Member(First, Last, State, "", false);
                    _members.TryAdd($"{First}-{Last}-{State}",m);
                }
            }
        }

        await LoadSubCommitees();
    }
    private async Task LoadSubCommitees()
    {
        using var response = await _client.GetAsync(URL);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(responseBody);
        var rightDiv = doc.DocumentNode.SelectNodes(".//section[contains(@class, 'subcommittees')]//a[contains(@class, 'library-committeePanel-subItems')]");
        if (rightDiv != null)
        {
            await Parallel.ForEachAsync(rightDiv, async (node, token) =>
            {
                var s = new SubCommitee(node.Attributes["href"].Value, _client, this);
                await s.GenerateSubCommittee();
                s.Title = node.InnerText.Trim();
                _SubCommitees.Add(s);
            });
        }
    }
}