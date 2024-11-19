using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Direct_Barber.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Direct_Barber.Controllers
{
    public class TipoSersController : Controller
    {
        private readonly DirectBarber1Context _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public TipoSersController(DirectBarber1Context context, IWebHostEnvironment webHost)
        {
            _context = context;
            this.webHostEnvironment = webHost;
        }

        // GET: TipoSers
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoSers.ToListAsync());
        }

        // GET: TipoSers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoSer = await _context.TipoSers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoSer == null)
            {
                return NotFound();
            }

            return View(tipoSer);
        }

        // GET: TipoSers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoSers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoSer tipoSer)
        {
            if (ModelState.IsValid)
            {
                string uFileName = UploadedFile(tipoSer);
                tipoSer.Foto = uFileName;
                _context.Add(tipoSer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoSer);
        }

        // GET: TipoSers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {

                return NotFound();
            }

            var tipoSer = await _context.TipoSers.FindAsync(id);
            if (tipoSer == null)
            {
                return NotFound();
            }
            return View(tipoSer);
        }

        // POST: TipoSers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Foto,Precio")] TipoSer tipoSer)
        {
            if (id != tipoSer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoSer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoSerExists(tipoSer.Id))
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
            return View(tipoSer);
        }

        // GET: TipoSers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoSer = await _context.TipoSers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoSer == null)
            {
                return NotFound();
            }

            return View(tipoSer);
        }

        // POST: TipoSers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoSer = await _context.TipoSers.FindAsync(id);
            if (tipoSer != null)
            {
                _context.TipoSers.Remove(tipoSer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoSerExists(int id)
        {
            return _context.TipoSers.Any(e => e.Id == id);
        }

        private string UploadedFile(TipoSer tipoSer)
        {
            string uFileName = null;

            if (tipoSer.ImagenFile != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uFileName = Guid.NewGuid().ToString() + "_" + tipoSer.ImagenFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uFileName);
                using (var myFileStream = new FileStream(filePath, FileMode.Create))
                {
                    tipoSer.ImagenFile.CopyTo(myFileStream);
                }

            }
            return uFileName;
        }
    }
}
