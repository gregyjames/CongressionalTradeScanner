using CongressionalTradeScanner.Data;
using CongressionalTradeScanner.Data.House;
using CongressionalTradeScanner.Data.Senate;

class Program
{
    static async Task Main(string[] args)
    {
        var congress = new Congress();
        await congress.Build();
        congress.Display();
    }
}