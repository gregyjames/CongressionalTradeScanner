using HtmlAgilityPack;

namespace CongressionalTradeScanner.Data.House;

public class SubCommitee
{
    private readonly string _url;
    private readonly HttpClient _commiteeClient;
    private readonly Commitee _parent;
    private HashSet<string> member_ids;
    private List<Member> commiteeOnlyMembers;
    public SubCommitee(string url, HttpClient commiteeClient, Commitee parent)
    {
        _url = url;
        _commiteeClient = commiteeClient;
        _parent = parent;
        member_ids = new HashSet<string>();
        commiteeOnlyMembers = new List<Member>();
    }

    public string Title { get; set; }

    public async Task GenerateSubCommittee()
    {
        using var response = await _commiteeClient.GetAsync(_url);
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

                    var key = $"{First}-{Last}-{State}";
                    var found = _parent._members.TryGetValue(key, out var member);
                    if(found)
                        member_ids.Add(member.getKey());
                    else
                    {
                        var newMember = new Member(
                            First,
                            Last,
                            State,
                            "",
                            true
                        );
                        commiteeOnlyMembers.Add(newMember);
                    }
                }
            }
        }
    }

    public IEnumerable<Member> GetMembers()
    {
        return member_ids.Select(id => _parent._members[id]).Concat(commiteeOnlyMembers);
    }
}