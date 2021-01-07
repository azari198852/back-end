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
    public class ProductCustomerRateController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILogHandler _logger;

        public ProductCustomerRateController(IMapper mapper, IRepositoryWrapper repository, ILogHandler logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// لیست نظرات کاربران  
        /// </summary>
        [HttpGet]
        [Route("ProductCustomerRate/GetCustomerRateList")]
        public IActionResult GetCustomerRateList()
        {
            try
            {

                var res = _repository.ProductCustomerRate
                      .FindByCondition(c => c.Ddate == null && c.DaDate == null && c.Pid == null)
                      .Include(c => c.Product)
                      .Include(c => c.Customer)
                      .Include(c => c.FinalStatus)
                      .Select(c => new
                      {
                          c.Id,
                          c.Rate,
                          c.CommentDesc,
                          CommentDate = DateTimeFunc.TimeTickToMiladi(c.CommentDate.Value),
                          ProductName = c.Product.Name,
                          ProductCoding = c.Product.Coding,
                          CustomerName = c.Customer.Name + " " + c.Customer.Fname,
                          c.FinalStatusId,
                          Status = c.FinalStatus.Name
                      }).ToList();

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
        /// تایید نظر کاربر  
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("ProductCustomerRate/ConfirmCustomerRate")]
        public IActionResult ConfirmCustomerRate(long productCustomerRateId)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var res = _repository.ProductCustomerRate
                    .FindByCondition(c => c.Id == productCustomerRateId).FirstOrDefault();
                if (res == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                res.FinalStatusId = 30;
                res.CheckUserId = userId;
                res.CheckDate = DateTime.Now.Ticks;
                _repository.ProductCustomerRate.Update(res);
                _repository.Save();


                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, productCustomerRateId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), productCustomerRateId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// پاسخ به نظر کاربر  
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("ProductCustomerRate/ReplyToCustomerRate")]
        public IActionResult ReplyToCustomerRate(long productCustomerRateId,string answerString)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var res = _repository.ProductCustomerRate
                    .FindByCondition(c => c.Id == productCustomerRateId).FirstOrDefault();
                if (res == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                res.FinalStatusId = 30;
                res.CheckUserId = userId;
                res.CheckDate = DateTime.Now.Ticks;
                res.CheckAnswer = answerString;
                _repository.ProductCustomerRate.Update(res);
                _repository.Save();


                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, productCustomerRateId, answerString);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), productCustomerRateId, answerString);
                return BadRequest(e.Message);
            }
        }
    }
}
