using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Xml;
using Ganss.XSS;
using System.Web;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace HotelManagement.Controllers
{
	public class ItemsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;


		public ItemsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		//GET: Items
		public async Task<IActionResult> Index()
		{
			//ViewBag.userid = _userManager.GetUserId(HttpContext.User);
			return View(await _context.Item.Where(i => i.active == 1).ToListAsync());
		}

		// POST: Items/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//[Authorize]
		//public async Task<IActionResult> Index(String? SearchPhrase)
		//{
		//    ViewData["SearchPhrase"] = SearchPhrase;
		//    IQueryable<Item> query = _context.Item.Where(i => i.active == 1);
		//    if(SearchPhrase != null)
		//    {
		//        query = query.Where(i => i.Name.Contains(SearchPhrase));
		//    }
		//    return View(await query.ToListAsync());
		//}

		// GET: Items/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{			
				return NotFound();
			}

			var item = await _context.Item
				.Include(i => i.creator)
				.Include(i => i.ItemImages)
				.AsNoTracking()
				.FirstOrDefaultAsync(m => m.Id == id)
				;
			
			if (item == null)
			{
				return NotFound();
			}

            var emp = (from i in _context.Item
                       join e in _context.Employee
                           on i.creatorId equals e.UserId                       
					   select new
					   {
							itemId = i.Id,
							empName = e.Name
					   })
					   .FirstOrDefault(e => e.itemId == id);
			if(emp == null)
			{
				if (item.creator == null)
				{
					ViewBag.empName = "N/A";
				}
				else
				{
					ViewBag.empName = item.creator.UserName;
				}
			}
			else
			{
				ViewBag.empName = emp.empName;
			}


			ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id", HttpContext.Session.GetString("primaryOrder"));
			//ViewData["OrderItem"] = _context.OrderItem;
			return View(item);
		}

		// GET: Items/Create
		[Authorize]
		public IActionResult Create()
		{
			ViewBag.userid = _userManager.GetUserId(HttpContext.User);
			return View();
		}

		// POST: Items/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,QuantityPar,Quantity,creatorId")] Item item)
		{
		 
			if (ModelState.IsValid)
			{
				_context.Add(item);
				await _context.SaveChangesAsync();

				//_context.
				return RedirectToAction(nameof(Index));
			}
			return View(item);
		} 

		// GET: Items/Edit/5
		[Authorize]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var item = await _context.Item
				.FirstOrDefaultAsync(m => m.Id == id);
			if (item == null)
			{

				string userid = _userManager.GetUserId(HttpContext.User);
				ViewBag.employee = await _context.Employee
					.FirstOrDefaultAsync(m => m.UserId == userid);

				return NotFound();
			}
			return View(item);
		}

		// POST: Items/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,QuantityPar,Quantity,active")] Item item)
		{
			if (id != item.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(item);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ItemExists(item.Id))
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
			return View(item);
		}

		// GET: Items/Delete/5
		[Authorize]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var item = await _context.Item
				.FirstOrDefaultAsync(m => m.Id == id);
			if (item == null)
			{
				return NotFound();
			}

			return View(item);
		}

		// POST: Items/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var item = await _context.Item.FindAsync(id);
			_context.Item.Remove(item);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ItemExists(int id)
		{
			return _context.Item.Any(e => e.Id == id);
		}

		private string Gsanitizer(string myString)
		{
			myString = HttpUtility.HtmlEncode(myString);
			myString = myString.Replace(Environment.NewLine, "<br>");
			return myString;
		}

		public JsonResult ajaxTest(RouteValueDictionary l1a)
		{
			var obj = new
			{
				k = 1,
				p =2,
				l= new
				{
					qw = 4,
					ee1 = 5,
					fre = 'a'
				}
			};
			//string response="{l1a:"+Convert.ToString(l1a["l2c"])+"}";
			//dynamic response = JsonConvert.DeserializeObject<dynamic>(l1a);
			//string response = (string)l1b;
			//string response = l1a;
			return Json(l1a);
		}
 

		[HttpPost]
		public JsonResult getItems(int start, int length, Dictionary<string, string> search)
		{
			int totalRecords = 0;
			int totalRecordwithFilter = 0;
			List<ItemDataRow> aaData = new List<ItemDataRow>();
			IQueryable<Item> items = _context.Item
				.Where(i => i.active == 1)
				.OrderBy(i => i.Name);
			totalRecords = items.Count();
			if (search["value"] != null)
			{
				items = items.Where(i => i.Name.Contains(search["value"]) || i.Description.Contains(search["value"]));
			}
			totalRecordwithFilter = items.Count();
			items = items.Skip(start).Take(length);
			
			foreach(var item in items)
			{
				string rowAction = "<a href='Items/Edit/" + item.Id + "'>Edit</a> |" +
					" <a href='Items/Details/" + item.Id + "'>Details</a> |" +
					" <a href='Items/Delete/" + item.Id + "'>Delete</a>";

				ItemDataRow itemData = new ItemDataRow(
					Gsanitizer(item.Name),
					item.Quantity,
					item.QuantityPar,
					item.Price,
					Gsanitizer(item.Description),
					rowAction
				);

				aaData.Add(itemData);
					
			}


			tbl_items_response response = new tbl_items_response
			{
				iTotalRecords = totalRecords,
				iTotalDisplayRecords = totalRecordwithFilter,
				aaData = aaData
			};


			return Json(response);
		}

		class ItemDataRow
		{
			public string Name { get; set; }
			public int Quantity { get; set; }
			public int QuantityPar { get; set; }
			public decimal Price { get; set; }
			public string Description { get; set; }
			public string Action { get; set; }

			public ItemDataRow(string Name, int Quantity, int QuantityPar, decimal Price, string Description, string Action)
			{
				this.Name = Name;
				this.Quantity = Quantity;
				this.QuantityPar = QuantityPar;
				this.Price = Price;
				this.Description = Description;
				this.Action = Action;
			}

		}

		class tbl_items_response
		{
			public int iTotalRecords { get; set; }
			public int iTotalDisplayRecords { get; set; }
			public dynamic aaData { get; set; }
		}

		class objSearch
		{
			public string value { get; set; }
			public string regex { get; set; }
		}

	}
}
