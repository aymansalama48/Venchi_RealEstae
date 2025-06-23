using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TestDash.Data;
using TestDash.Models;

namespace TestDash.Controllers
{
  
    public class HomeController : Controller
    {
        //  private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            // العملاء المضافين اليوم فقط
            var todayClients = await _context.Clients
                .Where(c => c.CreatedAt.Date == today)
                .Include(c => c.AddedBy)
                .Include(c => c.Project)
                .ToListAsync();

            // باقي الإحصائيات
            ViewBag.TotalClients = await _context.Clients.CountAsync();
            ViewBag.NewClientsToday = todayClients.Count;
            ViewBag.FollowUpClients = await _context.Clients.CountAsync(c => c.Status == "Follow Up");
            ViewBag.Meeting = await _context.Clients.CountAsync(c => c.Status == "Meeting");
            ViewBag.LowBudget = await _context.Clients.CountAsync(c => c.Status == "Low Budget");
            ViewBag.DoneDeal = await _context.Clients.CountAsync(c => c.Status == "Done Deal");
            ViewBag.NoAnsewar = await _context.Clients.CountAsync(c => c.Status == "No Ansewar");
            ViewBag.ProjectsCount = await _context.Projects.CountAsync();
            return View(todayClients); // ← بنبعت العملاء المضافين النهارده فقط
        }

        [Authorize(Roles = "Sales")]
        public async Task<IActionResult> Home_Sales()
        {
            var today = DateTime.Today;

            // اسم المستخدم الحالي
            var userName = User.Identity.Name;

            // نحصل على المستخدم الحالي من قاعدة البيانات
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.FullName == userName);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            // العملاء اللي تم إسنادهم للموظف النهارده
            var assignedTodayClients = await _context.Clients
                .Where(c => c.AssignedToEmployeeId == currentUser.Id && c.AssignedAt == today)
                .Include(c => c.Project)
                .ToListAsync();

            // باقي الإحصائيات
            ViewBag.TotalClients = await _context.Clients.CountAsync();
            ViewBag.NewClientsToday = assignedTodayClients.Count;
            ViewBag.FollowUpClients = await _context.Clients.CountAsync(c => c.Status == "Follow Up");
            ViewBag.Meeting = await _context.Clients.CountAsync(c => c.Status == "Meeting");
            ViewBag.LowBudget = await _context.Clients.CountAsync(c => c.Status == "Low Budget");
            ViewBag.DoneDeal = await _context.Clients.CountAsync(c => c.Status == "Done Deal");
            ViewBag.NoAnsewar = await _context.Clients.CountAsync(c => c.Status == "No Ansewar");
            ViewBag.ProjectsCount = await _context.Projects.CountAsync();

            return View(assignedTodayClients); // ← العملاء المسندين للموظف النهاردة
        }
        [Authorize(Roles = "Marketing")]
        public async Task<IActionResult> Home_Marketing()
        {
            var today = DateTime.Today;

            // اسم المستخدم الحالي (يفترض إنه مسجّل)
            var userName = User.Identity.Name;

            // نحصل على الـ ID الخاص بالمستخدم الحالي
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.FullName == userName);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            // العملاء اللي الموظف الحالي أضافهم النهارده
            var todayClientsByCurrentUser = await _context.Clients
                .Where(c => c.CreatedAt.Date == today && c.AddedById == currentUser.Id)
                .Include(c => c.Project)
                .ToListAsync();

            // باقي الإحصائيات
            ViewBag.TotalClients = await _context.Clients.CountAsync();
            ViewBag.NewClientsToday = todayClientsByCurrentUser.Count;
            ViewBag.FollowUpClients = await _context.Clients.CountAsync(c => c.Status == "Follow Up");
            ViewBag.Meeting = await _context.Clients.CountAsync(c => c.Status == "Meeting");
            ViewBag.LowBudget = await _context.Clients.CountAsync(c => c.Status == "Low Budget");
            ViewBag.DoneDeal = await _context.Clients.CountAsync(c => c.Status == "Done Deal");
            ViewBag.NoAnsewar = await _context.Clients.CountAsync(c => c.Status == "No Ansewar");
            ViewBag.ProjectsCount = await _context.Projects.CountAsync();

            return View(todayClientsByCurrentUser); // ← إرسال العملاء المضافين من الموظف فقط
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
