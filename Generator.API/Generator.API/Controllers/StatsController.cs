﻿using Generator.Infrastructure; // Ваш namespace с ApplicationDbContext
using Generator.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Generator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Ограничение доступа только для администраторов
    public class StatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Глобальная статистика для админа:
        /// 1) Общее количество "completed" вызовов за текущий месяц/год.
        /// 2) Топ-10 пользователей по количеству "completed" вызовов с разбивкой по категориям.
        /// </summary>
        [HttpGet("admin")]
        public async Task<IActionResult> GetGlobalStats()
        {
            var now = DateTime.UtcNow;
            var currentYear = now.Year;
            var currentMonth = now.Month;

            // 1. Получаем топ-10 пользователей по количеству "completed" вызовов за всё время
            var topUsersRaw = await _context.Calls
                .Where(c => c.status == "completed")
                .GroupBy(c => c.user_id)
                .Select(g => new
                {
                    UserId = g.Key,
                    CompletedCount = g.Count()
                })
                .OrderByDescending(x => x.CompletedCount)
                .Take(10)
                .ToListAsync();

            var userIds = topUsersRaw.Select(x => x.UserId).ToList();

            // Получаем имена пользователей
            var usersDict = await _context.Users
                .Where(u => userIds.Contains(u.user_id))
                .ToDictionaryAsync(u => u.user_id, u => u.username);

            // 2. Получаем все "completed" вызовы для топ-10 пользователей
            var topUsersCompletedCalls = await _context.Calls
                .Where(c => c.status == "completed" && userIds.Contains(c.user_id))
                .ToListAsync(); // Извлекаем данные в память

            // 3. Группируем по (user_id, call_name) и подсчитываем количества
            var grouped = topUsersCompletedCalls
                .GroupBy(c => new { c.user_id, c.call_name })
                .Select(g => new
                {
                    UserId = g.Key.user_id,
                    Category = g.Key.call_name,
                    MonthlyCompleted = g.Count(c =>
                        DateTime.TryParse(c.call_date, out DateTime callDate) &&
                        callDate.Year == currentYear &&
                        callDate.Month == currentMonth),
                    YearlyCompleted = g.Count(c =>
                        DateTime.TryParse(c.call_date, out DateTime callDate) &&
                        callDate.Year == currentYear)
                })
                .ToList(); // Обрабатываем в памяти

            // 4. Формируем итоговый список топ-10 пользователей с категориями
            var topUsersFinal = topUsersRaw.Select(u => new
            {
                username = usersDict.ContainsKey(u.UserId) ? usersDict[u.UserId] : $"User#{u.UserId}",
                completedCalls = u.CompletedCount,
                categories = grouped
                    .Where(c => c.UserId == u.UserId)
                    .Select(c => new
                    {
                        category = c.Category,
                        completedCalls = c.YearlyCompleted // Используем YearlyCompleted как общее количество за год
                    })
                    .ToList()
            }).ToList();

            // 5. Общие показатели за месяц и год
            int totalMonthlyCompleted = topUsersCompletedCalls.Count(c =>
                DateTime.TryParse(c.call_date, out DateTime callDate) &&
                callDate.Year == currentYear &&
                callDate.Month == currentMonth);

            int totalYearlyCompleted = topUsersCompletedCalls.Count(c =>
                DateTime.TryParse(c.call_date, out DateTime callDate) &&
                callDate.Year == currentYear);

            // 6. Формируем итоговый объект
            var result = new
            {
                totalMonthlyCompleted,
                totalYearlyCompleted,
                topUsers = topUsersFinal
            };

            return Ok(result);
        }

        /// <summary>
        /// Статистика для конкретного пользователя (по username):
        /// 1) Количество "completed" вызовов за текущий месяц.
        /// 2) Количество "completed" вызовов за текущий год.
        /// </summary>
        [HttpGet("user")]
        [AllowAnonymous] // Измените по необходимости
        public async Task<IActionResult> GetUserStats([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Имя пользователя не указано.");
            }

            // Находим пользователя по username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.username == username);
            if (user == null)
            {
                return NotFound($"Пользователь {username} не найден.");
            }

            var now = DateTime.UtcNow;
            var currentYear = now.Year;
            var currentMonth = now.Month;

            // Получаем все "completed" вызовы пользователя
            var userCompletedCalls = await _context.Calls
                .Where(c => c.user_id == user.user_id && c.status == "completed")
                .ToListAsync(); // Извлекаем данные в память

            // Подсчёт выполненных вызовов за месяц и за год
            int monthlyCompleted = userCompletedCalls.Count(c =>
                DateTime.TryParse(c.call_date, out DateTime callDate) &&
                callDate.Year == currentYear &&
                callDate.Month == currentMonth);

            int yearlyCompleted = userCompletedCalls.Count(c =>
                DateTime.TryParse(c.call_date, out DateTime callDate) &&
                callDate.Year == currentYear);

            // Подсчёт выполненных вызовов по категориям за месяц и за год
            var categoriesStats = userCompletedCalls
                .Where(c => DateTime.TryParse(c.call_date, out DateTime callDate) &&
                            callDate.Year == currentYear)
                .GroupBy(c => c.call_name)
                .Select(g => new
                {
                    category = g.Key,
                    monthlyCompleted = g.Count(c => DateTime.TryParse(c.call_date, out DateTime callDate) && callDate.Month == currentMonth),
                    yearlyCompleted = g.Count()
                })
                .ToList();

            var result = new
            {
                username = user.username,
                monthlyCompleted,
                yearlyCompleted,
                categoriesStats
            };

            return Ok(result);
        }
    }
}