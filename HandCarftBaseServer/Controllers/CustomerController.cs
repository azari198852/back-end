using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.Models;
using Entities.Params;
using Entities.UIResponse;
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

        /// <summary>
        /// دریافت شماره موبایل مشتری  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("Customer/GetCustomerMobileNo")]
        public IActionResult GetCustomerMobileNo()
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var res = _repository.Customer.FindByCondition(c => c.UserId == userId).Include(c => c.UserId)
                    .Select(c => c.User.Mobile).FirstOrDefault();

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
        /// دریافت وضعیت کیف پول مشتری  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("Customer/GetCustomerHaveWallet")]
        public IActionResult GetCustomerHaveWallet()
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var res = _repository.Customer.FindByCondition(c => c.UserId == userId)
                    .Select(c => c.HaveWallet == true ? "فعال" : "غیر فعال").FirstOrDefault();

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
        /// دریافت موجودی کیف پول مشتری  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("Customer/GetCustomerWalletRemind")]
        public IActionResult GetCustomerWalletRemind()
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var res = _repository.Customer.FindByCondition(c => c.UserId == userId)
                    .Select(c => c.WalletFinalPrice).FirstOrDefault();

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
        /// ارسال کد فعالسازی کیف پول مشتری  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("Customer/SendCustomerWalletActivationCode")]
        public VoidResult SendCustomerWalletActivationCode()
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var user = _repository.Users.FindByCondition(c => c.Id == userId).FirstOrDefault();

                var now = DateTime.Now.Ticks;
                if (_repository.UserActivation
                    .FindByCondition(c => c.UserId == userId && c.EndDateTime > now && c.LoginType == 3).Any())
                {
                    var ress = VoidResult.GetFailResult("کد فعالسازی قبلا برای شما ارسال گردیده است.");
                    _logger.LogData(MethodBase.GetCurrentMethod(), ress, null);
                    return ress;
                }

                var random = new Random();
                var code = random.Next(1000, 9999);

                user.UserActivation.Add(new UserActivation
                {
                    SendedCode = code,
                    EndDateTime = DateTime.Now.AddMinutes(2).Ticks,
                    Cdate = DateTime.Now.Ticks,
                    LoginType = 3,
                    UserId = userId


                });

                var sms = new SendSMS();
                var bb = sms.SendWalletActivationSms(user.Mobile.Value, code, user.FullName);
                var finalres = VoidResult.GetSuccessResult("کد فعال سازی برای کیف پول ، با موفقیت ارسال شد");
                _logger.LogData(MethodBase.GetCurrentMethod(), finalres, null);
                return finalres;

            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return VoidResult.GetFailResult(e.Message);
            }
        }


        /// <summary>
        ///بررسی صحت کدفعالسازی کیف پول  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("Customer/CheckWalletActivationCode")]
        public VoidResult CheckWalletActivationCode(int code)
        {

            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var user = _repository.Users.FindByCondition(c => c.Id == userId).FirstOrDefault();

                var now = DateTime.Now.Ticks;
                var s = _repository.UserActivation.FindByCondition(c =>
                    c.UserId == user.Id && c.EndDateTime > now && c.LoginType == 3 && c.SendedCode == code).FirstOrDefault();
                if (s == null)
                {
                    var ress = VoidResult.GetFailResult("کد وارد شده جهت تغییر کلمه عبور صحیح نمی باشد.");
                    _logger.LogData(MethodBase.GetCurrentMethod(), ress, null, code);
                    return ress;

                }

                var customer = _repository.Customer.FindByCondition(c => c.UserId == userId).FirstOrDefault();
                customer.HaveWallet = true;

                _repository.Customer.Update(customer);
                _repository.Save();
                var finalres = VoidResult.GetSuccessResult("کیف پول با موفقیت فعال شد .");
                _logger.LogData(MethodBase.GetCurrentMethod(), finalres, null, code);
                return finalres;


            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), code);
                return VoidResult.GetFailResult(e.Message);
            }


        }


    }
}
