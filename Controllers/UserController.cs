using System;
using System.Threading.Tasks;
using BirthdayNotifier.Data;
using BirthdayNotifier.models;
using BirthdayNotifier.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddUser([FromBody] UserCreateDto dto)
    {
        if (dto == null)
            return BadRequest("User data is required");

        var user = new Users
        {
            Name = dto.Name,
            PhoneNumber = dto.PhoneNumber,
            DOB = dto.DOB?.ToUniversalTime(),
            Email = dto.Email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User added successfully", user });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }
}
