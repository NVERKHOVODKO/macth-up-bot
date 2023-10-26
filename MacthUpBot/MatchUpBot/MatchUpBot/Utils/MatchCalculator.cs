namespace EntityFrameworkLesson.Utils;

public static class MatchCalculator
{
    private static readonly double FIRST_MATCH_MULTIPLIER = 0.4;
    private static readonly double SECOND_MATCH_MULTIPLIER = 0.4;
    private static readonly double THIRD_MATCH_MULTIPLIER = 0.2;
    private static readonly double FORTH_MATCH_MULTIPLIER = 0.8;
    private static readonly double FIFTH_MATCH_MULTIPLIER = 0.2;

    private static readonly int NORMAL_NUMBER_OF_COMMON_INTERESTS = 1;
    private static readonly int GOOD_NUMBER_OF_COMMON_INTERESTS = 2;
    private static readonly int PERFECT_NUMBER_OF_COMMON_INTERESTS = 4;

    public static double CalculateMatch(int firstPersonAge, int secPersonAge,
        string firstPersonZodiacSign,
        string secPersonZodiacSign,
        List<string> firstPersonInterests,
        List<string> secPersonInterests, bool isZodiacSignImportant)
    {
        if (isZodiacSignImportant)
            return CalculateMatchByAge(firstPersonAge, secPersonAge) * FIRST_MATCH_MULTIPLIER +
                   CalculateMatchByZodiac(firstPersonZodiacSign, secPersonZodiacSign) * SECOND_MATCH_MULTIPLIER +
                   CalculateMatchByInterests(firstPersonInterests, secPersonInterests) * THIRD_MATCH_MULTIPLIER;

        return CalculateMatchByAge(firstPersonAge, secPersonAge) * FORTH_MATCH_MULTIPLIER +
               CalculateMatchByInterests(firstPersonInterests, secPersonInterests) * FIFTH_MATCH_MULTIPLIER;
    }


    private static double CalculateMatchByInterests(List<string> firstPersonInterests,
        List<string> secPersonInterests)
    {
        var matchCount = firstPersonInterests.Intersect(secPersonInterests).Count();

        if (matchCount < NORMAL_NUMBER_OF_COMMON_INTERESTS)
            return 60.0;

        if (matchCount < NORMAL_NUMBER_OF_COMMON_INTERESTS)
            return 80.0;

        if (matchCount < PERFECT_NUMBER_OF_COMMON_INTERESTS)
            return 100.0;

        return 0.0;
    }

    private static double CalculateMatchByZodiac(string firstZodiac, string secondZodiac)
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

    private static double CalculateMatchByAge(int firstPersonAge, int secPersonAge)
    {
        double yearDeference = Math.Abs(firstPersonAge - secPersonAge);

        switch (yearDeference)
        {
            case < 4:
                return 100.0;
            case < 8:
                return 80.0;
            case < 12:
                return 60.0;
            case < 20:
                return 40.0;
            default:
                return 10.0;
        }
    }
}