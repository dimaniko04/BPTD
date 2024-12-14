using FundRaising.Server.BLL.Interfaces.Repository;
using FundRaising.Server.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundRaising.Server.DAL.Repository;

public class FundraiserRepository
    : GenericRepository<Fundraiser>, IFundraisersRepository
{
    public FundraiserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Fundraiser>> 
        GetAllUserFundraisers(Guid userId)
    {
        return await DbSet
            .Where(f => f.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> IsUserFundraiser(Guid userId, Guid fundraiserId)
    {
        return await DbSet.AnyAsync(f => 
            f.Id == fundraiserId && f.UserId == userId);
    }

    public async Task AddDonation(Donation donation)
    {
        await _context.Donations.AddAsync(donation);
    }
}