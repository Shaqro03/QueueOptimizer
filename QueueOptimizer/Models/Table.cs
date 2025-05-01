using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace QueueOptimizer.Models;

public class Table
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int Seats { get; set; } 

    [Required]
    public int PositionX { get; set; } 

    [Required]
    public int PositionY { get; set; } 

    public bool IsOccupied { get; set; } = false; 
}

