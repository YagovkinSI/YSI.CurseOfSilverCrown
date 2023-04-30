﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using YSI.CurseOfSilverCrown.Core.APIModels;
using YSI.CurseOfSilverCrown.Core.Database;
using YSI.CurseOfSilverCrown.Core.Helpers;

namespace YSI.CurseOfSilverCrown.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiMapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApiMapController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("getMap")]
        public ActionResult<List<MapElement>> GetMap()
        {
            try
            {
                var mapElements = MapHelper.GetMap(_context);
                return Ok(mapElements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
