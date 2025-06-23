using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TestDash.Data;
using TestDash.Models;


namespace TestDash.Controllers
{
  

    public class CLSclientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CLSclientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CLSclients
        // عرض جميع العملاء

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> clients()
        {
            var clients = await _context.Clients
                .Include(c => c.AddedBy)
                .Include(c => c.Project)
                .ToListAsync();

            ViewBag.Projects = new SelectList(await _context.Projects.ToListAsync(), "Id", "Name");

            // تمرير رسالة الخطأ إلى الـ View
            ViewBag.ClientError = TempData["ClientError"] as string;

            return View(clients);
        }
        // عرض جميع العملاء

        [Authorize(Roles = "Sales")]
        public async Task<IActionResult> Client_Sales()
        {
            // جلب رقم المستخدم الحالي (المندوب)
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            // جلب العملاء المخصصين لهذا المندوب فقط
            var clients = await _context.Clients
                .Include(c => c.AddedBy)
                .Include(c => c.Project)
                .Where(c => c.AssignedToEmployeeId == userId)
                .ToListAsync();

            ViewBag.Projects = new SelectList(await _context.Projects.ToListAsync(), "Id", "Name");
            ViewBag.ClientError = TempData["ClientError"] as string;

            return View(clients);
        }


        [Authorize(Roles = "Marketing")]
        public async Task<IActionResult> Client_Marketing()
        {
            // جلب رقم المستخدم الحالي (الموظف)
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            // جلب العملاء الذين أضافهم هذا الموظف فقط
            var clients = await _context.Clients
                .Include(c => c.AddedBy)
                .Include(c => c.Project)
                .Where(c => c.AddedById == userId)
                .ToListAsync();

            ViewBag.Projects = new SelectList(await _context.Projects.ToListAsync(), "Id", "Name");
            ViewBag.ClientError = TempData["ClientError"] as string;

            return View(clients);
        }










        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendClient()
        {
            var clients = await _context.Clients
         .Include(c => c.Project)
         .Include(c => c.AddedBy)
         .Where(c => c.AssignedToEmployeeId == null) // ✅ فقط اللي مش مسندين
         .ToListAsync();

            ViewBag.Employees = new SelectList(await _context.Users.ToListAsync(), "Id", "FullName"); // لو بتعرض الموظفين في دروب داون

           

            // كنترولر
            var salesEmployees = _context.Users
                .Where(u => u.Role == "Sales") // ← هنا بنجيب فقط موظفي المبيعات
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.FullName
                }).ToList();

            ViewBag.Employees = salesEmployees;


            // داخل الأكشن اللي فيه الزر
  

            // تمرير رسالة الخطأ إلى الـ View
            ViewBag.ClientError = TempData["ClientError"] as string;

            return View(clients);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AssignClients(int EmployeeId, List<int> SelectedClientIds)
        {
            if (SelectedClientIds != null && SelectedClientIds.Any())
            {
                var clients = await _context.Clients
                    .Where(c => SelectedClientIds.Contains(c.Id))
                    .ToListAsync();

                // جلب ID الأدمن الحالي من الـ Claim
                var adminIdClaim = User.FindFirst("UserId");
                if (adminIdClaim == null)
                    return Unauthorized();

                int adminId = int.Parse(adminIdClaim.Value);

                foreach (var client in clients)
                {
                    client.AssignedToEmployeeId = EmployeeId;
                    client.AssignedAt = DateTime.Now;
                    client.AssignedById = adminId; // 👈 تسجيل من اللي عمل الإسناد
                }

                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "تم الإسناد بنجاح.";
            return RedirectToAction("SendClient");
        }











        // GET: CLSclients/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cLSclient = await _context.Clients
                .Include(c => c.AddedBy)
                .Include(c => c.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cLSclient == null)
            {
                return NotFound();
            }

            return View(cLSclient);
        }

        [Authorize(Roles = "Sales")]
        public async Task<IActionResult> Details_Sales(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cLSclient = await _context.Clients
                .Include(c => c.AddedBy)
                .Include(c => c.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cLSclient == null)
            {
                return NotFound();
            }

            return View(cLSclient);
        }

        [Authorize(Roles = "Marketing")]
        public async Task<IActionResult> Details_Marketing(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cLSclient = await _context.Clients
                .Include(c => c.AddedBy)
                .Include(c => c.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cLSclient == null)
            {
                return NotFound();
            }

            return View(cLSclient);
        }

        // GET: CLSclients/Create
       
        public IActionResult Create()
        {
            ViewData["AddedById"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            return View();
        }

        // POST: CLSclients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Admin,Marketing")]
        [HttpPost]
        public async Task<IActionResult> Create(CLSclient cLSclient)
        {
            // إضافة بيانات تلقائية
            cLSclient.Status = "New";
            cLSclient.CreatedAt = DateTime.Now;

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Unauthorized();

            cLSclient.AddedById = int.Parse(userIdClaim.Value);

            if (!ModelState.IsValid)
            {
                TempData["ClientError"] = "هناك خطأ في البيانات المدخلة.";
                TempData["OldClient"] = JsonConvert.SerializeObject(cLSclient);
                return RedirectToAction(nameof(clients));
            }

            try
            {
                _context.Add(cLSclient);
                await _context.SaveChangesAsync();
                TempData["ClientSuccess"] = "تمت إضافة العميل بنجاح.";

                // 🔄 التوجيه حسب الدور باستخدام switch
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                switch (role)
                {
                    case "Admin":
                        return RedirectToAction("clients");

                    case "Marketing":
                        return RedirectToAction("Client_Marketing");


                    default:
                        return RedirectToAction("clients"); // افتراضي
                }
            }
            catch (Exception ex)
            {
                TempData["ClientError"] = "حدث خطأ أثناء الحفظ: " + ex.Message;
                TempData["OldClient"] = JsonConvert.SerializeObject(cLSclient);
                return RedirectToAction(nameof(clients));
            }
        }

        // GET: CLSclients/Edit/5
        // GET: CLSclients/Edit/5
        public async Task<IActionResult> client_edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cLSclient = await _context.Clients
                .Include(c => c.Project)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cLSclient == null)
            {
                return NotFound();
            }

            // إنشاء SelectList لحالة العميل (Status)
            var statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "New", Text = "New" },
                new SelectListItem { Value = "Follow Up", Text = "Follow Up" },
                new SelectListItem { Value = "No Answer", Text = "No Answer" }, // تم تصحيح الخطأ الإملائي: Ansewar -> Answer
                new SelectListItem { Value = "Meeting", Text = "Meeting" },
                new SelectListItem { Value = "Done Deal", Text = "Done Deal" },
                new SelectListItem { Value = "Low Budget", Text = "Low Budget" }
            };
            ViewBag.StatusList = new SelectList(statuses, "Value", "Text", cLSclient.Status);


            // إنشاء SelectList للمصدر (Source)
            var sources = new List<SelectListItem>
            {
                new SelectListItem { Value = "فيس بوك", Text = "فيس بوك" },
                new SelectListItem { Value = "بايوت", Text = "بايوت" },
                new SelectListItem { Value = "دوبزل", Text = "دوبزل" },
                new SelectListItem { Value = "واتساب", Text = "واتساب" },
                new SelectListItem { Value = "موقع إلكتروني", Text = "موقع إلكتروني" },
                new SelectListItem { Value = "إحالة", Text = "إحالة" }
            };
            ViewBag.SourceList = new SelectList(sources, "Value", "Text", cLSclient.Source);


            ViewData["AddedById"] = new SelectList(_context.Users, "Id", "FullName", cLSclient.AddedById);
            // تأكد من أنك تريد "ImageUrl" كـ Text للعرض أو قم بتغييرها إلى "Name" أو "Title" للمشروع
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", cLSclient.ProjectId); // تغيير "ImageUrl" إلى "Name" أو ما يعرض اسم المشروع


            return View(cLSclient);
        }
        public async Task<IActionResult> Client_Edit_Sales(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cLSclient = await _context.Clients
                .Include(c => c.Project)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cLSclient == null)
            {
                return NotFound();
            }

            // إنشاء SelectList لحالة العميل (Status)
            var statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "New", Text = "New" },
                new SelectListItem { Value = "Follow Up", Text = "Follow Up" },
                new SelectListItem { Value = "No Answer", Text = "No Answer" }, // تم تصحيح الخطأ الإملائي: Ansewar -> Answer
                new SelectListItem { Value = "Meeting", Text = "Meeting" },
                new SelectListItem { Value = "Done Deal", Text = "Done Deal" },
                new SelectListItem { Value = "Low Budget", Text = "Low Budget" }
            };
            ViewBag.StatusList = new SelectList(statuses, "Value", "Text", cLSclient.Status);


            // إنشاء SelectList للمصدر (Source)
            var sources = new List<SelectListItem>
            {
                new SelectListItem { Value = "فيس بوك", Text = "فيس بوك" },
                new SelectListItem { Value = "بايوت", Text = "بايوت" },
                new SelectListItem { Value = "دوبزل", Text = "دوبزل" },
                new SelectListItem { Value = "واتساب", Text = "واتساب" },
                new SelectListItem { Value = "موقع إلكتروني", Text = "موقع إلكتروني" },
                new SelectListItem { Value = "إحالة", Text = "إحالة" }
            };
            ViewBag.SourceList = new SelectList(sources, "Value", "Text", cLSclient.Source);


            ViewData["AddedById"] = new SelectList(_context.Users, "Id", "FullName", cLSclient.AddedById);
            // تأكد من أنك تريد "ImageUrl" كـ Text للعرض أو قم بتغييرها إلى "Name" أو "Title" للمشروع
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", cLSclient.ProjectId); // تغيير "ImageUrl" إلى "Name" أو ما يعرض اسم المشروع


            return View(cLSclient);
        }

        // POST: CLSclients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CLSclient cLSclient)
        {
            if (id != cLSclient.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Field '{entry.Key}' error: {error.ErrorMessage}");
                    }
                }

                TempData["ClientError"] = "هناك أخطاء في البيانات المدخلة.";
                TempData["OldClient"] = JsonConvert.SerializeObject(cLSclient);
                return RedirectToAction(nameof(clients));
            }

            try
            {
                var clientInDb = await _context.Clients.FindAsync(id);
                if (clientInDb == null)
                    return NotFound();

                clientInDb.Name = cLSclient.Name;
                clientInDb.Phone = cLSclient.Phone;
                clientInDb.Status = cLSclient.Status;
                clientInDb.ProjectId = cLSclient.ProjectId;
                clientInDb.Source = cLSclient.Source;
                clientInDb.Notes = cLSclient.Notes;
                clientInDb.CreatedAt = cLSclient.CreatedAt;

                await _context.SaveChangesAsync();

                TempData["ClientSuccess"] = "تم تعديل بيانات العميل بنجاح.";

                // ✅ تحديد الدور الحالي للمستخدم
                var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                var role = roleClaim?.Value;

                // ✅ استخدام switch للتوجيه حسب الدور
                switch (role)
                {
                    case "Admin":
                        return RedirectToAction("Details", new { id = cLSclient.Id });

                    case "Sales":
                        return RedirectToAction("Details_Sales", new { id = cLSclient.Id });

                    //case "Marketing":
                    //    return RedirectToAction("Client_Marketing");

                    default:
                        return RedirectToAction("clients");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Clients.Any(e => e.Id == cLSclient.Id))
                    return NotFound();

                TempData["ClientError"] = "حدث تعارض أثناء تعديل البيانات.";
                TempData["OldClient"] = JsonConvert.SerializeObject(cLSclient);
                return RedirectToAction(nameof(clients));
            }
            catch (Exception ex)
            {
                TempData["ClientError"] = "حدث خطأ أثناء الحفظ: " + ex.Message;
                TempData["OldClient"] = JsonConvert.SerializeObject(cLSclient);
                return RedirectToAction(nameof(clients));
            }
        }
        // GET: CLSclients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cLSclient = await _context.Clients
                .Include(c => c.AddedBy)
                .Include(c => c.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cLSclient == null)
            {
                return NotFound();
            }

            return View(cLSclient);
        }

        // POST: CLSclients/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cLSclient = await _context.Clients.FindAsync(id);
            if (cLSclient != null)
            {
                _context.Clients.Remove(cLSclient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(clients));
        }

        private bool CLSclientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }





    }
}
