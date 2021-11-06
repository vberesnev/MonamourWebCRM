using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonamourWeb.Models;
using MonamourWeb.Services.Filters;
using MonamourWeb.Services.Logs;

namespace MonamourWeb.Controllers
{
    [Authorize]
    public class PaymentTypesController : BaseController
    {
        public PaymentTypesController(MonamourDataBaseContext context, ILogService logService)
            : base(context, logService)
        {
        }

        [UserRoleFilter]
        public IActionResult All()
        {
            var paymentTypes = Context.PaymentTypes;
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
        public async Task<IActionResult> Create(PaymentType paymentType)
        {
            if (ModelState.IsValid)
            {
                Context.PaymentTypes.Add(paymentType);
                await Context.SaveChangesAsync();
                await LogService.AddCreationLogAsync<PaymentType>(paymentType, UserId);
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

            var paymentType = Context.PaymentTypes.Find(id);
            
            if (paymentType == null)
                return NotFound();

            return View(paymentType);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PaymentType paymentType)
        {
            if (ModelState.IsValid)
            {
                var editedPaymentType = await Context.PaymentTypes.FindAsync(paymentType.Id);
                if (editedPaymentType == null)
                    return NotFound();
                var oldPaymentType = editedPaymentType.Clone() as PaymentType;
                editedPaymentType.Type = paymentType.Type;

                await Context.SaveChangesAsync();
                await LogService.AddUpdatedLogAsync<PaymentType>(oldPaymentType, editedPaymentType, UserId);
                return RedirectToAction("All");
            }
            return Update(paymentType.Id);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var paymentType = Context.PaymentTypes.Find(id);

            if (paymentType == null)
                return NotFound();

            return View(paymentType);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var paymentType = await Context.PaymentTypes.FindAsync(id);
            if (paymentType == null)
                return NotFound();
            Context.PaymentTypes.Remove(paymentType);
            await Context.SaveChangesAsync();
            await LogService.AddDeletedLogAsync<PaymentType>(paymentType, UserId);
            return RedirectToAction("All");
        }
    }
}
