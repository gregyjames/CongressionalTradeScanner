namespace CongressionalTradeScanner.Data.House;

public class Member
{
    public Member(string first, string last, string state, string party, bool subCommiteeOnly = false)
    {
        First = first;
        Last = last;
        State = state;
        Party = party;
        SubCommiteeOnly = subCommiteeOnly;
    }

    public string First { get; init; }
    public string Last { get; init; }
    public string State { get; init; }
    public string Party { get; init; }
    public bool SubCommiteeOnly { get; set; }

    public List<Trades.Member> GetTrades()
    {
        var trades = Trades.trades[getTradeKey].FirstOrDefault() ?? new List<Trades.Member>();
        return trades;
    }
    
    public override string ToString()
    {
        return First + " " + Last + ", " + State;
    }

    internal string getKey()
    {
        return $"{First}-{Last}-{State}";
    }

    private string getTradeKey => $"{First}-{Last}";

}