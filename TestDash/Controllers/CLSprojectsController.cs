using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestDash.Data;
using TestDash.Models;

namespace TestDash.Controllers
{

    public class CLSprojectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CLSprojectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CLSprojects

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> projects()
        {
            return View(await _context.Projects.ToListAsync());
        }

        public async Task<IActionResult> Project_S_M()
        {
            return View(await _context.Projects.ToListAsync());
        }

        // GET: CLSprojects/Details/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cLSproject = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cLSproject == null)
            {
                return NotFound();
            }

            return View(cLSproject);
        }


        // POST: CLSprojects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ImageUrl,Location")] CLSproject cLSproject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cLSproject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(projects));
            }
            return View(cLSproject);
        }

        // GET: CLSprojects/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cLSproject = await _context.Projects.FindAsync(id);
            if (cLSproject == null)
            {
                return NotFound();
            }
            return View(cLSproject);
        }

        // POST: CLSprojects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImageUrl,Location")] CLSproject cLSproject)
        {
            if (id != cLSproject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cLSproject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CLSprojectExists(cLSproject.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(projects));
            }
            return View(cLSproject);
        }

        // GET: CLSprojects/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cLSproject = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cLSproject == null)
            {
                return NotFound();
            }

            return View(cLSproject);
        }

        // POST: CLSprojects/Delete/5
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cLSproject = await _context.Projects.FindAsync(id);
            if (cLSproject != null)
            {
                _context.Projects.Remove(cLSproject);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(projects));
        }

        private bool CLSprojectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
