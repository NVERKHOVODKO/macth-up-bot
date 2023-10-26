namespace EntityFrameworkLesson.Utils;

public static class MatchCalculator
{

    private static readonly double FIRST_MATCH_MULTIPLIER = 0.4; 
    private static readonly double SECOND_MATCH_MULTIPLIER = 0.4; 
    private static readonly double THIRD_MATCH_MULTIPLIER = 0.2;

    private static readonly int NORMAL_NUMBER_OF_COMMON_INTERESTS = 1;
    private static readonly int GOOD_NUMBER_OF_COMMON_INTERESTS = 2;
    private static readonly int PERFECT_NUMBER_OF_COMMON_INTERESTS = 4;
    
    public static double CalculateMatch(int firstPersonAge, int secPersonAge,
                                        string firstPersonZodiacSign,
                                        string secPersonZodiacSign,
                                        List<string> firstPersonInterests,
                                        List<string> secPersonInterests,bool isZodiacSignImportant)
    {
        if (isZodiacSignImportant)
            return CalculateMatchByAge(firstPersonAge,secPersonAge) * FIRST_MATCH_MULTIPLIER +
                   CalculateMatchByZodiac(firstPersonZodiacSign,secPersonZodiacSign) * SECOND_MATCH_MULTIPLIER + 
                   CalculateMatchByInterests(firstPersonInterests,secPersonInterests) * THIRD_MATCH_MULTIPLIER;
        
        return CalculateMatchByAge(firstPersonAge,secPersonAge) * FIRST_MATCH_MULTIPLIER +
               CalculateMatchByZodiac(firstPersonZodiacSign,secPersonZodiacSign) * THIRD_MATCH_MULTIPLIER + 
               CalculateMatchByInterests(firstPersonInterests,secPersonInterests) * SECOND_MATCH_MULTIPLIER;
    }
 

    private static double CalculateMatchByInterests( List<string> firstPersonInterests,
        List<string> secPersonInterests )
    {
        // Получаем количество совпадений интересов 
        int matchCount = firstPersonInterests.Intersect(secPersonInterests).Count();

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
        // Создаем словарь с элементами и качествами каждого знака зодиака
        Dictionary<string, (string Element, string Quality)> zodiacCharacteristics = new Dictionary<string, (string, string)>
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

        // Проверяем, совпадают ли элементы знаков
        string elementFirst = zodiacCharacteristics[firstZodiac].Element;
        string elementSecond = zodiacCharacteristics[secondZodiac].Element;

        if (elementFirst == elementSecond)
        {
            // Если элементы совпадают, добавляем баллы
            double compatibilityScore = 70.0;

            // Проверяем, совпадают ли качества знаков
            string qualityFirst = zodiacCharacteristics[firstZodiac].Quality;
            string qualitySecond = zodiacCharacteristics[secondZodiac].Quality;

            if (qualityFirst == qualitySecond)
            {
                // Если и качества совпадают, увеличиваем баллы
                compatibilityScore += 30.0;
            }

            return compatibilityScore;
        }

        // Если элементы не совпадают, совместимость равна 0
        return 0.0;
    }
    
    private static double CalculateMatchByAge(int firstPersonAge, int secPersonAge)
    {
        // Получаем разницу в возрасте 
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