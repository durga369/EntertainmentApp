using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Vidly.Models;
using Vidly.ViewModels;

namespace Vidly.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult New()
        {
            var membershiptype = _context.MembershipType.ToList();
            var customer = new Customer();
            var viewModel = new CustomerFormViewModel
            {
                MembershipType = membershiptype,
                Customer = customer

            };
            return View("CustomerForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Customer customer)
        {
            if(!ModelState.IsValid)
            {
                var viewModel = new CustomerFormViewModel
                {
                    Customer = customer,
                    MembershipType = _context.MembershipType.ToList()
                };
                return View("CustomerForm", viewModel);
            }
            if (customer.Id == 0)
            {
                _context.Customers.Add(customer);
            }
            else
            {
                var cusInDb = _context.Customers.Single(c => c.Id == customer.Id);
                cusInDb.Name = customer.Name;
                cusInDb.BirthDate = customer.BirthDate;
                cusInDb.MembershipTypeId = customer.MembershipTypeId;
                cusInDb.IsSubscribedToNewsLetter = customer.IsSubscribedToNewsLetter;
               
            }
           
            _context.SaveChanges();
            return RedirectToAction("Index","Customers");
        }

        // GET: Customers
        public ActionResult Index()
        {
            var customers = _context.Customers.Include(c => c.MembershipType).ToList();
            return View(customers);
        }

        public ActionResult Details(int id)
        {
            var customers = _context.Customers.Include(c => c.MembershipType).SingleOrDefault(c => c.Id == id);
            if (customers == null)
                return HttpNotFound();
            
            return View(customers);
        }
        
        public ActionResult Edit(int id)
        {
            var customer = _context.Customers.SingleOrDefault(c => c.Id == id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            var viewModel = new CustomerFormViewModel
            {
                Customer = customer,
                MembershipType = _context.MembershipType.ToList()
            };
            return View("CustomerForm", viewModel);
        }
    }
}