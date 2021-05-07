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
using YSI.CurseOfSilverCrown.Core.EndOfTurn;
using YSI.CurseOfSilverCrown.Core.Actions;
using YSI.CurseOfSilverCrown.Core.Database.EF;
using YSI.CurseOfSilverCrown.Core.Database.Models;
using YSI.CurseOfSilverCrown.Core.ViewModels;
using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Helpers;
using YSI.CurseOfSilverCrown.Core.Commands;

namespace YSI.CurseOfSilverCrown.Web.Controllers
{
    public class CommandsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<HomeController> _logger;

        public CommandsController(ApplicationDbContext context, UserManager<User> userManager, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Commands
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetCurrentUser(HttpContext.User, _context);

            if (currentUser == null)
                return NotFound();
            if (string.IsNullOrEmpty(currentUser.OrganizationId))
                return RedirectToAction("Index", "Provinces");

            var organization = await _context.Organizations
                .Include(c => c.Vassals)
                .Include(c => c.Suzerain)
                .SingleAsync(o => o.Id == currentUser.OrganizationId);

            var commands = await _context.Commands
                .Include(c => c.Organization)
                .Include(c => c.Target)
                .Include(c => c.Target2)
                .Where(c => c.OrganizationId == currentUser.OrganizationId)
                .ToListAsync();

            ViewBag.Budget = new Budget(organization, commands);

            return View(commands);
        }

        // GET: Commands/Create
        public async Task<IActionResult> CreateAsync(int type)
        {
            var currentUser = await _userManager.GetCurrentUser(HttpContext.User, _context);

            if (currentUser == null)
                return NotFound();
            if (string.IsNullOrEmpty(currentUser.OrganizationId))
                return NotFound();

            ViewBag.Resourses = await FillResources(currentUser.OrganizationId);

            switch ((enCommandType)type)
            {
                case enCommandType.War:
                    return await WarAsync(null, currentUser.OrganizationId);
                case enCommandType.WarSupportDefense:
                    return await WarSupportDefenseAsync(null, currentUser.OrganizationId);
                case enCommandType.VassalTransfer:
                    return await VassalTransferAsync(null, currentUser.OrganizationId);
                case enCommandType.GoldTransfer:
                    return await GoldTransferAsync(null, currentUser.OrganizationId);
                default:
                    return NotFound();
            }
        }

        // POST: Commands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Type,TargetOrganizationId,Target2OrganizationId,Coffers,Warriors")] Command command)
        {
            var currentUser = await _userManager.GetCurrentUser(HttpContext.User, _context);

            if (currentUser == null)
                return NotFound();
            if (string.IsNullOrEmpty(currentUser.OrganizationId))
                return NotFound();

            if (new [] { enCommandType.War, enCommandType.WarSupportDefense, 
                enCommandType.VassalTransfer, enCommandType.GoldTransfer }.Contains(command.Type) && 
                command.TargetOrganizationId == null)
                return RedirectToAction("Index", "Commands");

            if (new[] { enCommandType.VassalTransfer }.Contains(command.Type) &&
                command.Target2OrganizationId == null)
                return RedirectToAction("Index", "Commands");

            command.OrganizationId = currentUser.OrganizationId;
            command.Id = Guid.NewGuid().ToString();

            if (ModelState.IsValid)
            {
                _context.Add(command);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var allOrganizations = await _context.Organizations
                .Include(o => o.Province)
                .Include(o => o.Vassals)
                .Where(o => o.OrganizationType == enOrganizationType.Lord)
                .ToListAsync();

            var userOrganization = allOrganizations.First(o => o.Id == currentUser.OrganizationId);
            var targetOrganizations = userOrganization.SuzerainId == null
                ? allOrganizations.Where(o => o.Id != currentUser.OrganizationId && !userOrganization.Vassals.Any(v => v.Id == o.Id))
                : allOrganizations.Where(o => o.Id == userOrganization.SuzerainId);

            ViewData["TargetOrganizationId"] = new SelectList(targetOrganizations, "Id", "Province.Name");
            return View(command);
        }



        // GET: Commands/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(string id, bool optimizeIdleness = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetCurrentUser(HttpContext.User, _context);
            var command = await _context.Commands
                .FirstOrDefaultAsync(o => o.Id == id);

            if (currentUser == null)
                return NotFound();
            if (string.IsNullOrEmpty(currentUser.OrganizationId))
                return NotFound();
            if (command == null)
                return NotFound();
            if (command.OrganizationId != currentUser.OrganizationId)
                return NotFound();

            ViewBag.Resourses = await FillResources(currentUser.OrganizationId, command.Id);

            switch (command.Type)
            {
                case enCommandType.Idleness:
                    return Idleness(command, optimizeIdleness);
                case enCommandType.Growth:
                    return Growth(command);
                case enCommandType.CollectTax:
                    return CollectTax(command);
                case enCommandType.War:
                    return await WarAsync(command, currentUser.OrganizationId);
                case enCommandType.Investments:
                    return Investments(command);
                case enCommandType.Fortifications:
                    return Fortifications(command);
                case enCommandType.WarSupportDefense:
                    return await WarSupportDefenseAsync(command, currentUser.OrganizationId);
                case enCommandType.VassalTransfer:
                    return await VassalTransferAsync(command, currentUser.OrganizationId);
                case enCommandType.GoldTransfer:
                    return await GoldTransferAsync(command, currentUser.OrganizationId);
                default:
                    return NotFound();
            }
        }

