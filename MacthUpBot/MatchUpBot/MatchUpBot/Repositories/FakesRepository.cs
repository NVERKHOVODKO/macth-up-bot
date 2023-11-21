﻿using System.Text.RegularExpressions;
using Data;
using Entities;
using Microsoft.Extensions.Logging;

namespace MatchUpBot.Repositories;

public class FakesRepository
{
    private static readonly Context _context = new();

    private static readonly ILogger<FakesRepository> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<FakesRepository>();
    
    public static void CreateRandomMaleUsers_0_100()
    {
        var random = new Random();
        var zodiacSigns = new[]
            { "Рак", "Овен", "Телец", "Близнцы", "Рыбы", "Дева", "Лев", "Скорпион", "Стрелец", "Водолей", "Козерог" };
        var names = new[]
        {
            "Дэн", "Егич", "Парень из первой группы", "Данила", "Димооооон",
            "Андрюха", "Тима", "Ильюха", "Адам", "Антоха",
            "Твой господин"
        };
        var descriptions = new[]
        {
            "Привет, я тут, чтобы проверить, насколько ты терпеливая. Готов к взрывам мозга и экспериментам с чувствами!",
            "Сверхразум в поиске своего места в этом мире. Если ты любишь загадки, то я та загадка, которую ты искала.",
            "Обожаю котиков и собачек, а ещё могу заставить тебя смеяться своими гениальными шутками. Приготовься к каскаду уморы!",
            "Я как сложный код, разгадать меня интересно, и если ты справишься, мы создадим лучший проект вместе!",
            "Люблю приключения и экстрим. Если ты готова к неожиданностям и веселью, давай знакомиться!",
            "Обычный парень, который смешивает хаос и порядок. Готов поставить всё на карту в имени любви!",
            "Моя философия: жизнь слишком коротка, чтобы не смеяться над своими шутками. Если у тебя хорошее чувство юмора, мы точно подружимся!",
            "",
            "",
            "",
            "",
            "",
        };

        int j = 0;
        for (var i = 42; i <= 51; i++)
            if (!_context.Users.Any(u => u.TgId == i))
            {
                var user = new UserEntity
                {
                    TgId = i,
                    Name = names[j],
                    Age = random.Next(17, 26),
                    City = "минск",
                    Gender = "М",
                    TgUsername = $"user{i}_telegram",
                    Stage = 1,
                    About = descriptions[random.Next(descriptions.Length)],
                    ZodiacSign = zodiacSigns[random.Next(zodiacSigns.Length)],
                    IsZodiacSignMatters = false,
                    GenderOfInterest = "Ж",
                    LastShowedBlankTgId = 1,
                    IsVip = false
                };
                j++;
                _context.Users.Add(user);
            }

        _context.SaveChanges();
    }
    
