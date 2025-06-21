using Microsoft.AspNetCore.Mvc;
using ElectionsAppApi.Models;
using ElectionsAppApi.Services;

namespace ElectionsAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidatesController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidatesController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    // GET: api/candidates
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Candidate>>> GetCandidates()
    {
        var candidates = await _candidateService.GetAllCandidatesAsync();
        return Ok(candidates);
    }

    // GET: api/candidates/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Candidate>> GetCandidate(int id)
    {
        var candidate = await _candidateService.GetCandidateByIdAsync(id);
        
        if (candidate == null)
        {
            return NotFound();
        }

        return Ok(candidate);
    }

    // POST: api/candidates
    [HttpPost]
    public async Task<ActionResult<Candidate>> CreateCandidate(Candidate candidate)
    {
        try
        {
            var createdCandidate = await _candidateService.CreateCandidateAsync(candidate);
            return CreatedAtAction(nameof(GetCandidate), new { id = createdCandidate.Id }, createdCandidate);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/candidates/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCandidate(int id, Candidate candidate)
    {
        if (id != candidate.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            var updatedCandidate = await _candidateService.UpdateCandidateAsync(candidate);
            return Ok(updatedCandidate);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE: api/candidates/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCandidate(int id)
    {
        var deleted = await _candidateService.DeleteCandidateAsync(id);
        
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
} 