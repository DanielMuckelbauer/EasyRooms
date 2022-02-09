﻿namespace EasyRooms.Model.Rows.Models;

public record Row(string StartTime, string Duration, string TherapyShort, string TherapyLong, string Patient, string Therapist)
{
    public string EndTime => TimeOnly.Parse(StartTime).AddMinutes(double.Parse(Duration)).ToString();
    public TimeOnly StartTimeAsTimeOnly => TimeOnly.Parse(StartTime);
    public TimeOnly EndTimeAsTimeOnly => TimeOnly.Parse(EndTime);
    public TimeSpan StartTimeAsTimeSpan => TimeSpan.Parse(StartTime);
    public TimeSpan EndTimeAsTimeSpan => TimeSpan.Parse(EndTime);
    public TimeSpan DurationAsTimeSpan => TimeSpan.FromMinutes(double.Parse(Duration));
}