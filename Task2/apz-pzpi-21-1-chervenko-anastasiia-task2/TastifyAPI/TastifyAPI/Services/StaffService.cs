﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TastifyAPI.DTOs.Features_DTOs;
using TastifyAPI.Entities;

namespace TastifyAPI.Services
{
    public class StaffService
    {
        private readonly IMongoCollection<Staff> _staffCollection;
        private readonly IMongoCollection<Schedule> _scheduleCollection;
        private readonly ILogger<StaffService> _logger;


        public StaffService(IMongoDatabase database, IMongoCollection<Schedule> scheduleCollection, ILogger<StaffService> logger)
        {
            _staffCollection = database.GetCollection<Staff>("Staff");
            _scheduleCollection = database.GetCollection<Schedule>("Schedule");
            _logger = logger;
        }

        public async Task<List<Staff>> GetAsync() =>
            await _staffCollection.Find(_ => true).ToListAsync();

        public async Task<Staff> GetByIdAsync(string id) =>
            await _staffCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Staff staff) =>
            await _staffCollection.InsertOneAsync(staff);

        public async Task UpdateAsync(string id, Staff updatedStaff) =>
            await _staffCollection.ReplaceOneAsync(x => x.Id == id, updatedStaff);

        public async Task RemoveAsync(string id) =>
            await _staffCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<bool> AnyAsync(Expression<Func<Staff, bool>> filter) =>
            await _staffCollection.Find(filter).AnyAsync();        

        public async Task<Staff> GetByLoginAsync(string login) =>
            await _staffCollection.Find(x => x.Login == login).FirstOrDefaultAsync();

        /*public async Task<List<StaffReportDto>> GetWeeklyWorkingHoursAsync(DateTime startDate, DateTime endDate)
        {
            var result = new List<StaffReportDto>();

            var staffList = await _staffCollection.Find(_ => true).ToListAsync();

            foreach (var staff in staffList)
            {
                var staffReport = new StaffReportDto
                {

                    Name = staff.Name,
                    TotalWorkingHours = 0
                };

                var scheduleList = await _scheduleCollection.Find(x => x.StaffId == staff.Id && x.StartDateTime >= startDate && x.FinishDateTime <= endDate).ToListAsync();

                foreach (var schedule in scheduleList)
                {
                    if (schedule.FinishDateTime.HasValue && schedule.StartDateTime.HasValue)
                    {
                        var hours = (schedule.FinishDateTime.Value - schedule.StartDateTime.Value).TotalHours;
                        staffReport.TotalWorkingHours += hours;
                    }
                }

                result.Add(staffReport);
            }

            return result;
        }*/


        /*
        public async Task<List<StaffReportDto>> GetWeeklyWorkingHoursAsync(DateTime date)
        {
            var firstDayOfWeek = date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
            var lastDayOfWeek = firstDayOfWeek.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            var result = new List<StaffReportDto>();

            var staffList = await _staffCollection.Find(_ => true).ToListAsync();

            foreach (var staff in staffList)
            {
                var staffReport = new StaffReportDto
                {
                    Name = staff.Name,
                    TotalWorkingHours = 0
                };

                var scheduleList = await _scheduleCollection.Find(x => x.StaffId == staff.Id && x.StartDateTime >= firstDayOfWeek && x.FinishDateTime <= lastDayOfWeek).ToListAsync();

                foreach (var schedule in scheduleList)
                {
                    if (schedule.FinishDateTime.HasValue && schedule.StartDateTime.HasValue)
                    {
                        var hours = (schedule.FinishDateTime.Value - schedule.StartDateTime.Value).TotalHours;
                        staffReport.TotalWorkingHours += hours;
                    }
                }

                result.Add(staffReport);
            }

            return result;
        }*/


        //логирование

