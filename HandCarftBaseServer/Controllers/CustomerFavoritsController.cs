using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using HandCarftBaseServer.Tools;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HandCarftBaseServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CustomerFavoritsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILogHandler _logger;

        public CustomerFavoritsController(IMapper mapper, IRepositoryWrapper repository, ILogHandler logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// دریافت لیست علاقه مندی ها  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("CustomerFavorits/GetCustomerFavorites")]
        public IActionResult GetCustomerFavorites()
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var customerId = _repository.Customer.FindByCondition(c => c.UserId == userId).Select(c => c.Id)
                    .FirstOrDefault();

                var res = _repository.CustomerFavoriteProducts.FindByCondition(c => c.CustomerId == customerId)
                    .Include(c => c.Product)
                    .GroupBy(c => new { c.ProductId, c.Product.Name, c.Product.Coding })
                    .Select(c => new { c.Key.Name, c.Key.Coding, c.Key.ProductId, Count = c.Count() }).ToList();

               

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
        /// دریافت لیست مشتریان علاقه مند به محصول  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("CustomerFavorits/GetCustomerListByFavorite")]
        public IActionResult GetCustomerListByFavorite(long productId)
        {
            try
            {
               
     

                var res = _repository.CustomerFavoriteProducts.FindByCondition(c => c.ProductId == productId)
                    .Include(c => c.Customer)
                    .Select(c => new { c.Customer.Id, c.Customer.Name , c.Customer.Fname , c.Customer.Email, c.Customer.Mobile }).ToList();



                _logger.LogData(MethodBase.GetCurrentMethod(), res, null, productId);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), productId);
                return BadRequest(e.Message);
            }
        }
    }
}
