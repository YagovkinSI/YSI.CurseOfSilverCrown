﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YSI.CurseOfSilverCrown.Web.BL.EndOfTurn;
using YSI.CurseOfSilverCrown.Web.BL.EndOfTurn.Event;
using YSI.CurseOfSilverCrown.Web.Data;
using YSI.CurseOfSilverCrown.Core.Database.Models;

namespace YSI.CurseOfSilverCrown.Web.Controllers
{
    public class OrganizationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<HomeController> _logger;

        public OrganizationsController(ApplicationDbContext context, UserManager<User> userManager, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Organizations/My
        [Authorize]
        public async Task<IActionResult> My()
        {
            var currentUser = await _userManager.GetCurrentUser(HttpContext.User, _context);

            if (currentUser == null)
                return NotFound();
            if (string.IsNullOrEmpty(currentUser.OrganizationId))
                return RedirectToAction("Index", "Provinces");

            var organisation = await _context.Organizations
                .Include(o => o.Suzerain)
                .Include(o => o.Vassals)
                .Include(o => o.Commands)
                .Include("Commands.Target")
                .SingleAsync(o => o.Id == currentUser.OrganizationId);

            var organizationEventStories = await _context.OrganizationEventStories
                .Include(o => o.EventStory)
                .Include("EventStory.Turn")
                .Where(o => o.OrganizationId == organisation.Id)
                .OrderByDescending(o => o.EventStoryId)
                .OrderByDescending(o => o.TurnId)
                .Take(20)
                .ToListAsync();

            var eventStories = organizationEventStories
                .Select(o => o.EventStory)
                .ToList();

            ViewBag.LastEventStories = await EventStoryHelper.GetTextStories(_context, eventStories);
            return View(organisation);
        }

        // GET: Organizations/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var organisation = await _context.Organizations
                .Include(o => o.User)
                .Include(o => o.Province)
                .Include(o => o.Suzerain)
                .Include(o => o.Vassals)
                .SingleAsync(o => o.Id == id);

            var organizationEventStories = await _context.OrganizationEventStories
                .Include(o => o.EventStory)
                .Include("EventStory.Turn")
                .Where(o => o.OrganizationId == organisation.Id)
                .OrderByDescending(o => o.EventStoryId)
                .OrderByDescending(o => o.TurnId)
                .Take(20)
                .ToListAsync();

            var eventStories = organizationEventStories
                .Select(o => o.EventStory)
                .ToList();

            ViewBag.LastEventStories = await EventStoryHelper.GetTextStories(_context, eventStories);

            return View(organisation);
        }
    }
}
