using System.ComponentModel.DataAnnotations;
using QueueOptimizer.Models;
using Microsoft.AspNetCore.Mvc;

namespace QueueOptimizer.ViewModels;


public class ReservationFilterViewModel
{
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int PeopleCount { get; set; }

    public List<Table> AllTables { get; set; } = new();
    public List<Table> BusyTables { get; set; } = new();
    public List<Table> AvailableTables { get; set; } = new();
}
