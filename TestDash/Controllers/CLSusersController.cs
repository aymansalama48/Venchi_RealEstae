using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestDash.Data;
using TestDash.Models;

namespace TestDash.Controllers
{

    //[Route("CLSusers")]

    public class CLSusersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CLSusersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CLSusers
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> sign_in()
        {
            return View(await _context.Users.ToListAsync());
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> employees()
        {
            return View(await _context.Users.ToListAsync());
        }
      
        public async Task<IActionResult> Show_EmployeesFor_S_M()
        {
            return View(await _context.Users.ToListAsync());
        }


        // GET: CLSusers/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
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


        // POST: CLSusers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,JobTitle,Department,PhoneNumber,Email,HireDate,Notes,PasswordHash,Role,ProfileImageFileName")] CLSuser cLSuser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cLSuser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(employees));
            }
            return View(cLSuser);
        }

        // GET: CLSusers/Edit/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CLSuser updatedUser)
        {
            if (id != updatedUser.Id)
                return NotFound();

            try
            {
                var userInDb = await _context.Users.FindAsync(id);
                if (userInDb == null)
                    return NotFound();

                // تعديل الحقول القابلة للتغيير فقط
                userInDb.FullName = updatedUser.FullName;
                userInDb.JobTitle = updatedUser.JobTitle;
                userInDb.PhoneNumber = updatedUser.PhoneNumber;
                userInDb.Email = updatedUser.Email;
                userInDb.Notes = updatedUser.Notes;
                userInDb.Role = updatedUser.Role;

                await _context.SaveChangesAsync();

                TempData["UserSuccess"] = "تم تعديل بيانات الموظف بنجاح.";
                return RedirectToAction("Details", new { id = updatedUser.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == updatedUser.Id))
                {
                    return NotFound();
                }

                TempData["UserError"] = "حدث تعارض أثناء تعديل البيانات.";
                TempData["OldUser"] = JsonConvert.SerializeObject(updatedUser);
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                TempData["UserError"] = "حدث خطأ أثناء الحفظ: " + ex.Message;
                TempData["OldUser"] = JsonConvert.SerializeObject(updatedUser);
                return RedirectToAction(nameof(employees));
            }
        }


        // GET: CLSusers/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CLSuser/DeleteConfirmed/{id:int}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cLSuser = await _context.Users.FindAsync(id);
            if (cLSuser != null)
            {
                _context.Users.Remove(cLSuser);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(employees));
        }

        private bool CLSuserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
