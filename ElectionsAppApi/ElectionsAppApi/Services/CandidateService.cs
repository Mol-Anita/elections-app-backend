using ElectionsAppApi.Models;
using ElectionsAppApi.Repositories;

namespace ElectionsAppApi.Services;

public class CandidateService : ICandidateService
{
    private readonly ICandidateRepository _candidateRepository;

    public CandidateService(ICandidateRepository candidateRepository)
    {
        _candidateRepository = candidateRepository;
    }

    public async Task<IEnumerable<Candidate>> GetAllCandidatesAsync()
    {
        return await _candidateRepository.GetAllAsync();
    }

    public async Task<Candidate?> GetCandidateByIdAsync(int id)
    {
        return await _candidateRepository.GetByIdAsync(id);
    }

    public async Task<Candidate> CreateCandidateAsync(Candidate candidate)
    {
        // Validate candidate data
        if (string.IsNullOrWhiteSpace(candidate.Name))
        {
            throw new ArgumentException("Candidate name is required.");
        }

        if (string.IsNullOrWhiteSpace(candidate.Party))
        {
            throw new ArgumentException("Candidate party is required.");
        }

        return await _candidateRepository.CreateAsync(candidate);
    }

    public async Task<Candidate> UpdateCandidateAsync(Candidate candidate)
    {
        // Validate candidate data
        if (string.IsNullOrWhiteSpace(candidate.Name))
        {
            throw new ArgumentException("Candidate name is required.");
        }

        if (string.IsNullOrWhiteSpace(candidate.Party))
        {
            throw new ArgumentException("Candidate party is required.");
        }

        return await _candidateRepository.UpdateAsync(candidate);
    }

    public async Task<bool> DeleteCandidateAsync(int id)
    {
        return await _candidateRepository.DeleteAsync(id);
    }
} 