        /*
         public async Task<List<StaffReportDto>> GetWeeklyWorkingHoursAsync(DateTime date)
         {
             var firstDayOfWeek = date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
             var lastDayOfWeek = firstDayOfWeek.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

             _logger.LogInformation($"Calculating weekly working hours from {firstDayOfWeek} to {lastDayOfWeek}");

             var result = new List<StaffReportDto>();

             var staffList = await _staffCollection.Find(_ => true).ToListAsync();

             _logger.LogInformation($"Found {staffList.Count} staff members");

             foreach (var staff in staffList)
             {
                 var staffReport = new StaffReportDto
                 {
                     Name = staff.Name,
                     TotalWorkingHours = 0
                 };

                 _logger.LogInformation($"Calculating working hours for {staffReport.Name}");

                 var scheduleList = await _scheduleCollection.Find(x => x.StaffId == staff.Id && x.StartDateTime >= firstDayOfWeek && x.FinishDateTime <= lastDayOfWeek).ToListAsync();

                 _logger.LogInformation($"Found {scheduleList.Count} schedules for {staffReport.Name}");

                 foreach (var schedule in scheduleList)
                 {
                     if (schedule.FinishDateTime.HasValue && schedule.StartDateTime.HasValue)
                     {
                         var hours = (schedule.FinishDateTime.Value - schedule.StartDateTime.Value).TotalHours;
                         staffReport.TotalWorkingHours += hours;
                     }
                 }

                 _logger.LogInformation($"Total working hours for {staffReport.Name}: {staffReport.TotalWorkingHours}");

                 result.Add(staffReport);
             }

             return result;
         }*/


        /*

        public async Task<List<StaffReportDto>> GetWeeklyWorkingHoursAsync(DateTime date)
        {
            var firstDayOfWeek = date.Date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
            var lastDayOfWeek = firstDayOfWeek.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            _logger.LogInformation($"Calculating weekly working hours from {firstDayOfWeek:dd.MM.yyyy HH:mm:ss} to {lastDayOfWeek:dd.MM.yyyy HH:mm:ss}");

            var result = new List<StaffReportDto>();

            var staffList = await _staffCollection.Find(_ => true).ToListAsync();

            _logger.LogInformation($"Found {staffList.Count} staff members");

            foreach (var staff in staffList)
            {
                var staffReport = new StaffReportDto
                {
                    Name = staff.Name,
                    TotalWorkingHours = 0
                };

                _logger.LogInformation($"Calculating working hours for {staffReport.Name}");

                var scheduleList = await _scheduleCollection.Find(x => x.StaffId == staff.Id && x.StartDateTime >= firstDayOfWeek && x.FinishDateTime <= lastDayOfWeek).ToListAsync();

                _logger.LogInformation($"Found {scheduleList.Count} schedules for {staffReport.Name}");

                foreach (var schedule in scheduleList)
                {
                    if (schedule.FinishDateTime.HasValue && schedule.StartDateTime.HasValue)
                    {
                        var hours = (schedule.FinishDateTime.Value - schedule.StartDateTime.Value).TotalHours;
                        staffReport.TotalWorkingHours += hours;
                    }
                }

                _logger.LogInformation($"Total working hours for {staffReport.Name}: {staffReport.TotalWorkingHours}");

                result.Add(staffReport);
            }

            return result;
        }*/


        public async Task<List<StaffReportDto>> GetWeeklyWorkingHoursAsync(DateTime startDate)
        {
            //var firstDayOfWeek = date.Date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
            var lastDay = startDate.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            _logger.LogInformation($"Calculating weekly working hours from {startDate:dd.MM.yyyy HH:mm:ss} to {lastDay:dd.MM.yyyy HH:mm:ss}");

            var result = new List<StaffReportDto>();

            var staffList = await _staffCollection.Find(_ => true).ToListAsync();

            _logger.LogInformation($"Found {staffList.Count} staff members");

            foreach (var staff in staffList)
            {
                var staffReport = new StaffReportDto
                {
                    Name = staff.Name,
                    TotalWorkingHours = 0
                };

                _logger.LogInformation($"Calculating working hours for {staffReport.Name}");

            //    var scheduleList = await _scheduleCollection.Find(x => x.StaffId == staff.Id && x.StartDateTime >= startDate && x.FinishDateTime <= lastDay).ToListAsync();
                var scheduleList = await _scheduleCollection.Find(x => x.StaffId == staff.Id).ToListAsync();

                _logger.LogInformation($"Found {scheduleList.Count} schedules for {staffReport.Name}");

                foreach (var schedule in scheduleList)
                {
                    if (schedule.FinishDateTime.HasValue && schedule.StartDateTime.HasValue)
                    {
                        var hours = (schedule.FinishDateTime.Value - schedule.StartDateTime.Value).TotalHours;
                        staffReport.TotalWorkingHours += hours;
                    }
                }

                _logger.LogInformation($"Total working hours for {staffReport.Name}: {staffReport.TotalWorkingHours}");

                result.Add(staffReport);
            }

            return result;
        }





    }
}
