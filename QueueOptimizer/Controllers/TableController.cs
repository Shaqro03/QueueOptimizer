using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using QueueOptimizer.Data;
using QueueOptimizer.ViewModels;
using QueueOptimizer.Models;

namespace QueueOptimizer.Controllers;

[Authorize]
public class TableController : Controller
{
    private readonly AppDbContext _context;

    public TableController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var allTables = _context.Tables.ToList();

        var model = new ReservationFilterViewModel
        {
            AllTables = allTables,
            BusyTables = new List<Table>(),           
            AvailableTables = new List<Table>(),      
            Date = DateTime.Today,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            PeopleCount = 2
        };

        return View(model); 
    }


    [HttpGet]
    public IActionResult Filter()
    {
        return View(new ReservationFilterViewModel
        {
            Date = DateTime.Today,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            PeopleCount = 2
        });
    }



    
    [HttpPost]
    public IActionResult Filter(ReservationFilterViewModel model)
    {
        if (model.StartTime >= model.EndTime)
        {
            ModelState.AddModelError("", "Start time must be before end time.");
            model.AllTables = _context.Tables.ToList();
            return View("Index", model); 
        }

        var reservationStart = model.Date.Date + model.StartTime;
        var reservationEnd = model.Date.Date + model.EndTime;

        reservationStart = reservationStart.AddMinutes(-15);
        reservationEnd = reservationEnd.AddMinutes(15);

        
        var reservations = _context.Reservations
            .Where(r => r.ReservationDate == model.Date.Date)
            .ToList(); 

        
        var busyTableIds = reservations
            .Where(r =>
                r.ReservationDate.Add(r.StartTime) < reservationEnd &&
                r.ReservationDate.Add(r.EndTime) > reservationStart
            )
            .Select(r => r.TableId)
            .Distinct()
            .ToList();


        var allowedSeats = GetAllowedSeats(model.PeopleCount);

        model.AllTables = _context.Tables.ToList();
        model.BusyTables = _context.Tables.Where(t => busyTableIds.Contains(t.Id)).ToList();
        model.AvailableTables = model.AllTables
            .Where(t => !busyTableIds.Contains(t.Id) && allowedSeats.Contains(t.Seats))
            .ToList();

        return View("Index", model); 
    }

    private List<int> GetAllowedSeats(int peopleCount)
    {
        return peopleCount switch
        {
            1 => new List<int> { 1, 2, 3, 6 },
            2 => new List<int> { 2, 3, 6 },
            3 => new List<int> { 3, 6 },
            4 or 5 or 6 => new List<int> { 6 },
            _ => new List<int>() 
        };
    }
}

