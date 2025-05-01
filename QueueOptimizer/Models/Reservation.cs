using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueOptimizer.Models;

public class Reservation
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int TableId { get; set; }

    [ForeignKey("TableId")]
    public Table Table { get; set; }

    [Required]
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public Users User { get; set; }

    [Required]
    public DateTime ReservationDate { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }
}
