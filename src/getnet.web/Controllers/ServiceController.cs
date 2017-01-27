using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using getnet.Model;
using getnet.Client;

namespace getnet.Controllers
{
    [Authorize(Roles = Roles.GlobalAdmins)]
    public class ServiceController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Configure()
        {
            return View();
        }

        public IActionResult NewKeys()
        {
            return Json(new
            {
                AdminKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                ReadKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            });
        }

        [HttpPost]
        public IActionResult SetUrl(string Url, string DefaultKey)
        {
            if (!Url.HasValue())
                ModelState.AddModelError("Url", "Url required");
            Uri result;
            if (!Uri.TryCreate(Url, UriKind.Absolute, out result))
                ModelState.AddModelError("Url", "Url is invalid");
            if (!DefaultKey.HasValue())
                ModelState.AddModelError("DefaultKey", "Default Key is required");

            if (ModelState.IsValid)
            {
                CoreCurrent.Configuration.Set("Data:GetNetService:Url", Url);
                CoreCurrent.Configuration.SetSecure("Api:Keys:Default", DefaultKey);
                HttpContext.Session.AddSnackMessage("Successfully set the url and default key values");
            }
            return RedirectToAction("Configure");
        }

        [HttpPost]
        public IActionResult SetKeys(string AdminKey, string ReadKey)
        {
            ServiceClient client;
            if (CoreCurrent.Configuration.GetSecure("Api:Keys:Admin").HasValue())
                client = new Client.ServiceClient(ApiKeyType.Admin);
            else if (CoreCurrent.Configuration.GetSecure("Api:Keys:Default").HasValue())
                client = new Client.ServiceClient(ApiKeyType.Default);
            else
            {
                HttpContext.Session.AddSnackMessage("No API key available to communicate with service");
                return RedirectToAction("Configure");
            }

            if (!AdminKey.HasValue())
                ModelState.AddModelError("AdminKey", "Admin Key required");
            if (!ReadKey.HasValue())
                ModelState.AddModelError("ReadKey", "Read Key required");

            if (ModelState.IsValid)
            {
                string result;
                try
                {
                    result = client.SetKeys(AdminKey, ReadKey);
                    CoreCurrent.Configuration.SetSecure("Api:Keys:Read", ReadKey);
                    CoreCurrent.Configuration.SetSecure("Api:Keys:Admin", AdminKey);
                } catch (Exception ex)
                {
                    result = ex.Message;
                }
                HttpContext.Session.AddSnackMessage(result);
            }
            return RedirectToAction("Configure");
        }

        public IActionResult RunJob(string id) {
            var client = new ServiceClient(ApiKeyType.Admin);
            client.RunJob(id);
            HttpContext.Session.AddSnackMessage("Job '{0}' initiated", id);
            return RedirectToAction("Index");
        }
    }
}
