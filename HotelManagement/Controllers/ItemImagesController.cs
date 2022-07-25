using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Controllers
{
    public class ItemImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment hostingEnvironment;

        public ItemImagesController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: ItemImages
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ItemImage.Include(i => i.Item);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ItemImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemImage = await _context.ItemImage
                .Include(i => i.Item)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemImage == null)
            {
                return NotFound();
            }

            return View(itemImage);
        }

        // GET: ItemImages/Create
        public IActionResult Create(int? id)
        {
            ViewData["ItemId"] = new SelectList(_context.Item.Where(i=>i.active==1).OrderBy(i => i.Name), "Id", "Name", id);
            ViewData["Id"] = id;
            return View();
        }

        // POST: ItemImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,ItemId")] ItemImage itemImage, IFormFile fileUpload)
        {

            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (fileUpload != null)
                {
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "uploads/ItemImages");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + fileUpload.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    fileUpload.CopyTo(new FileStream(filePath, FileMode.Create));

                    itemImage.Filename = uniqueFileName;
                }
                _context.Add(itemImage);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Items", new { @id = itemImage.ItemId });
            }
            ViewData["ItemId"] = new SelectList(_context.Item, "Id", "Name", itemImage.ItemId);


            var item = await _context.Item
                .Include(i => i.creator)
                .Include(i => i.ItemImages)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == itemImage.ItemId);

            return View(item);
        }

        // GET: ItemImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemImage = await _context.ItemImage.FindAsync(id);
            if (itemImage == null)
            {
                return NotFound();
            }
            ViewData["ItemId"] = new SelectList(
                _context.Item.Where(i => i.active == 1)
                , "Id", "Name", itemImage.ItemId);
            return View(itemImage);
        }

        // POST: ItemImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Filename,ItemId")] ItemImage itemImage)
        {
            if (id != itemImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemImage);
                    await _context.SaveChangesAsync();
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemImageExists(itemImage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Items", new { @id = itemImage.ItemId });
            }
            ViewData["ItemId"] = new SelectList(_context.Item, "Id", "Id", itemImage.ItemId);
            return View(itemImage);


        }

        // GET: ItemImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemImage = await _context.ItemImage
                .Include(i => i.Item)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemImage == null)
            {
                return NotFound();
            }

            return View(itemImage);
        }

        // POST: ItemImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemImage = await _context.ItemImage.FindAsync(id);
            _context.ItemImage.Remove(itemImage);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Details", "Items", new { @id = itemImage.ItemId });
        }

        private bool ItemImageExists(int id)
        {
            return _context.ItemImage.Any(e => e.Id == id);
        }
    }
}
