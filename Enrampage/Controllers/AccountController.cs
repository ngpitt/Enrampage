using Enrampage.Extensions;
using Enrampage.Helpers;
using Enrampage.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Enrampage.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet]
        public ActionResult Login(string ReturnUrl)
        {
            if (Request.IsAuthenticated)
            {
                return Redirect(ReturnUrl);
            }

            return new ChallengeResult(Url.Action("Callback", "Account", new { ReturnUrl = ReturnUrl }));
        }

        [HttpGet]
        public ActionResult Callback(string ReturnUrl)
        {
            try
            {
                var loginInfo = AuthenticationManager.GetExternalLoginInfo();

                if (loginInfo == null)
                {
                    TempData["Error"] = "Failed to login.";
                    return RedirectToAction("Index", "Home");
                }

                using (var context = new EnrampageEntities())
                {
                    var user = context.Users.FirstOrDefault(b => b.Email == loginInfo.Email);

                    if (user == null)
                    {
                        user = new User
                        {
                            Email = loginInfo.Email,
                            Admin = false,
                            Banned = false
                        };
                        context.Users.Add(user);
                        context.SaveChanges();
                    }
                    else if (user.Banned)
                    {
                        TempData["Error"] = "Your account has been banned.";
                        return RedirectToAction("Index", "Home");
                    }

                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Admin ? "Admin" : "User")
                    };
                    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignIn(new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = true,
                    }, identity);
                }
            }
            catch (Exception Ex)
            {
                LogHelper.Log(Ex);
                TempData["Error"] = "Failed to login.";
            }

            TempData["Success"] = "Logged in successfully.";
            return Redirect(ReturnUrl);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Logout(string ReturnUrl)
        {
            try
            {
                AuthenticationManager.SignOut(
                    DefaultAuthenticationTypes.ApplicationCookie,
                    DefaultAuthenticationTypes.ExternalCookie);
            }
            catch (Exception Ex)
            {
                LogHelper.Log(Ex);
                TempData["Error"] = "Failed to logout.";
            }

            TempData["Success"] = "Logged out successfully.";
            return Redirect(ReturnUrl);
        }

        [HttpGet]
        [Authorize]
        public ActionResult BanUser(int Id)
        {
            try
            {
                if (!CurrentUser.Admin())
                {
                    TempData["Error"] = "You are not an administrator.";
                    return RedirectToAction("Index", "Home");
                }

                using (var context = new EnrampageEntities())
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == Id);

                    if (user == null)
                    {
                        TempData["Error"] = "User does not exist.";
                        return RedirectToAction("Index", "Home");
                    }

                    if (user.Banned)
                    {
                        TempData["Error"] = "User already banned.";
                        return RedirectToAction("Index", "Home");
                    }

                    user.Banned = true;
                    context.SaveChanges();
                }

                TempData["Success"] = "User banned successfully.";
            }
            catch (Exception Ex)
            {
                LogHelper.Log(Ex);
                TempData["Error"] = "Failed to ban user.";
            }

            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }
    }

    internal class ChallengeResult : HttpUnauthorizedResult
    {
        private string RedirectUri;

        public ChallengeResult(string RedirectUri)
        {
            this.RedirectUri = RedirectUri;
        }

        public override void ExecuteResult(ControllerContext Context)
        {
            var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
            Context.HttpContext.GetOwinContext().Authentication.Challenge(properties, "Google");
        }
    }
}