    public static void CreateRandomFemaleUsers_0_100()
    {
        var random = new Random();
        var zodiacSigns = new[]
            { "Рак", "Овен", "Телец", "Близнцы", "Рыбы", "Дева", "Лев", "Скорпион", "Стрелец", "Водолей", "Козерог" };
        var names = new[]
        {
            "Анна", "Екатерина", "Мария", "Ольга", "Татьяна",
            "Елена", "Наталья", "Ирина", "Светлана", "Алиса",
            "Виктория", "Евгения", "Дарья", "Ксения", "Милена",
            "Надежда", "Полина", "Виолетта", "Юлия", "Ангелина"
        };
        var descriptions = new[]
        {
            "Привет, я новая здесь и ищу интересных собеседников. Обожаю музыку и кино.",
            "Я студентка, увлекаюсь спортом и мечтаю посетить много стран.",
            "Люблю природу и путешествия. Ищу кого-то, кто разделит мои интересы.",
            "Сразу цветы, а потом посмотрим",
            "Если ты тоже любишь животных, мы найдем общие темы для разговора.",
            "Увлекаюсь искусством и живописью. Готова делиться своими впечатлениями.",
            "Люблю активный образ жизни. Давай вместе заниматься спортом!",
            "Мечтаю научиться играть на гитаре. Ищу учителя и вдохновение.",
            "Обожаю готовить разные блюда. Может, устроим кулинарный поединок?",
            "Интересуюсь наукой и новейшими технологиями. Обсудим актуальные темы?",
            "Мечтаю о кругосветном путешествии. Присоединись к моим приключениям!",
            "Люблю читать книги и обсуждать их с кем-то. Есть любимая книга?",
            "Ищу собеседника, чтобы вместе учиться новому. Готов присоединиться?",
            "Хочу стать лучшей версией себя. Поддержи меня в этом!",
            "Мечтаю о большой семье. Ищу надежного партнера для серьезных отношений.",
            "Путешественник и фотограф. Покажу тебе лучшие места для фотосессии.",
            "Учусь на программиста. Расскажу тебе о мире IT и компьютерах.",
            "Люблю музыку разных жанров. Поделишься своими музыкальными вкусами?",
            "Считаю себя экологически грамотным человеком. Давай обсудим природу и экологию.",
            "Увлекаюсь экстримальными видами спорта. Готов к адреналину?",
            "Люблю искусство и культуру разных стран. Давай обсудим искусство мира.",
            "Ищу интересных собеседников. Готов обсудить любые темы.",
            "Увлекаюсь космосом и астрономией. Поделишься своими знаниями?",
            "Ищу спутника для походов в кино и театр. Какие фильмы ты предпочитаешь?",
            "Люблю животных и всегда готова помочь им. У тебя есть домашние питомцы?",
            "Мечтаю научиться танцевать. Готов показать тебе свои танцевальные движения.",
            "Интересуюсь модой и стилем. Расскажешь о своем стиле одежды?",
            "Увлекаюсь историей и археологией. Давай поговорим о древних цивилизациях.",
            "Ищу спутника для походов на природу. Готов провести время на свежем воздухе?",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
        };

        for (var i = 1; i <= 41; i++)
            if (!_context.Users.Any(u => u.TgId == i))
            {
                var user = new UserEntity
                {
                    TgId = i,
                    Name = names[random.Next(0, names.Length)],
                    Age = random.Next(17, 26),
                    City = "минск",
                    Gender = "Ж",
                    TgUsername = $"user{i}_telegram",
                    Stage = 1,
                    About = descriptions[random.Next(descriptions.Length)],
                    ZodiacSign = zodiacSigns[random.Next(zodiacSigns.Length)],
                    IsZodiacSignMatters = false,
                    GenderOfInterest = "М",
                    LastShowedBlankTgId = 1,
                    IsVip = false
                };

                _context.Users.Add(user);
            }
        
        for (var i = 102; i <= 105; i++)
            if (!_context.Users.Any(u => u.TgId == i))
            {
                var user = new UserEntity
                {
                    TgId = i,
                    Name = names[random.Next(0, names.Length)],
                    Age = random.Next(17, 26),
                    City = "минск",
                    Gender = "Ж",
                    TgUsername = $"user{i}_telegram",
                    Stage = 1,
                    About = descriptions[random.Next(descriptions.Length)],
                    ZodiacSign = zodiacSigns[random.Next(zodiacSigns.Length)],
                    IsZodiacSignMatters = false,
                    GenderOfInterest = "М",
                    LastShowedBlankTgId = 1,
                    IsVip = false
                };

                _context.Users.Add(user);
            }
        
        for (var i = 200; i <= 234; i++)
            if (!_context.Users.Any(u => u.TgId == i))
            {
                var user = new UserEntity
                {
                    TgId = i,
                    Name = names[random.Next(0, names.Length)],
                    Age = random.Next(17, 23),
                    City = "минск",
                    Gender = "Ж",
                    TgUsername = $"user{i}_telegram",
                    Stage = 1,
                    About = descriptions[random.Next(descriptions.Length)],
                    ZodiacSign = zodiacSigns[random.Next(zodiacSigns.Length)],
                    IsZodiacSignMatters = false,
                    GenderOfInterest = "М",
                    LastShowedBlankTgId = 1,
                    IsVip = false
                };

                _context.Users.Add(user);
            }

        _context.SaveChanges();
    }
    
