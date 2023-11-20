using MatchUpBot.Repositories;

namespace EntityFrameworkLesson.Utils;

public static class MatchCalculator
{
    private static readonly double FIRST_MATCH_MULTIPLIER = 0.7;
    private static readonly double SECOND_MATCH_MULTIPLIER = 0.2;
    private static readonly double THIRD_MATCH_MULTIPLIER = 0.1;
    private static readonly double FORTH_MATCH_MULTIPLIER = 0.9;
    private static readonly double FIFTH_MATCH_MULTIPLIER = 0.1;

    private static readonly int NORMAL_NUMBER_OF_COMMON_INTERESTS = 1;
    private static readonly int GOOD_NUMBER_OF_COMMON_INTERESTS = 2;
    private static readonly int PERFECT_NUMBER_OF_COMMON_INTERESTS = 4;

    public static double CalculateMatch(long firstUserId,int firstPersonAge, int secPersonAge,
        string firstPersonZodiacSign,
        string secPersonZodiacSign,
        List<string> secPersonInterests, bool isZodiacSignImportant)
    {
        double match = 0;
        if (isZodiacSignImportant)
        {
            match = CalculateMatchByAge(firstPersonAge, secPersonAge) * FIRST_MATCH_MULTIPLIER +
                   CalculateMatchByZodiac(firstPersonZodiacSign.ToLower(), secPersonZodiacSign.ToLower()) * SECOND_MATCH_MULTIPLIER +
                   CalculateMatchByInterests(firstUserId, secPersonInterests) * THIRD_MATCH_MULTIPLIER;
            Console.WriteLine($"match: {match}");
            return match;
        }
        
        match =  CalculateMatchByAge(firstPersonAge, secPersonAge) * FORTH_MATCH_MULTIPLIER +
               CalculateMatchByInterests(firstUserId, secPersonInterests) * FIFTH_MATCH_MULTIPLIER;
        Console.WriteLine($"match: {match}");
        return match;
    }


    public static double CalculateMatchByInterests(long firstUserId,
        List<string> secPersonInterests)
    {
        var matchCount = MatchCount(firstUserId, secPersonInterests);
        
        if (matchCount < NORMAL_NUMBER_OF_COMMON_INTERESTS)
            return 60.0;

        if (matchCount < GOOD_NUMBER_OF_COMMON_INTERESTS)
            return 80.0;

        if (matchCount < PERFECT_NUMBER_OF_COMMON_INTERESTS)
            return 100.0;

        return 0.0;
    }

    public static double CalculateMatchByZodiac(string firstZodiac, string secondZodiac)
    {
        Dictionary<string, (string Element, string Quality)> zodiacCharacteristics = new()
        {
            { "овен", ("Fire", "Cardinal") },
            { "телец", ("Earth", "Fixed") },
            { "близнецы", ("Air", "Mutable") },
            { "рак", ("Water", "Cardinal") },
            { "лев", ("Fire", "Fixed") },
            { "дева", ("Earth", "Mutable") },
            { "весы", ("Air", "Cardinal") },
            { "скорпион", ("Water", "Fixed") },
            { "стрелец", ("Fire", "Mutable") },
            { "козерог", ("Earth", "Cardinal") },
            { "водолей", ("Air", "Fixed") },
            { "рыбы", ("Water", "Mutable") }
        };

        var elementFirst = zodiacCharacteristics[firstZodiac].Element;
        var elementSecond = zodiacCharacteristics[secondZodiac].Element;

        if (elementFirst == elementSecond)
        {
            var compatibilityScore = 70.0;

            var qualityFirst = zodiacCharacteristics[firstZodiac].Quality;
            var qualitySecond = zodiacCharacteristics[secondZodiac].Quality;

            if (qualityFirst == qualitySecond) compatibilityScore += 30.0;

            return compatibilityScore;
        }

        return 0.0;
    }

    public static string GetCompatibilityLevel(double compatibilityScore)
    {
        if (compatibilityScore < 50.0)
        {
            return "Плохая совместимость";
        }
        else if (compatibilityScore < 80.0)
        {
            return "Нормальная совместимость";
        }
        else
        {
            return "Хорошая совместимость";
        }
    }

    
    private static double CalculateMatchByAge(int firstPersonAge, int secPersonAge)
    {
        double yearDeference = Math.Abs(firstPersonAge - secPersonAge);

        return yearDeference switch
        {
            < 2 => 100.0,
            < 5 => 80.0,
            < 10 => 60.0,
            < 15 => 40.0,
            _ => 10.0
        };
    }



    private static double MatchCount(long firstUserId,
        List<string> secPersonInterests)
    {
        double matchCount = 0;
        var weightRepository = new InterestWeightRepository();
        foreach (var interest in secPersonInterests)
        {
            switch (interest)
            {
                case "спорт":
                    matchCount += (weightRepository.GetSportWeight(firstUserId) > 80) ? 1 : (weightRepository.GetSportWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "искусство":
                    matchCount += (weightRepository.GetArtWeight(firstUserId) > 80) ? 1 : (weightRepository.GetArtWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "музыка":
                    matchCount += (weightRepository.GetMusicWeight(firstUserId) > 80) ? 1 : (weightRepository.GetMusicWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "природа":
                    matchCount += (weightRepository.GetNatureWeight(firstUserId) > 80) ? 1 : (weightRepository.GetNatureWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "путешествия":
                    matchCount += (weightRepository.GetTravelWeight(firstUserId) > 80) ? 1 : (weightRepository.GetTravelWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "фотография":
                    matchCount += (weightRepository.GetPhotoWeight(firstUserId) > 80) ? 1 : (weightRepository.GetPhotoWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "кулинария":
                    matchCount += (weightRepository.GetCookingWeight(firstUserId) > 80) ? 1 : (weightRepository.GetCookingWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "кино":
                    matchCount += (weightRepository.GetMovieWeight(firstUserId) > 80) ? 1 : (weightRepository.GetMovieWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "литература":
                    matchCount += (weightRepository.GetLiteratureWeight(firstUserId) > 80) ? 1 : (weightRepository.GetLiteratureWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "наука":
                    matchCount += (weightRepository.GetScienceWeight(firstUserId) > 80) ? 1 : (weightRepository.GetScienceWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "технологии":
                    matchCount += (weightRepository.GetTechnologiesWeight(firstUserId) > 80) ? 1 : (weightRepository.GetTechnologiesWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "история":
                    matchCount += (weightRepository.GetHistoryWeight(firstUserId) > 80) ? 1 : (weightRepository.GetHistoryWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "психология":
                    matchCount += (weightRepository.GetPsychologyWeight(firstUserId) > 80) ? 1 : (weightRepository.GetPsychologyWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "религия":
                    matchCount += (weightRepository.GetReligionWeight(firstUserId) > 80) ? 1 : (weightRepository.GetReligionWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
                case "мода":
                    matchCount += (weightRepository.GetFashionWeight(firstUserId) > 80) ? 1 : (weightRepository.GetFashionWeight(firstUserId) > 40) ? 0.5 : 0;
                    break;
            }
        }
        return matchCount;
    }
}