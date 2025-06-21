using ElectionsAppApi.Models;

namespace ElectionsAppApi.Repositories;

public class InMemoryCandidateRepository : ICandidateRepository
{
    private readonly List<Candidate> _candidates = new();
    private int _nextId = 1;

    public InMemoryCandidateRepository()
    {
        // Add some sample data
        _candidates.AddRange(new[]
        {
            new Candidate { Id = _nextId++, Name = "John Smith", Description = "Experienced politician with 10 years in office", Image = "john-smith.jpg", Party = "Democratic Party" },
            new Candidate { Id = _nextId++, Name = "Sarah Johnson", Description = "Former business executive turned politician", Image = "sarah-johnson.jpg", Party = "Republican Party" },
            new Candidate { Id = _nextId++, Name = "Michael Brown", Description = "Environmental activist and community leader", Image = "michael-brown.jpg", Party = "Green Party" }
        });
        
        Console.WriteLine($"InMemoryCandidateRepository initialized with {_candidates.Count} candidates. Next ID: {_nextId}");
    }

    public async Task<IEnumerable<Candidate>> GetAllAsync()
    {
        Console.WriteLine($"GetAllAsync called. Returning {_candidates.Count} candidates.");
        return await Task.FromResult(_candidates.AsEnumerable());
    }

    public async Task<Candidate?> GetByIdAsync(int id)
    {
        var candidate = _candidates.FirstOrDefault(c => c.Id == id);
        Console.WriteLine($"GetByIdAsync called with ID {id}. Found: {candidate != null}");
        return await Task.FromResult(candidate);
    }

    public async Task<Candidate> CreateAsync(Candidate candidate)
    {
        candidate.Id = _nextId++;
        _candidates.Add(candidate);
        Console.WriteLine($"CreateAsync called. Created candidate with ID {candidate.Id}, Name: {candidate.Name}. Total candidates: {_candidates.Count}");
        return await Task.FromResult(candidate);
    }

    public async Task<Candidate> UpdateAsync(Candidate candidate)
    {
        var existingCandidate = _candidates.FirstOrDefault(c => c.Id == candidate.Id);
        if (existingCandidate == null)
        {
            Console.WriteLine($"UpdateAsync called with ID {candidate.Id}. Candidate not found!");
            throw new ArgumentException($"Candidate with ID {candidate.Id} not found.");
        }

        var index = _candidates.IndexOf(existingCandidate);
        _candidates[index] = candidate;
        Console.WriteLine($"UpdateAsync called. Updated candidate with ID {candidate.Id}, Name: {candidate.Name}");
        
        return await Task.FromResult(candidate);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var candidate = _candidates.FirstOrDefault(c => c.Id == id);
        if (candidate == null)
        {
            Console.WriteLine($"DeleteAsync called with ID {id}. Candidate not found!");
            return await Task.FromResult(false);
        }

        _candidates.Remove(candidate);
        Console.WriteLine($"DeleteAsync called. Deleted candidate with ID {id}, Name: {candidate.Name}. Total candidates: {_candidates.Count}");
        return await Task.FromResult(true);
    }
} 