    public void CreateRandomMaleUsers_100_200()
    {
        var random = new Random();
        var zodiacSigns = new[]
        {
            "Рак", "Овен", "Телец", "Близнцы", "Рыбы", "Дева", "Лев", "Скорпион", "Стрелец", "Водолей", "Козерог"
        };
        var names = new[]
        {
            "Иван", "Алексей", "Сергей", "Дмитрий", "Андрей",
            "Павел", "Михаил", "Владимир", "Николай", "Георгий",
            "Константин", "Артем", "Максим", "Игорь", "Олег",
            "Юрий", "Федор", "Виктор", "Геннадий", "Станислав"
        };
        string[] descriptions = new string[]
        {
            "Люблю активный образ жизни и ищу единомышленников для спортивных приключений.",
            "Студент, увлекаюсь программированием и готов поделиться своими знаниями.",
            "Мечтаю стать профессиональным музыкантом. Любишь музыку?",
            "Если у тебя есть домашние питомцы, мы точно найдем общие темы для разговора.",
            "Обожаю путешествия и готов поделиться историями своих приключений.",
            "Учусь в университете и ищу интересные беседы и друзей.",
            "Люблю науку и технологии. Готов обсудить актуальные темы и новейшие открытия.",
            "Активно занимаюсь спортом и приглашаю присоединиться к тренировкам.",
            "Увлекаюсь экологией и природой. Можем поговорить о сохранении окружающей среды.",
            "Жизнерадостный и веселый. Готов создавать позитивные моменты вместе.",
            "Интересуюсь астрономией и мечтаю смотреть звезды вдвоем.",
            "Музыкант и автор песен. Готов исполнить что-то специально для тебя.",
            "Люблю экстремальные виды спорта. Если ты любишь адреналин, напиши мне.",
            "Готов с тобой делиться вдохновением и мечтами.",
            "Природолюб и готов к приключениям на свежем воздухе.",
            "Любитель искусства и культуры. Давай посетим музеи и выставки вместе.",
            "Заинтересован в истории и археологии. Обсудим древние цивилизации?",
            "Фанат кино и театра. Давай вместе посмотрим новинки и спектакли.",
            "Увлекаюсь фотографией. Готов провести фотосессию на природе.",
            "Мечтаю об большой семье. Ищу серьезные отношения и надежного партнера.",
            "Люблю танцевать и готов показать тебе свои танцевальные движения.",
            "Интересуюсь модой и стилем. Расскажешь о своем стиле одежды?",
            "Заинтересован в архитектуре и дизайне. Можем обсудить новые тенденции.",
            "Мечтаю научиться готовить вкусные блюда. Готов соревноваться в кулинарии.",
            "Люблю книги и читаю много. Есть любимая книга?",
            "Интересуюсь футболом и болельщиком. Можем смотреть матчи вместе.",
            "Заинтересован в фотографии и искусстве. Давай обсудим творчество.",
            "Жизнерадостный и всегда в поиске новых приключений.",
            "Мечтаю об большой семье. Ищу серьезные отношения и надежного партнера.",
            "Люблю приключения и экстрима. Путешествия и адреналин - это о нас."
        };

        for (var i = 100; i <= 200; i++)
        {
            if (!_context.Users.Any(u => u.TgId == i))
            {
                var user = new UserEntity
                {
                    TgId = i,
                    Name = names[i - 21],
                    Age = random.Next(17, 26),
                    City = "минск",
                    Gender = "М",
                    TgUsername = $"user{i}_telegram",
                    Stage = 0,
                    About = descriptions[random.Next(descriptions.Length)],
                    ZodiacSign = zodiacSigns[random.Next(zodiacSigns.Length)],
                    IsZodiacSignMatters = random.Next(2) == 0,
                    GenderOfInterest = "Ж",
                    LastShowedBlankTgId = 0,
                    IsVip = false
                };
                _context.Users.Add(user);
            }
        }
        _context.SaveChanges();
    }
    
