using eShopCore.Hubs;
using eShopCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace eShopCore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductDbContext _context;
        private readonly IHubContext<ProductHub> _hub;

        public ProductsController(ProductDbContext context, IHubContext<ProductHub> hub) {
            _context = context;
            _hub = hub;
        }

        // GET: Products
        public async Task<IActionResult> Index() {
            return _context.Products != null ?
                        View(await _context.Products.OrderByDescending(p => p.ID).ToListAsync()) :
                        Problem("Entity set 'ProductDbContext.Products'  is null.");
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null || _context.Products == null) {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null) {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult CreateProduct([FromBody] Product product) {
            _context.Add(product);
            _context.SaveChanges();
            _hub.Clients.All.SendAsync("addedItem", product);
            _hub.Clients.All.SendAsync("editedOrCreatedProduct", product);
            return Json(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null || _context.Products == null) {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null) {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult EditProduct([FromBody] Product product) { //[Bind("ID,Title,DescriptionShort,DescriptionLong,Category,Price,ImageSource,Manufacturer")] 
                                                                    //if (id != product.ID) {
                                                                    //    return Json(NotFound());
                                                                    //}
            var dbproduct = _context.Products.Find(product.ID);
            if (dbproduct == null) {
                return Json(NotFound());
            }

            try {
                dbproduct.Update(product);
                _context.SaveChanges();
                _hub.Clients.All.SendAsync("updateProduct", product);
                _hub.Clients.All.SendAsync("editedOrCreatedProduct", product);
            }
            catch (DbUpdateConcurrencyException ex) {
                if (!ProductExists(product.ID)) {
                    return Json(NotFound());
                }
                else {
                    throw ex;
                }
            }
            return Json(dbproduct);
        }



        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null || _context.Products == null) {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null) {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost]
        public JsonResult DeleteConfirmed([FromBody] int ID) {
            if (_context.Products == null) {
                return Json("Entity set 'ProductDbContext.Products'  is null.");
            }
            var product = _context.Products.Find(ID);
            if (product != null) {
                _context.Products.Remove(product);
            }
            _context.SaveChanges();
            _hub.Clients.All.SendAsync("deletedItem", product);
            return Json(null);
        }

        private bool ProductExists(int id) {
            return (_context.Products?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}

