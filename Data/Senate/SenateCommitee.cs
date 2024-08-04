using System.Xml.Serialization;

namespace CongressionalTradeScanner.Data.Senate;

public class SenateCommitee
{
    [XmlRoot(ElementName="name")]
    public class Name { 

        [XmlElement(ElementName="first")] 
        public string First { get; set; } 

        [XmlElement(ElementName="last")] 
        public string Last { get; set; } 
    }

    [XmlRoot(ElementName="member")]
    public class Member { 

        [XmlElement(ElementName="name")] 
        public Name Name { get; set; } 

        [XmlElement(ElementName="state")] 
        public string State { get; set; } 

        [XmlElement(ElementName="party")] 
        public string Party { get; set; } 

        [XmlElement(ElementName="position")] 
        public string Position { get; set; } 
    }

    [XmlRoot(ElementName="members")]
    public class Members { 

        [XmlElement(ElementName="member")] 
        public List<Member> Member { get; set; } 
    }

    [XmlRoot(ElementName="subcommittee")]
    public class Subcommittee { 

        [XmlElement(ElementName="subcommittee_name")] 
        public string SubcommitteeName { get; set; } 

        [XmlElement(ElementName="committee_code")] 
        public string CommitteeCode { get; set; } 

        [XmlElement(ElementName="members")] 
        public Members Members { get; set; } 
    }

    [XmlRoot(ElementName="committees")]
    public class Committees { 

        [XmlElement(ElementName="majority_party")] 
        public string MajorityParty { get; set; } 

        [XmlElement(ElementName="committee_name")] 
        public string CommitteeName { get; set; } 

        [XmlElement(ElementName="committee_code")] 
        public string CommitteeCode { get; set; } 

        [XmlElement(ElementName="members")] 
        public Members Members { get; set; } 

        [XmlElement(ElementName="subcommittee")] 
        public List<Subcommittee> Subcommittee { get; set; } 
    }

    [XmlRoot(ElementName="committee_membership")]
    public class CommitteeMembership { 

        [XmlElement(ElementName="committees")] 
        public Committees Committees { get; set; } 
    }


}