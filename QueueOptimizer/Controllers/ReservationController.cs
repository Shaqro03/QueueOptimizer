using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueueOptimizer.Data;
using QueueOptimizer.Models;
using QueueOptimizer.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QueueOptimizer.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;

        public ReservationController(AppDbContext context)
        {
            _context = context;
        }

        // cuyc tal bolor hertery
        public async Task<IActionResult> MyReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var now = DateTime.Now;

            var allReservations = await _context.Reservations
                .Include(r => r.Table)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            var futureReservations = allReservations
                .Where(r => r.ReservationDate.Add(r.StartTime) >= now)
                .OrderBy(r => r.ReservationDate).ThenBy(r => r.StartTime)
                .ToList();

            var pastReservations = allReservations
                .Where(r => r.ReservationDate.Add(r.EndTime) < now)
                .OrderByDescending(r => r.ReservationDate).ThenByDescending(r => r.EndTime)
                .ToList();

            var model = new ReservationListViewModel
            {
                Upcoming = futureReservations,
                Past = pastReservations
            };

            return View(model);
        }

        // chexarkel
        [HttpPost]
        public async Task<IActionResult> CancelReservation(CancelRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservation = await _context.Reservations
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.Id == request.Id && r.UserId == userId);

            if (reservation == null)
            {
                TempData["Error"] = "Reservation not found or unauthorized.";
                return RedirectToAction("MyReservations");
            }

            if (reservation.Table != null)
            {
                reservation.Table.IsOccupied = false;
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Reservation canceled successfully.";
            return RedirectToAction("MyReservations");
        }

        public class CancelRequest
        {
            public int Id { get; set; }
        }

        // nor hert
        [HttpPost]
        public async Task<IActionResult> Reserve([FromBody] ReservationRequest request)
        {
            if (request == null || request.TableId <= 0 ||
                request.StartTime == TimeSpan.Zero || request.EndTime <= request.StartTime || request.ReservationDate == default)
            {
                return BadRequest(new { success = false, message = "Invalid data." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // stugel zbxvac e te che
            var exists = await _context.Reservations
                .AnyAsync(r =>
                    r.TableId == request.TableId &&
                    r.ReservationDate == request.ReservationDate &&
                    (
                        (r.StartTime < request.EndTime && r.EndTime > request.StartTime)
                    )
                );

            if (exists)
            {
                return Json(new { success = false, message = "Table is already reserved for this time range." });
            }

            var reservation = new Reservation
            {
                TableId = request.TableId,
                UserId = userId,
                ReservationDate = request.ReservationDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };

            _context.Reservations.Add(reservation);

            var table = await _context.Tables.FindAsync(request.TableId);
            if (table != null)
            {
                table.IsOccupied = true;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        public class ReservationRequest
        {
            public int TableId { get; set; }
            public DateTime ReservationDate { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
        }
    }
}
