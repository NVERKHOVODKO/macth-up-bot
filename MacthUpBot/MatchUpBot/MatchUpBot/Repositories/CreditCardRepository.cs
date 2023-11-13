using System.Text.RegularExpressions;

namespace MatchUpBot.Repositories;

public class CreditCardRepository
{
     public bool IsValidCreditCardNumber(string creditCardNumber)
    {
        // Удаляем пробелы из номера карты и проверяем, что длина номера от 13 до 19 символов
        creditCardNumber = creditCardNumber.Replace(" ", "");
        if (creditCardNumber.Length < 13 || creditCardNumber.Length > 19)
        {
            return false;
        }

        // Проверяем, что номер карты состоит только из цифр
        long number;
        if (!long.TryParse(creditCardNumber, out number))
        {
            return false;
        }
        // Вставляем пробел после каждой четвертой цифры
        
        // Проверяем, что номер карты проходит проверку Луна
        int sum = 0;
        bool alternate = false;
        for (int i = creditCardNumber.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(creditCardNumber[i].ToString());
            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }
            sum += digit;
            alternate = !alternate;
        }
        return (sum % 10 == 0);
    }
    
    public bool IsValidCardholderName(string name)
    {
        // Проверяем, что имя держателя состоит только из букв и пробелов
        if (!Regex.IsMatch(name, @"^[a-zA-Z ]+$"))
        {
            return false;
        }

        // Проверяем, что длина имени держателя не превышает 26 символов
        if (name.Length > 26)
        {
            return false;
        }

        return true;
    }
    
    public bool IsValidExpirationDate(string date)
    {
        // Проверяем, что дата действия состоит из 4 цифр и разделителя "/"
        if (!Regex.IsMatch(date, @"^\d{2}/\d{2}$"))
        {
            return false;
        }

        // Разбиваем дату на месяц и год
        string[] parts = date.Split('/');
        int month = int.Parse(parts[0]);
        int year = int.Parse(parts[1]);

        // Проверяем, что месяц и год находятся в допустимых пределах
        if (month < 1 || month > 12 || year < DateTime.Now.Year % 100)
        {
            return false;
        }

        return true;
    }
    
    public bool IsValidCVV(string cvv)
    {
        // Проверяем, что CVV состоит из 3 или 4 цифр
        if (!Regex.IsMatch(cvv, @"^\d{3,4}$"))
        {
            return false;
        }

        return true;
    }

}