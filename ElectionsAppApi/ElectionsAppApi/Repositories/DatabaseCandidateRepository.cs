//using ElectionsAppApi.Models;
//using Microsoft.EntityFrameworkCore;

//namespace ElectionsAppApi.Repositories;

//// This is a sample implementation showing how to switch to Entity Framework Core later
//// To use this, you would need to:
//// 1. Create a DbContext
//// 2. Add the connection string to appsettings.json
//// 3. Register the DbContext in Program.cs
//// 4. Change the service registration from InMemoryCandidateRepository to DatabaseCandidateRepository
//public class DatabaseCandidateRepository : ICandidateRepository
//{
//    private readonly ApplicationDbContext _context;

//    public DatabaseCandidateRepository(ApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<IEnumerable<Candidate>> GetAllAsync()
//    {
//        return await _context.Candidates.ToListAsync();
//    }

//    public async Task<Candidate?> GetByIdAsync(int id)
//    {
//        return await _context.Candidates.FindAsync(id);
//    }

//    public async Task<Candidate> CreateAsync(Candidate candidate)
//    {
//        _context.Candidates.Add(candidate);
//        await _context.SaveChangesAsync();
//        return candidate;
//    }

//    public async Task<Candidate> UpdateAsync(Candidate candidate)
//    {
//        _context.Entry(candidate).State = EntityState.Modified;
//        await _context.SaveChangesAsync();
//        return candidate;
//    }

//    public async Task<bool> DeleteAsync(int id)
//    {
//        var candidate = await _context.Candidates.FindAsync(id);
//        if (candidate == null)
//        {
//            return false;
//        }

//        _context.Candidates.Remove(candidate);
//        await _context.SaveChangesAsync();
//        return true;
//    }
//}

//// Sample DbContext (uncomment and implement when ready to use database)
///*
//public class ApplicationDbContext : DbContext
//{
//    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
//    {
//    }

//    public DbSet<Candidate> Candidates { get; set; }

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        base.OnModelCreating(modelBuilder);

//        modelBuilder.Entity<Candidate>(entity =>
//        {
//            entity.HasKey(e => e.Id);
//            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
//            entity.Property(e => e.Description).HasMaxLength(500);
//            entity.Property(e => e.Image).HasMaxLength(200);
//            entity.Property(e => e.Party).IsRequired().HasMaxLength(100);
//        });
//    }
//}
//*/ 