    public static void CreateTeachers_400_500()
    {
        string[] profiles =
        {
            "401 - Никита, 23. Я досконально проверю всю твою пояснительную и заставалю распечатать ее 123 раза. Но это все ради тебя...",
            "402 - Никита, 25. С виду может быть я и молчу всегда, но ты даже не представляешь, что происходит у меня в голове и какие планы на тебя. Обожаю наблюдать за людьми",
            "403 - Виктор, 51. Люблю когда все в жизни идет по моим планам. Первый не пишу. А если в своем сообщении ты не поприветствуешь меня как надо или позволишь себе вульгарность, то сразу в бан",
            "404 - Антоша, 27. Обожаю Италию, особенно Венецию. Расскажу и покажу все касаемо ООП. Со мной не соскучишься. Люблю Козерогов",
            "405 - Коля, 69. Я расскажу тебе все о строении компьютера и процессора, покажу как перевести чилсо из системы счисления в другую, но у меня будет только одна просьба: говори громче!",
            "406 - Володя, 55. Люблю кататься на лошадях и кормить кроликов морковкой. Часто болею и лежу в больницах, поэтому будь готова носить мне апельсины",
            "407 - Влаdick, 25. Обычный парень с блока, люблю слушать русский реп и есть чипсы с пивом у подъезда. В ближайший год буду не в сети, но ты меня дождись -- не пожалеешь)",
            "408 - Марина, 35. Ворк эверивере и сразу, Вери лайк нэйм Ярик. Можешь меня ни о чем не просить. бекос что ай все равно сделаю хау ай вонт.",
            "409 - Алексей, 28. Люблю поразмышлять над философией, особенно нравится идея и посыл фильма 'Зеленый слоник'. Не думай, что все философы слабые дрыщи. Если понадобиться и могу взять всю твою семью и сделать становую.",
            "410 - Ирина, 46. В прошлом хороший химик, а сейчас -- бизнес-аналитик. Можешь мне ничего не рассказывать и не обещать, ведь мне все равно не понравится, и тебе придется все переделывать. А еще я очень занятой человек, поэтому пиши мне только по делу",
            "411 - Оля, 40. Нравится, когда человек знает разницу между дифференциалом и производной. Не люблю сильно заморачиваться, люблю кайфовать, смотреть видосики на ютубе и слушать реп",
            "412 - Вика, 24. Обожаю, когда человек понимает то, что говорит, и когда с ним есть о чем поговорить. Безумно фанатею от трансформеров!",
            "413 - Настя, 24. Я спокойный, добрый человек. Не люблю конфликты и тех, кто их создает. Ищу любовб на одну ночь. Лошкам без мака не писать!",
            "414 - Витя, 60. Кандидат технических наук, доцент, дачник. Издал столько книг и публикаций, что сбился после 5 посаженных грядок морковки. Не думай, что я простачок, узнай меня получше",
            "415 - Жанна, 41. Не люблю, когда опаздывают. Я не знаю что еще написать. Люблю собак, кино и селедку под шубой",
            "416 - Ирина, 65. Люблю вежливых, молчаливых людей, которые не пишут на меня докладных. Можешь говорить мне все, что хочешь, но я все равно сделаею по-своему. Иногда очень часто промахиваюсь по кнопкам, но я специально",
            "417 - Даша, 35. Обожаю своих дочек, обязательно расскажу о них при первой встрече. Но я просто ненавижу chatgpt. Люблю мороженое",
            "418 - Виктор, 43. Я люблю власть и деньги. Не верю людям, которые учатся в автошколе, а так в целом позитивный, добрый парень. Делаю мой факультет лучше каждый день, за счет стипендий, который заюирем у студентов",
            "419 - Инна, 32. Люблю всех студентов (есть конечно пару исключений). Помогаю делать факультет компьютерного проектирования лучше. Нравится лишать стипендии и делать выговоры. Просто обожаю BMW, особоенно 850CSi",
            "420 - Танюша, 65. Обожаю программирование и первашей. Особенно нравится посылать их нахер. Люблю помладше и тех, кто не пользуется библиотеками. Еще люблю пиво пить и о жизни говорить",
            "421 - Светочка, 45. Люблю, отправлять молодых мальчиков на пересдачи, но еще больше люблю смотреть на их жалкие попытки сдать мой предмет.",
            "422 - Катя, 43. Нормальная женщина, но люблю пожестче",
            "423 - Кирилл, 24. С первого взгляда можно подумать, что слабая личность, но позволь тебя переубедить: я с легкостью возьму ВЕРХ над тобой",
            "424 - Анатолий, 67. Всемиуважаемый кандидат физико-математических наук. Могу заставить любого миллионера задолжать мне, но для счастья не хватает молодой програмисстки, которая поможет мне решить одну личную задачу)",
            "425 - Алина, 24. Люблю Верховодко Никиту, танцю тверк и могу объяснить наследование и с абстракцией",
            "426 - Паша, 23. Актер, программист, рок-музыкант, стендапер и многое другое;) Со мной не соскучишься. Нравятся брюнетки"

        };
        string[] genders = { "М", "М", "М", "М", "М", "М", "М", "Ж", "М", "Ж", "Ж", "Ж", "Ж", "М", "Ж", "Ж", "Ж", "М", "Ж", "Ж", "Ж", "Ж", "М", "Ж", "Ж", "Ж", "Ж", "М", "Ж", "М" };
        var random = new Random();
        var zodiacSigns = new[]
        {
            "Рак", "Овен", "Телец", "Близнцы", "Рыбы", "Дева", "Лев", "Скорпион", "Стрелец", "Водолей", "Козерог"
        };
        
        List<UserEntity> users = new List<UserEntity>();
        
        
        int i = 0;
        int tgId = 0, age = 0;
        string name = "", description = "";
        foreach (var profile in profiles)
        {
            Regex regex = new Regex(@"(\d+) - (.+), (\d+). (.+)");
            Match match = regex.Match(profile);

            if (match.Success)
            {
                tgId = int.Parse(match.Groups[1].Value);
                name = match.Groups[2].Value;
                age = int.Parse(match.Groups[3].Value);
                description = match.Groups[4].Value;

            }
            else
            {
                Console.WriteLine("Не удалось извлечь информацию из строки.");
            }
            

            UserEntity user = new UserEntity
            {
                TgId = tgId,
                Name = name,
                Age = age,
                City = "минск", // Значение города, которое вам нужно
                Gender = genders[i], // Значение пола, которое вам нужно
                TgUsername = $"user{tgId}_telegram", // Измените по вашему усмотрению
                Stage = 22,
                About = description,
                ZodiacSign = zodiacSigns[random.Next(zodiacSigns.Length)], // Например, "значение" нужно заменить на реальное значение
                IsZodiacSignMatters = false, // Значение по умолчанию, замените по необходимости
                GenderOfInterest = (genders[i] == "М") ? "Ж" : "М", // Значение по умолчанию, замените по необходимости
                LastShowedBlankTgId = 1, // Значение по умолчанию, замените по необходимости
                IsVip = false // Значение по умолчанию, замените по необходимости
            };
            i++;
            _context.Users.Add(user);
        }
        Console.WriteLine("Teachers are created");
        _context.SaveChanges();
    }
}