        private IActionResult Idleness(Command command, bool optimizeIdleness)
        {
            if (command == null || command.Type != enCommandType.Idleness)
            {
                return NotFound();
            }

            if (optimizeIdleness)
            {
                command.Coffers = IdlenessHelper.GetOptimizedCoffers();
                _context.Update(command);
                _context.SaveChangesAsync();
            }

            ViewBag.Optimized = IdlenessHelper.IsOptimized(command.Coffers);
            return View("Idleness", command);
        }

        private IActionResult Growth(Command command)
        {
            if (command == null || command.Type != enCommandType.Growth)
            {
                return NotFound();
            }

            return View("Growth", command);
        }

        private IActionResult CollectTax(Command command)
        {
            if (command == null || command.Type != enCommandType.CollectTax)
            {
                return NotFound();
            }

            return View("CollectTax", command);
        }

        private async Task<IActionResult> GoldTransferAsync(Command command, string userOrganizationId)
        {
            if (command != null && command.Type != enCommandType.GoldTransfer)
                return NotFound();

            var targetOrganizations = await GoldTransferHelper.GetAvailableTargets(_context, userOrganizationId, command);

            ViewBag.TargetOrganizations = OrganizationInfo.GetOrganizationInfoList(targetOrganizations);
            var defaultTargetId = command != null
                ? command.TargetOrganizationId
                : targetOrganizations.FirstOrDefault()?.Id;
            ViewData["TargetOrganizationId"] = new SelectList(targetOrganizations.OrderBy(o => o.Name), "Id", "Name", defaultTargetId);
            return View("GoldTransfer", command);
        }

        private async Task<IActionResult> WarAsync(Command command, string userOrganizationId)
        {
            if (command != null && command.Type != enCommandType.War)
                return NotFound();

            var targetOrganizations = await WarHelper.GetAvailableTargets(_context, userOrganizationId, command);

            ViewBag.TargetOrganizations = OrganizationInfo.GetOrganizationInfoList(targetOrganizations);
            var defaultTargetId = command != null
                ? command.TargetOrganizationId
                : targetOrganizations.FirstOrDefault()?.Id;
            ViewData["TargetOrganizationId"] = new SelectList(targetOrganizations.OrderBy(o => o.Name), "Id", "Name", defaultTargetId);
            return View("War", command);
        }        

        private async Task<IActionResult> WarSupportDefenseAsync(Command command, string userOrganizationId)
        {
            if (command != null && command.Type != enCommandType.WarSupportDefense)
            {
                return NotFound();
            }

            var targetOrganizations = await WarSupportDefenseHelper.GetAvailableTargets(_context, userOrganizationId, command);            

            ViewBag.TargetOrganizations = OrganizationInfo.GetOrganizationInfoList(targetOrganizations);
            var defaultTargetId = command != null
                ? command.TargetOrganizationId
                : targetOrganizations.Any(o => o.Id == userOrganizationId)
                    ? userOrganizationId
                    : targetOrganizations.FirstOrDefault()?.Id;
            ViewData["TargetOrganizationId"] = new SelectList(targetOrganizations.OrderBy(o => o.Name), "Id", "Name", defaultTargetId);
            return View("WarSupportDefense", command);
        }

