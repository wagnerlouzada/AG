// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace IdentityServer4.Quickstart.UI
{
    /// <summary>
    /// This sample controller allows a user to revoke grants given to clients
    /// </summary>
    [SecurityHeaders]
    [Authorize]
    public class ContentController : Controller
    {
        private readonly ControllerContext _context;

        public ContentController(ControllerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Show list of grants
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(string ContentName)
        {
            return View("Index", await BuildViewModelAsync(ContentName));
        }

        private async Task<ContentViewModel> BuildViewModelAsync(string ContentName)
        {
            //var content = configDbContext;

            return new ContentViewModel
            {
                //Content = list
            };
        }
    }
}
