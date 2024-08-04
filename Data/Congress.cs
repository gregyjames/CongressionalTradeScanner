namespace CongressionalTradeScanner.Data;

public class Congress
{
    private readonly House.House _house;
    private readonly Senate.Senate _senate;

    public Congress()
    {
        _house = new House.House();
        _senate = new Senate.Senate();
    }

    public async Task Build()
    {
        await Task.WhenAll(new List<Task>()
        {
            _house.Build(),
            _senate.Build()
        });
    }

    public async Task Display()
    {
        Console.WriteLine("Senate");
        foreach (var commitee in _senate.Commitees)
        {
            Console.WriteLine("\t" + commitee.Key);
            foreach (var member in commitee.Value.Members.Member)
            {
                Console.WriteLine("\t\t" + member.Name.First + " " + member.Name.Last);
            }
            
            foreach (var subcommittee in commitee.Value.Subcommittee)
            {
                Console.WriteLine("\t\t" + subcommittee.SubcommitteeName);
                
                foreach (var member in subcommittee.Members.Member)
                {
                    Console.WriteLine("\t\t\t" + member.Name.First + " " + member.Name.Last);
                }
            }
        }
        Console.WriteLine("House");

        foreach (var commitee in _house.Commitees)
        {
            Console.WriteLine("\t" + commitee.Key);
            foreach (var member in commitee.Value._members.Values)
            {
                Console.WriteLine("\t\t" + member.First + " " + member.Last);
            }
            
            foreach (var subcommittee in commitee.Value._SubCommitees)
            {
                Console.WriteLine("\t\t" + subcommittee.Title);
                
                foreach (var member in subcommittee.GetMembers())
                {
                    Console.WriteLine("\t\t\t" + member.First + " " + member.Last);
                    var trades = member.GetTrades();
                    foreach (var trade in trades)
                    {
                        Console.WriteLine("\t\t\t\t" + trade.DocID);
                    }
                }
            }
        }
        
    }
}