        private async Task<IActionResult> VassalTransferAsync(Command command, string userOrganizationId)
        {
            if (command != null && command.Type != enCommandType.VassalTransfer)
            {
                return NotFound();
            }

            var targetOrganizations = await VassalTransferHelper.GetAvailableTargets(_context, userOrganizationId, command);
            var target2Organizations = await VassalTransferHelper.GetAvailableTargets2(_context, userOrganizationId, command);

            ViewBag.TargetOrganizations = OrganizationInfo.GetOrganizationInfoList(targetOrganizations);
            ViewBag.Target2Organizations = OrganizationInfo.GetOrganizationInfoList(target2Organizations);

            var userOrganization = target2Organizations.First(o => o.Id == userOrganizationId);

            var defaultTargetId = command != null
                ? command.TargetOrganizationId
                : targetOrganizations.FirstOrDefault()?.Id;

            var defaultTarget2Id = command != null
                ? command.Target2OrganizationId
                : userOrganization.SuzerainId != null
                    ? userOrganization.SuzerainId
                    : targetOrganizations.FirstOrDefault()?.Id;

            ViewData["TargetOrganizationId"] = new SelectList(targetOrganizations.OrderBy(o => o.Name), "Id", "Name", defaultTargetId);
            ViewData["Target2OrganizationId"] = new SelectList(target2Organizations.OrderBy(o => o.Name), "Id", "Name", defaultTarget2Id);
            return View("VassalTransfer", command);
        }

        private IActionResult Investments(Command command)
        {
            if (command == null || command.Type != enCommandType.Investments)
            {
                return NotFound();
            }

            return View("Investments", command);
        }

        private IActionResult Fortifications(Command command)
        {
            if (command == null || command.Type != enCommandType.Fortifications)
            {
                return NotFound();
            }

            return View("Fortifications", command);
        }

        // POST: Commands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Type,TargetOrganizationId,Target2OrganizationId,Coffers,Warriors")] Command command)
        {
            if (id != command.Id)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetCurrentUser(HttpContext.User, _context);
            var realCommand = await _context.Commands
                .FirstOrDefaultAsync(o => o.Id == id);

            if (currentUser == null)
                return NotFound();
            if (string.IsNullOrEmpty(currentUser.OrganizationId))
                return NotFound();
            if (realCommand == null)
                return NotFound();
            if (realCommand.OrganizationId != currentUser.OrganizationId)
                return NotFound();

            realCommand.Coffers = command.Coffers;
            realCommand.Warriors = command.Warriors;
            realCommand.Type = command.Type;
            realCommand.TargetOrganizationId = command.TargetOrganizationId;
            realCommand.Target2OrganizationId = command.Target2OrganizationId;

            try
            {
                _context.Update(realCommand);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommandExists(realCommand.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        // GET: Commands/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetCurrentUser(HttpContext.User, _context);

            var command = await _context.Commands
                .Include(c => c.Organization)
                .Include(c => c.Target)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (command == null || command.OrganizationId != currentUser.OrganizationId)
            {
                return NotFound();
            }

            _context.Commands.Remove(command);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommandExists(string id)
        {
            return _context.Commands.Any(e => e.Id == id);
        }
        
        private async Task<Dictionary<string, List<int>>> FillResources(string organizationId, string withoutCommandId = null)
        {
            var organization = await _context.Organizations
                .Include(o => o.Commands)
                .SingleAsync(o => o.Id == organizationId);

            var dictionary = new Dictionary<string, List<int>>();
            var busyCoffers = organization.Commands
                .Where(c => string.IsNullOrEmpty(withoutCommandId) || c.Id != withoutCommandId)
                .Where(c => c.Type != enCommandType.Idleness)
                .Sum(c => c.Coffers);
            var busyWarriors = organization.Commands
                .Where(c => string.IsNullOrEmpty(withoutCommandId) || c.Id != withoutCommandId)
                .Sum(c => c.Warriors);
            dictionary.Add("Казна", new List<int>(3)
            { 
                organization.Coffers,
                busyCoffers,
                organization.Coffers - busyCoffers
            });
            dictionary.Add("Воины", new List<int>(3)
            {
                organization.Warriors,
                busyWarriors,
                organization.Warriors - busyWarriors
            });
            dictionary.Add("Инвестиции", new List<int>(3)
            {
                organization.Investments,
                0,
                organization.Investments
            }); 
            dictionary.Add("Укрепления", new List<int>(3)
            {
                organization.Fortifications,
                0,
                organization.Fortifications
            });
            return dictionary;
        }
    }
}
