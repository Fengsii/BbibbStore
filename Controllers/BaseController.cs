﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    
    public class BaseController : Controller
    {
        //protected bool IsUserLoggedIn()
        //{
        //    return HttpContext.User.Identity.IsAuthenticated &&
        //          HttpContext.Session.GetString("UserId") != null;
        //}

        //protected int GetCurrentUserId()
        //{
        //    var userIdString = HttpContext.Session.GetString("UserId");
        //    return !string.IsNullOrEmpty(userIdString) ? int.Parse(userIdString) : 0;
        //}

        protected bool IsUserLoggedIn()
        {
            return HttpContext.User.Identity.IsAuthenticated && HttpContext.Session.GetInt32("UserId") != null;
        }

        protected int GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }

        protected string GetCurrentUserRole()
        {
            return HttpContext.Session.GetString("UserRole") ?? string.Empty;
        }

        protected string GetCurrentUserName()
        {
            return HttpContext.Session.GetString("UserName") ?? string.Empty;
        }

        protected bool IsAdmin()
        {
            return GetCurrentUserRole() == "Admin";
        }

        protected bool IsUser()
        {
            return GetCurrentUserRole() == "User";
        }


    }
}
