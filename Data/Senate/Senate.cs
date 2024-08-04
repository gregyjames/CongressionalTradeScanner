using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Serialization;
using HtmlAgilityPack;

namespace CongressionalTradeScanner.Data.Senate;

public class Senate
{
    private readonly HttpClient _client;
    public ConcurrentDictionary<string, SenateCommitee.Committees> Commitees;
    
    public Senate()
    {
        Commitees = new ConcurrentDictionary<string, SenateCommitee.Committees>();
        _client = new HttpClient();
    }

    public async Task<List<SenateMember>> GetMembers()
    {
        var member_url = "https://www.senate.gov/general/contact_information/senators_cfm.xml";
        using var response = await _client.GetAsync(member_url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        
        XmlSerializer serializer = new XmlSerializer(typeof(ContactInformation));
        using StringReader reader = new StringReader(responseBody);
        var test = (ContactInformation)serializer.Deserialize(reader);
        return test.Member;
    }

    public async Task Build()
    {
        var commitees = new List<string>()
        {
            "SSAF",
            "SSAP",
            "SSAS",
            "SSBK",
            "SSCM",
            "SSEG",
            "SSEV",
            "SSFI",
            "SSFR",
            "SSHR",
            "SSGA",
            "SLIA",
            "SSRA",
            "SSSB",
            "SSBU",
            "SSJU",
            "SSVA",
            "JSPR",
            "JSTX",
            "JSLC",
            "JSIK",
            "JSEC",
            "SLET",
            "SLIN",
            "SPAG",
            "SCNC",
            "JCSE",
        };
        
        await Parallel.ForEachAsync(commitees, async (commitee, token) =>
        {
            try
            {
                var member_url = $"https://www.senate.gov/general/committee_membership/committee_memberships_{commitee}.xml";
                using var response = await _client.GetAsync(member_url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var serializer = new XmlSerializer(typeof(SenateCommitee.CommitteeMembership));
                using var reader = new StringReader(responseBody);
                var test = (SenateCommitee.CommitteeMembership)serializer.Deserialize(reader);
                Commitees.TryAdd(test.Committees.CommitteeName, test.Committees);
            }
            catch
            {
                Console.WriteLine($"Error Processing: {commitee}");
            }
        });
    }
}