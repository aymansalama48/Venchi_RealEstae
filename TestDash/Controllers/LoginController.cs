using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TestDash.Data;
using TestDash.Models;

namespace TestDash.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }


        



        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("sign_in", "Login"); // يرجّعه لصفحة تسجيل الدخول
        }

         



        [HttpPost]
        public async Task<IActionResult> Login(CLSuser User)
        {
            // تحقق من وجود المستخدم
            var user = _context.Users.FirstOrDefault(u => u.Email == User.Email && u.PasswordHash == User.PasswordHash);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("JobTitle", user.JobTitle ?? "");
                HttpContext.Session.SetString("Department", user.Department ?? "");
                HttpContext.Session.SetString("PhoneNumber", user.PhoneNumber);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("ProfileImage", user.ProfileImageFileName ?? "");

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role), // مثلاً "Admin" أو "Sales"
                new Claim("UserId", user.Id.ToString())  // 👈 مهم جداً
        };


                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);






                // التوجيه حسب الدور
                switch (user.Role)
                {
                    case "Admin":
                        return RedirectToAction("Index", "Home");

                    case "Marketing":
                        return RedirectToAction("Home_Marketing", "Home");

                    case "Sales":
                        return RedirectToAction("Home_Sales", "Home");

                    default:
                        return RedirectToAction("sign_in", "Login");
                }
            }
            ModelState.Remove("FullName");
            ModelState.Remove("Role");
            ModelState.Remove("JobTitle");
            ModelState.Remove("Department");
            ModelState.Remove("PhoneNumber");

            ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة");
            return View("sign_in");
        }


        public async Task<IActionResult> sign_in()
        {
            return View(await _context.Users.ToListAsync());
        }



        public async Task<IActionResult> profile()
        {
            return View(await _context.Users.ToListAsync());
        }







        // GET: CLSusers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CLSusers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,JobTitle,Department,PhoneNumber,Email,HireDate,Notes,PasswordHash,Role,ProfileImageFileName")] CLSuser cLSuser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cLSuser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cLSuser);
        }

        // GET: CLSusers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cLSuser = await _context.Users.FindAsync(id);
            if (cLSuser == null)
            {
                return NotFound();
            }
            return View(cLSuser);
        }

        // POST: CLSusers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,JobTitle,Department,PhoneNumber,Email,HireDate,Notes,PasswordHash,Role,ProfileImageFileName")] CLSuser cLSuser)
        {
            if (id != cLSuser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cLSuser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CLSuserExists(cLSuser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cLSuser);
        }

        // GET: CLSusers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cLSuser = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cLSuser == null)
            {
                return NotFound();
            }

            return View(cLSuser);
        }

        // POST: CLSusers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cLSuser = await _context.Users.FindAsync(id);
            if (cLSuser != null)
            {
                _context.Users.Remove(cLSuser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CLSuserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
