using Microsoft.AspNetCore.Mvc;
using QueueOptimizer.Models;

namespace QueueOptimizer.ViewModels;

public class ReservationListViewModel : Controller
{
    public List<Reservation> Upcoming { get; set; }
    public List<Reservation> Past { get; set; }
}
