using Data;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchUpBot.Repositories;

public class InterestWeightRepository
{
    private readonly Context _context = new();


    public async void CreateInterestWeight(long tgId)
    {
        _context.InterestWeightEntities.Add(new InterestWeightEntity
        {
            Id = Guid.NewGuid(),
            UserId = tgId
        });
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateUserInterestWeightDecrement(long userId, List<string> interestList)
    {
        var interestWeight = _context.InterestWeightEntities.AsNoTracking()
            .FirstOrDefault(entity => entity.UserId == userId);
        foreach (var interest in interestList)
        {
            switch (interest)
            {
                case "спорт":
                    if (interestWeight.SportWeight > 0)
                        interestWeight.SportWeight--;
                    break;
                case "искусство":
                    if (interestWeight.ArtWeight > 0)
                        interestWeight.ArtWeight--;
                    break;
                case "музыка":
                    if(interestWeight.MusicWeight > 0)
                        interestWeight.MusicWeight--;
                    break;
                case "природа": 
                    if(interestWeight.NatureWeight > 0)
                        interestWeight.NatureWeight--;
                    break;
                case "путешествия":
                    if(interestWeight.TravelWeight > 0)
                        interestWeight.TravelWeight--;
                    break;
                case "фотография":
                    if(interestWeight.PhotoWeight > 0)
                        interestWeight.PhotoWeight--;
                    break;
                case "кулинария":
                    if(interestWeight.CookingWeight > 0)
                        interestWeight.CookingWeight--;
                    break;
                case "кино":
                    if(interestWeight.MovieWeight > 0)
                        interestWeight.MovieWeight--;
                    break;
                case "литература":
                    if(interestWeight.LiteratureWeight > 0)
                        interestWeight.LiteratureWeight--;
                    break;
                case "наука":
                    if(interestWeight.ScienceWeight > 0)
                        interestWeight.ScienceWeight--;
                    break;
                case "технологии":
                    if(interestWeight.TechnologiesWeight > 0)
                        interestWeight.TechnologiesWeight--;
                    break;
                case "история":
                    if(interestWeight.HistoryWeight > 0)
                        interestWeight.HistoryWeight--;
                    break;
                case "психология":
                    if(interestWeight.PsychologyWeight > 0)
                        interestWeight.PsychologyWeight--;
                    break; 
                case "религия":
                    if(interestWeight.ReligionWeight > 0)
                        interestWeight.ReligionWeight--;
                    break;
                case "мода":
                    if(interestWeight.FashionWeight > 0) 
                        interestWeight.FashionWeight--;
                    break;
            }
        }
        await _context.SaveChangesAsync();
    }
    
     public async Task UpdateUserInterestWeightIncrement(long userId, List<string> interestList)
    {
        var interestWeight = _context.InterestWeightEntities.AsNoTracking()
            .FirstOrDefault(entity => entity.UserId == userId);
        if (interestWeight == null) 
            return;
        foreach (var interest in interestList)
        {
            switch (interest)
            {
                case "спорт":
                    if (interestWeight.SportWeight < 100)
                        interestWeight.SportWeight++;
                    break;
                case "искусство":
                    if (interestWeight.ArtWeight < 100)
                        interestWeight.ArtWeight++;
                    break;
                case "музыка":
                    if(interestWeight.MusicWeight < 100)
                        interestWeight.MusicWeight++;
                    break;
                case "природа": 
                    if(interestWeight.NatureWeight < 100)
                        interestWeight.NatureWeight++;
                    break;
                case "путешествия":
                    if(interestWeight.TravelWeight < 100)
                        interestWeight.TravelWeight++;
                    break;
                case "фотография":
                    if(interestWeight.PhotoWeight < 100)
                        interestWeight.PhotoWeight++;
                    break;
                case "кулинария":
                    if(interestWeight.CookingWeight < 100)
                        interestWeight.CookingWeight++;
                    break;
                case "кино":
                    if(interestWeight.MovieWeight < 100)
                        interestWeight.MovieWeight++;
                    break;
                case "литература":
                    if(interestWeight.LiteratureWeight < 100)
                        interestWeight.LiteratureWeight++;
                    break;
                case "наука":
                    if(interestWeight.ScienceWeight < 100)
                        interestWeight.ScienceWeight++;
                    break;
                case "технологии":
                    if(interestWeight.TechnologiesWeight < 100)
                        interestWeight.TechnologiesWeight++;
                    break;
                case "история":
                    if(interestWeight.HistoryWeight < 100)
                        interestWeight.HistoryWeight++;
                    break;
                case "психология":
                    if(interestWeight.PsychologyWeight < 100)
                        interestWeight.PsychologyWeight++;
                    break; 
                case "религия":
                    if(interestWeight.ReligionWeight < 100)
                        interestWeight.ReligionWeight++;
                    break;
                case "мода":
                    if(interestWeight.FashionWeight < 100) 
                        interestWeight.FashionWeight++;
                    break;
            }
        }
        await _context.SaveChangesAsync();
    }

     public byte GetSportWeight(long userId)
     {
         var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId); 
         return entity?.SportWeight ?? (byte)50;
     }

     public byte GetArtWeight(long userId)
     {
         var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
         return entity?.ArtWeight ?? (byte)50;
     }

     public byte GetNatureWeight(long userId)
     {
         var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
         return entity?.NatureWeight ?? (byte)50;
     }
     
      public byte GetMusicWeight(long userId)
    {
        var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
        return entity?.MusicWeight ?? (byte)50;
    }

    public byte GetTravelWeight(long userId)
    {
        var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
        return entity?.TravelWeight ?? (byte)50;
    }

    public byte GetPhotoWeight(long userId)
    {
        var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
        return entity?.PhotoWeight ?? (byte)50;
    }

    public byte GetCookingWeight(long userId)
    {
        var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
        return entity?.CookingWeight ?? (byte)50;
    }

    public byte GetMovieWeight(long userId)
    {
        var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
        return entity?.MovieWeight ?? (byte)50;
    }

    public byte GetLiteratureWeight(long userId)
    {
        var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
        return entity?.LiteratureWeight ?? (byte)50;
    }

    public byte GetScienceWeight(long userId)
    {
        var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
        return entity?.ScienceWeight ?? (byte)50;
    }

    public byte GetTechnologiesWeight(long userId)
    {
        var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
        return entity?.TechnologiesWeight ?? (byte)50;
    }

    public byte GetHistoryWeight(long userId)
    {
        var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
        return entity?.HistoryWeight ?? (byte)50;
    }

    public byte GetPsychologyWeight(long userId)
    {
        var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
        return entity?.PsychologyWeight ?? (byte)50;
    }

    public byte GetReligionWeight(long userId)
    {
        var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
        return entity?.ReligionWeight ?? (byte)50;
    }

    public byte GetFashionWeight(long userId)
    {
        var entity = _context.InterestWeightEntities.AsNoTracking().FirstOrDefault(entity => entity.UserId == userId);
        return entity?.FashionWeight ?? (byte)50;
    }

}