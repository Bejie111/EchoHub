using EchoHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace EchoHub.ViewModels
{
    public class UserViewModel
    {
        public User User { get; set; }
        public string Address { get; set; }
    }
}
