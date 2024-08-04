using System.IO.Compression;
using System.Xml.Serialization;

namespace CongressionalTradeScanner.Data.House;

public class Trades
{
    [XmlRoot(ElementName="Member")]
    public class Member { 

        [XmlElement(ElementName="Prefix")] 
        public object Prefix { get; set; } 

        [XmlElement(ElementName="Last")] 
        public string Last { get; set; } 

        [XmlElement(ElementName="First")] 
        public string First { get; set; } 

        [XmlElement(ElementName="Suffix")] 
        public object Suffix { get; set; } 

        [XmlElement(ElementName="FilingType")] 
        public string FilingType { get; set; } 

        [XmlElement(ElementName="StateDst")] 
        public string StateDst { get; set; } 

        [XmlElement(ElementName="Year")] 
        public int Year { get; set; } 

        [XmlElement(ElementName="FilingDate")] 
        public string FilingDate { get; set; } 

        [XmlElement(ElementName="DocID")] 
        public int DocID { get; set; }

        public string key => ParseName(First + " " + Last).ToUpper();

        public async Task GetDocument()
        {
            var url = $"https://disclosures-clerk.house.gov/public_disc/ptr-pdfs/{Year}/{DocID}.pdf";
            await DownloadFileAsync(url, Directory.GetCurrentDirectory() + "/" + DocID + ".pdf");
        }
    }

    [XmlRoot(ElementName="FinancialDisclosure")]
    public class FinancialDisclosure { 

        [XmlElement(ElementName="Member")] 
        public List<Member> Member { get; set; } 
    }
    
    public static ILookup<string, List<Member>> trades;
    
    static async Task DownloadFileAsync(string url, string outputPath)
    {
        using HttpClient client = new HttpClient();
        try
        {
            // Send a GET request to the specified URL
            using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            // Read the content stream
            using Stream contentStream = await response.Content.ReadAsStreamAsync(),
                fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            // Copy the content stream to the file stream
            await contentStream.CopyToAsync(fileStream);
            Console.WriteLine($"File downloaded successfully to {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    
    public async Task DownloadTrades(int year)
    {
        var url = $"https://disclosures-clerk.house.gov/public_disc/financial-pdfs/{year}FD.zip";
        var dest = $"{year}.zip";
        using HttpClient client = new HttpClient();
        try
        {
            // Download the file as a stream
            using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using Stream contentStream = await response.Content.ReadAsStreamAsync(),
                fileStream = new FileStream(dest, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            // Copy the content stream to the file stream
            await contentStream.CopyToAsync(fileStream);
            
            contentStream.Close();
            fileStream.Close();
            
            // Get the current directory
            string extractPath = Directory.GetCurrentDirectory();

            // Extract the ZIP file to the current directory
            try
            {
                //ZipFile.ExtractToDirectory(dest, extractPath);
                ExtractZipFileToDirectory(dest, extractPath);
                Console.WriteLine($"Files extracted successfully to: {extractPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            
            Console.WriteLine("File downloaded successfully.");
            
            XmlSerializer serializer = new XmlSerializer(typeof(FinancialDisclosure));
            var str = File.ReadAllText($"{year}FD.xml");
            using StringReader reader = new StringReader(str);
            var test = (FinancialDisclosure)serializer.Deserialize(reader);
            trades = test.Member.GroupBy(x => x.key).ToLookup(x => x.Key, x => x.ToList());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    
    static string ParseName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("The full name cannot be null or whitespace.", nameof(fullName));
        }

        string[] parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 3)
        {
            return parts[0] + "-" + parts[2];
        }
        // Combine all parts except the last one for the first name
        string firstName = string.Join(" ", parts, 0, parts.Length - 1);
        string lastName = parts[parts.Length - 1];

        return firstName + "-" + lastName;
    }
    
    static void ExtractZipFileToDirectory(string zipPath, string extractPath)
    {
        using (ZipArchive archive = ZipFile.OpenRead(zipPath))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                // Ensure the destination path is within the extraction directory
                if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                {
                    // Create directory if necessary
                    if (Path.GetFileName(destinationPath).Length == 0)
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                    else
                    {
                        // Ensure the directory for the file exists
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                        // Overwrite the file if it exists
                        entry.ExtractToFile(destinationPath, true);
                    }
                }
            }
        }
    }
}