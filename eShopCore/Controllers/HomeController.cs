using Newtonsoft.Json;
using PagedList;
using Microsoft.AspNetCore.Mvc;
using eShopCore.Models;
using Microsoft.EntityFrameworkCore;
using eShopCore.Hubs;
using System.Timers;
using Microsoft.AspNetCore.SignalR;

namespace eShopApp.Controllers
{
    /* All kinds of data */
    public static class DataProvider
    {
        private static List<Product> _products { get; set; }
        public static List<Product> Products {
            get {
                if (_products == null) {
                    using (StreamReader r = new StreamReader("Content/json.json")) {
                        string json = r.ReadToEnd();
                        List<Product> items = JsonConvert.DeserializeObject<List<Product>>(json);
                        _products = items;
                    }
                }
                return _products;
            }
        }
    }
    public class PagedData<T>
    {
        public int TotalPages { get; set; }
        public List<T> Data { get; set; }
    }

    public class ProductsWithCategories : PagedData<Product>
    {
        public List<string> Categories { get; set; }
    }

    /* CONTROLLER */

    public class HomeController : Controller
    {
        private bool useSQLite = false;

        public HomeController(ProductDbContext context, IHubContext<ProductHub> hub) {
            _context = context;
            _hub = hub;
        }
        private readonly ProductDbContext _context;
        private readonly IHubContext<ProductHub> _hub;
        private SqliteDbContext sqliteDB = new SqliteDbContext();
        
        public MyDbContext ActiveContext {
            get {
                if (useSQLite)
                    return sqliteDB;
                else
                    return _context;
            }
        }

        void _initSQLite() {
            if (useSQLite) {
                //Ensure database is created
                sqliteDB.Database.EnsureCreated();
                if (!sqliteDB.Products.Any()) {
                    string dbName = "eShopDatabase.db";
                    if (System.IO.File.Exists(dbName)) {
                        System.IO.File.Delete(dbName);
                    }
                    foreach (Product pr in DataProvider.Products) {
                        sqliteDB.Products.Add(pr);
                        sqliteDB.SaveChanges();
                    }
                }
            }
        }
        public ActionResult Index() {
            _initSQLite();
            int pageSize = 4;
            var products = from p in DataProvider.Products //ActiveContext instead of DataProvider once connection with established database has been created
                           select p;
            products = products.OrderBy(p => p.Category);
            var categories = products.Select(x => x.Category).Distinct().ToList();
            var pagedList = products.ToPagedList(1, pageSize);
            var retVal = new ProductsWithCategories() { Data = pagedList.ToList(), TotalPages = pagedList.PageCount, Categories = categories };
            return View(retVal);
        }
        public ActionResult Cart() {
            ViewBag.Message = "Your application's cart page.";
            return View();
        }

        public JsonResult SearchProducts(SearchParameters parameters) {
            var products = from p in DataProvider.Products //ActiveContext instead of DataProvider once connection with established database has been created
                           select p;
            if (!String.IsNullOrEmpty(parameters.Query)) {
                parameters.Query = parameters.Query.Trim().ToLower();
                products = products.Where(p => p.Title.ToLower().Contains(parameters.Query)
                || p.Category.ToLower().Contains(parameters.Query)
                || p.DescriptionLong.ToLower().Contains(parameters.Query)
                || p.DescriptionShort.ToLower().Contains(parameters.Query)
                || p.Manufacturer.ToLower().Contains(parameters.Query));
            }
            products = products.OrderBy(p => p.Category);
            int totalPages = (products.Count() + parameters.PageSize - 1) / parameters.PageSize;
            int pageNumber = (parameters.Page ?? 1);
            var retVal = new PagedData<Product>() { Data = products.ToPagedList(pageNumber, parameters.PageSize).ToList(), TotalPages = totalPages };
            return Json(retVal);
        }

        public JsonResult SearchCategories(SearchParameters parameters) {
            var products = from p in DataProvider.Products //ActiveContext instead of DataProvider once connection with established database has been created
                           select p;
            if (!String.IsNullOrEmpty(parameters.Query)) {
                parameters.Query = parameters.Query.Trim().ToLower();
                products = products.Where(p => p.Category.ToLower().Equals(parameters.Query)).OrderBy(p => p.ID);
    
            }
            products = products.OrderBy(p => p.Category);
            int totalPages = (products.Count() + parameters.PageSize - 1) / parameters.PageSize;
            int pageNumber = (parameters.Page ?? 1);
            var retVal = new PagedData<Product>() { Data = products.ToPagedList(pageNumber, parameters.PageSize).ToList(), TotalPages = totalPages };
            return Json(retVal);
        }
    }
}
