using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HandCarftBaseServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ComissionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILogHandler _logger;

        public ComissionController(IMapper mapper, IRepositoryWrapper repository, ILogHandler logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }


        /// <summary>
        /// لیست کمیسسون ها  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("Comission/GetComissionList")]
        public IActionResult GetComissionList()
        {
            try
            {
                var res = _repository.Comission.FindByCondition(c => c.DaDate == null && c.Ddate == null)
                    .Include(c => c.ProductComission).ThenInclude(c => c.Product)
                    .Select(c => new
                    {
                        c.Id,
                        c.Description,
                        c.Title,
                        c.SendEmail,
                        c.SendSms,
                        c.FinalStatusId,
                        c.Value,
                        productList = c.ProductComission.Select(x => string.Join("-", x.Product.Name))
                    })
                    .AsNoTracking().ToList();

                _logger.LogData(MethodBase.GetCurrentMethod(), res, null);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// لیست محصولات براساس کمیسیون آیدی  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("Comission/GetComissionProductById")]
        public IActionResult GetComissionProductById(long comissionId)
        {
            try
            {
                var res = _repository.ProductComission
                    .FindByCondition(c => c.DaDate == null && c.Ddate == null && c.Id == comissionId)
                    .Include(c => c.Product)
                    .Select(c => new { c.ProductId, c.Product.Name }).ToList();

                _logger.LogData(MethodBase.GetCurrentMethod(), res, null);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);
            }
        }
    }
}
