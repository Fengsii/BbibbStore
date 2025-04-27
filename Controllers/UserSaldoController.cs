using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    public class UserSaldoController : BaseController
    {
        private readonly IUserSaldo _userSaldo;
        private readonly IAuthentication _authentication;

        public UserSaldoController (IUserSaldo userSaldo, IAuthentication authentication)
        {
            _userSaldo = userSaldo;
            _authentication = authentication;
        }


        // GET: ProductController
        public ActionResult Index()
        {
            var data = _userSaldo.GetListUserSaldo();
            return View(data);
        }



        public IActionResult Edit(int id)
        {
            ViewBag.User = _authentication.Users();
            var data = _userSaldo.GetUserSaldoById(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(UserSaldoDTO userSaldo)
        {
            if (userSaldo.Id == 0)
            {
                var data = _userSaldo.AddUserSaldo(userSaldo);
                if (data)
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            else
            {
                var data = _userSaldo.EditUserSaldo(userSaldo);
                if (data)
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            return View();

        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var data = _userSaldo.DeleteUserSaldo(id);
            if (data)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("Gagal menghapus produk.");
        }

    }
}
