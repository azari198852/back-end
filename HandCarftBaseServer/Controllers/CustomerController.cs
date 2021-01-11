using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.Params;
using HandCarftBaseServer.Tools;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HandCarftBaseServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILogHandler _logger;

        public CustomerController(IMapper mapper, IRepositoryWrapper repository, ILogHandler logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// دریافت لیست مشتری با فیلتر  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("Customer/GetCustomerListByFilter")]
        public IActionResult GetCustomerListByFilter(CustomerListParam input)
        {
            try
            {
                var res = _repository.Customer.GetCustomerListByFilter(input);
                _logger.LogData(MethodBase.GetCurrentMethod(), res, null, input);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input);
                return BadRequest("");
            }
        }

        /// <summary>
        /// دریافت تاریخچه سفارشات مشتری  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("Customer/GetCustomerOrderHistory")]
        public IActionResult GetCustomerOrderHistory(long customerId)
        {
            try
            {
                var res = _repository.CustomerOrder.FindByCondition(c => c.CustomerId == customerId)
                    .Include(c => c.FinalStatus)
                    .Select(c => new { c.OrderNo, OrderDate = DateTimeFunc.TimeTickToMiladi(c.OrderDate.Value), c.FinalPrice, Status = c.FinalStatus.Name }).ToList();
                _logger.LogData(MethodBase.GetCurrentMethod(), res, null, customerId);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), customerId);
                return BadRequest(e.Message);
            }
        }
    }
}
