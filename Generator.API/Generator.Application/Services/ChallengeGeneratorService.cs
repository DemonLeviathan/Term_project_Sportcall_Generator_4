using Generator.Application.Interfaces;
using Generator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Application.Services
{
    public class ChallengeGeneratorService : IChallengeGeneratorService
    {
        public Calls GenerateDailyCall(Users user, int friendId)
        {
            (int age, float bmi) = GetAgeAndBmi(user);

            // Пример: ежедневный вызов - пробежать 2 км
            return new Calls
            {
                call_name = "Daily challenge: пробежать 2 км",
                friend_id = friendId,
                call_date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                status = "pending"
            };
        }

        public Calls GenerateWeeklyCall(Users user, int friendId)
        {
            (int age, float bmi) = GetAgeAndBmi(user);

            // Пример: еженедельный вызов - пробежать суммарно 10 км за неделю
            return new Calls
            {
                call_name = "Еженедельный вызов: 10 км за неделю",
                friend_id = friendId,
                call_date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                status = "pending"
            };
        }

        public Calls GenerateMonthlyCall(Users user, int friendId)
        {
            (int age, float bmi) = GetAgeAndBmi(user);

            // Пример: ежемесячный вызов - пройти 300 000 шагов
            return new Calls
            {
                call_name = "Ежемесячный вызов: 300 000 шагов",
                friend_id = friendId,
                call_date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                status = "pending"
            };
        }

        /// <summary>
        /// Вспомогательный метод для вычисления возраста и ИМТ пользователя.
        /// </summary>
        private (int age, float bmi) GetAgeAndBmi(Users user)
        {
            // Парсим дату рождения пользователя.
            DateTime birthDate;
            if (!DateTime.TryParse(user.birthday, out birthDate))
            {
                // Если не удалось распарсить, можно задать дефолт или выбросить ошибку.
                // Для примера используем текущее время - пользователь будет 0 лет.
                birthDate = DateTime.UtcNow;
            }

            int age = CalculateAge(birthDate, DateTime.UtcNow);

            // Находим последнюю запись UserData (например, самую свежую по дате)
            var latestUserData = user.UserData?.OrderByDescending(ud => ud.date_info).FirstOrDefault();
            if (latestUserData == null)
            {
                // Если нет данных, предположим некие значения по умолчанию
                // или можно выдать задание без персонализации.
                return (age, 0f);
            }

            float weight = latestUserData.weight;
            float height = latestUserData.height; // в метрах или сантиметрах? Предполагаем, что в метрах.

            float bmi = CalculateBMI(weight, height);
            return (age, bmi);
        }

        private int CalculateAge(DateTime birthDate, DateTime now)
        {
            int age = now.Year - birthDate.Year;
            if (now < birthDate.AddYears(age))
                age--;
            return age;
        }

        private float CalculateBMI(float weight, float height)
        {
            // Если height хранится в метрах, ок.
            // Если в сантиметрах, надо разделить на 100.
            // Проверим логически: UserData.height судя по всему float,
            // предположим, что хранится в метрах (например, 1.75).
            return weight / (height * height);
        }
    }
}
