using System.Xml.Serialization;

namespace CongressionalTradeScanner.Data.Senate;
// using System.Xml.Serialization;
// XmlSerializer serializer = new XmlSerializer(typeof(ContactInformation));
// using (StringReader reader = new StringReader(xml))
// {
//    var test = (ContactInformation)serializer.Deserialize(reader);
// }

[XmlRoot(ElementName="member")]
public class SenateMember { 

    [XmlElement(ElementName="member_full")] 
    public string MemberFull { get; set; } 

    [XmlElement(ElementName="last_name")] 
    public string LastName { get; set; } 

    [XmlElement(ElementName="first_name")] 
    public string FirstName { get; set; } 

    [XmlElement(ElementName="party")] 
    public string Party { get; set; } 

    [XmlElement(ElementName="state")] 
    public string State { get; set; } 

    [XmlElement(ElementName="address")] 
    public string Address { get; set; } 

    [XmlElement(ElementName="phone")] 
    public string Phone { get; set; } 

    [XmlElement(ElementName="email")] 
    public string Email { get; set; } 

    [XmlElement(ElementName="website")] 
    public string Website { get; set; } 

    [XmlElement(ElementName="class")] 
    public string Class { get; set; } 

    [XmlElement(ElementName="bioguide_id")] 
    public string BioguideId { get; set; } 

    [XmlElement(ElementName="leadership_position")] 
    public string LeadershipPosition { get; set; } 
    
    internal string getKey()
    {
        return $"{FirstName.ToUpper()}-{LastName.ToUpper()}-{State.ToUpper()}";
    }

    internal string getElementKey()
    {
        return $"{LastName}{State}";
    }
}

[XmlRoot(ElementName="contact_information")]
public class ContactInformation { 

    [XmlElement(ElementName="member")] 
    public List<SenateMember> Member { get; set; } 

    //[XmlElement(ElementName="last_updated")] 
    //public DateTime LastUpdated { get; set; } 
}

