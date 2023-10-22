using Data;
using Microsoft.EntityFrameworkCore;

namespace MatchUpBot.Repositories;

public class ProfilesRepository
{
    private readonly Context _context = new();


    public void UpdateUserStage(long tgId, int newStage)
    {
        var user = _context.Users
            .FirstOrDefault(u => u.TgId == tgId);

        if (user != null)
        {
            user.Stage = newStage;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}