using ElectionsAppApi.Models;

namespace ElectionsAppApi.Services;

public class CandidateGenerator
{
    private readonly Random _random = new Random();
    
    private readonly string[] _firstNames = {
        "John", "Jane", "Michael", "Sarah", "David", "Emily", "Robert", "Jessica", "William", "Ashley",
        "James", "Amanda", "Christopher", "Stephanie", "Daniel", "Nicole", "Matthew", "Elizabeth", "Anthony", "Helen",
        "Mark", "Deborah", "Donald", "Rachel", "Steven", "Carolyn", "Paul", "Janet", "Andrew", "Catherine",
        "Joshua", "Maria", "Kenneth", "Heather", "Kevin", "Diane", "Brian", "Ruth", "George", "Julie",
        "Timothy", "Joyce", "Ronald", "Virginia", "Jason", "Victoria", "Edward", "Kelly", "Jeffrey", "Christine"
    };
    
    private readonly string[] _lastNames = {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
        "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
        "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
        "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
        "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts"
    };
    
    private readonly string[] _parties = {
        "Democratic Party", "Republican Party", "Green Party", "Libertarian Party", "Independent",
        "Progressive Party", "Conservative Party", "Liberal Party", "Centrist Party", "Reform Party"
    };
    
    private readonly string[] _professions = {
        "lawyer", "business executive", "teacher", "doctor", "engineer", "politician", "activist", "professor",
        "consultant", "entrepreneur", "community leader", "veteran", "environmentalist", "economist", "journalist"
    };
    
    private readonly string[] _focusAreas = {
        "education reform", "healthcare", "economic development", "environmental protection", "social justice",
        "infrastructure", "national security", "immigration reform", "tax policy", "climate change",
        "criminal justice reform", "rural development", "urban planning", "technology innovation", "foreign policy"
    };

    public Candidate GenerateCandidate()
    {
        var firstName = _firstNames[_random.Next(_firstNames.Length)];
        var lastName = _lastNames[_random.Next(_lastNames.Length)];
        var name = $"{firstName} {lastName}";
        
        var party = _parties[_random.Next(_parties.Length)];
        var profession = _professions[_random.Next(_professions.Length)];
        var focusArea = _focusAreas[_random.Next(_focusAreas.Length)];
        
        var description = GenerateDescription(firstName, lastName, profession, focusArea);
        var image = GenerateImageUrl(firstName, lastName);
        
        return new Candidate
        {
            Name = name,
            Description = description,
            Image = image,
            Party = party
        };
    }
    
    private string GenerateDescription(string firstName, string lastName, string profession, string focusArea)
    {
        var descriptions = new[]
        {
            $"A {profession} with over {_random.Next(5, 25)} years of experience, {firstName} {lastName} is passionate about {focusArea}.",
            $"{firstName} {lastName}, a dedicated {profession}, has been working tirelessly to address {focusArea} in our community.",
            $"With a background in {profession}, {firstName} {lastName} brings unique insights to the challenges of {focusArea}.",
            $"A former {profession} turned public servant, {firstName} {lastName} focuses on innovative solutions for {focusArea}.",
            $"{firstName} {lastName} is a {profession} who has dedicated their career to advancing {focusArea} policies."
        };
        
        return descriptions[_random.Next(descriptions.Length)];
    }
    
    private string GenerateImageUrl(string firstName, string lastName)
    {
        // Generate a placeholder image URL
        var name = $"{firstName}-{lastName}".ToLower().Replace(" ", "-");
        return $"https://via.placeholder.com/150?text={Uri.EscapeDataString($"{firstName}+{lastName}")}";
    }
} 