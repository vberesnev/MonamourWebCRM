using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonamourWeb.Models;
using MonamourWeb.Services.Filters;

namespace MonamourWeb.Controllers
{
    [Authorize]
    public class PaymentTypesController : Controller
    {
        private readonly MonamourDataBaseContext _context;

        public PaymentTypesController(MonamourDataBaseContext context)
        {
            _context = context;
        }

        [UserRoleFilter]
        public IActionResult All()
        {
            var paymentTypes = _context.PaymentTypes;
            return View(paymentTypes);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PaymentType paymentType)
        {
            if (ModelState.IsValid)
            {
                _context.PaymentTypes.Add(paymentType);
                _context.SaveChanges();
                return RedirectToAction("All");
            }
            return View();
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var paymentType = _context.PaymentTypes.Find(id);
            
            if (paymentType == null)
                return NotFound();

            return View(paymentType);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePost(PaymentType paymentType)
        {
            var editedPaymentType = _context.PaymentTypes.Find(paymentType.Id);
            if (editedPaymentType == null)
                return NotFound();

            editedPaymentType.Type = paymentType.Type;

            _context.SaveChanges();
            return RedirectToAction("All");
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var paymentType = _context.PaymentTypes.Find(id);

            if (paymentType == null)
                return NotFound();

            return View(paymentType);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var paymentType = _context.PaymentTypes.Find(id);
            if (paymentType == null)
                return NotFound();
            _context.PaymentTypes.Remove(paymentType);
            _context.SaveChanges();
            return RedirectToAction("All");
        }
    }
}
