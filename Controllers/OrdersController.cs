using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Html;

namespace HotelManagement.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {

            var order = _context.Order
                .Include(o => o.Creator)                
                .Where(o => o.Active.Equals(1))                
                .OrderByDescending(o=>o.Id);

            Dictionary<int, string> employees = new Dictionary<int, string>();
            var emps = order
                .Join(
                    _context.Employee,
                    o => o.Creator.Id,
                    emp => emp.UserId,
                    (o, emp) => new
                    {
                        orderId = o.Id,
                        empName = emp.Name
                    }
                );
            foreach (var emp in emps)
            {
                employees.Add(emp.orderId, emp.empName);
            }
            //foreach (var o in order)
            //{
            //    employees.Add(o.o.Id, o.empName);
            //}
            ViewBag.emps = employees;

            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Employee
                .FirstOrDefault(e => e.UserId == userId);
            ViewBag.userPermission = user.Permission;

            ViewBag.primaryOrder = HttpContext.Session.GetString("primaryOrder");

            return View(order);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Creator)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            var emp = (from o in _context.Order
                       join e in _context.Employee
                           on o.CreatorId equals e.UserId
                       select new
                       {
                           orderId = o.Id,
                           empName = e.Name,
                       })
                       .FirstOrDefault(e => e.orderId == id);
            if (emp == null)
            {
                if (order.Creator == null)
                {
                    ViewBag.empName = "N/A";
                }
                else
                {
                    ViewBag.empName = order.Creator.UserName;
                }
            }
            else
            {
                ViewBag.empName = emp.empName;
            }

            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Employee
                .FirstOrDefault(e => e.UserId == userId);
            ViewBag.userPermission = user.Permission;

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewBag.userid = _userManager.GetUserId(HttpContext.User);
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Name,CreatorId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", order.CreatorId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", order.CreatorId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status,Active, CreatorId, Date_created")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", order.CreatorId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            order.Active = 0;
            _context.Update(order);
            //_context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }

        [HttpPost]
        public JsonResult setPrimaryOrder(string order)
        {
            var response = new Dictionary<string, dynamic>();
            response.Add("status", false);
            response.Add("message", "error");

            HttpContext.Session.SetString("primaryOrder", order);
            response["status"] = true;
            response["message"] = "save";

            return Json(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveAudit(int id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Creator)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (order == null)
            {
                return NotFound();
            }
            foreach ( var oi in order.OrderItems)
            {
                int newQty = oi.Quantity;

                oi.Item.Quantity = newQty;
                await _context.SaveChangesAsync();
            }

            order.Status = "Approved";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id});
        }
    }
}
