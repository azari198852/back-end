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
using Entities.DataTransferObjects;
using Entities.Models;
using HandCarftBaseServer.Tools;

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

        /// <summary>
        /// ثبت کمیسیون
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("Comission/InsertComission")]
        public IActionResult InsertComission(InsertComissionDto input)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);

                #region Validation
                var validator = new ParamValidator();
                validator.ValidateNull(input.Title, General.Messages_.NullInputMessages_.GeneralNullMessage("عنوان"))
                    .ValidateNull(input.Value, General.Messages_.NullInputMessages_.GeneralNullMessage("مقدار کمیسیون"))
                    .Throw(General.Results_.FieldNullErrorCode());
                if (input.Value > 70)
                    throw new BusinessException("مقدار کمیسیون نباید بیشتر از 70 باشد.", 1001);
                if (input.ProductIdList.Count == 0)
                    throw new BusinessException("محصولی انتخاب نشده است.", 1001);

                #endregion

                var productIdList = new List<Product>();

                var comission = new Comission
                {
                    Cdate = DateTime.Now.Ticks,
                    CuserId = userId,
                    SendEmail = input.SendEmail,
                    SendSms = input.SendSms,
                    Title = input.Title,
                    Value = input.Value,
                    ProductComission = new List<ProductComission>()

                };

                input.ProductIdList.ForEach(c =>
                {
                    #region Change Product Status

                    var product = _repository.Product.FindByCondition(x => x.Id == c).Include(c=>c.Seller).FirstOrDefault();
                    if (product == null)
                        throw new BusinessException("کد محصولات صحیح نیست", 1001);
                    productIdList.Add(product);
                    product.FinalStatusId = 7;
                    _repository.Product.Update(product);

                    #endregion

                    #region Deactive Previous Comission

                    var _productComission = _repository.ProductComission
                        .FindByCondition(x => x.ProductId == c && x.DaDate == null).FirstOrDefault();
                    if (_productComission != null)
                    {
                        _productComission.DaDate = DateTime.Now.Ticks;
                        _productComission.DaUserId = userId;
                        _repository.ProductComission.Update(_productComission);
                    }

                    #endregion


                    var productComission = new ProductComission
                    {
                        CuserId = userId,
                        Cdate = DateTime.Now.Ticks,
                        ProductId = c,

                    };
                    comission.ProductComission.Add(productComission);

                });

                #region SendSms

                if (input.SendSms)
                {

                    productIdList.ForEach(c =>
                    {
                        var sms = new SendSMS();
                        sms.SendChangeComissionSms(c.Seller.Mobile.Value, c.Seller.Name + " " + c.Seller.Fname, c.Name, comission.Value.Value);
                    });

                }

                #endregion

                #region SendEmail

                if (input.SendEmail)
                {

                    productIdList.ForEach(c =>
                    {
                        var email = new SendEmail();
                        email.SendChangeComissionEmail(c.Seller.Email, c.Seller.Name + " " + c.Seller.Fname, c.Name, comission.Value.Value);
                    });
                }


                #endregion

                _repository.Comission.Create(comission);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), comission.Id, null, input);
                return Ok(comission.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// حذف کمیسیون
        /// </summary>
        [Authorize]
        [HttpDelete]
        [Route("Comission/DeleteComission")]
        public IActionResult DeleteComission(long comissionId)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var comission = _repository.Comission.FindByCondition(c => c.Id == comissionId).FirstOrDefault();
                if (comission == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                comission.DaDate = DateTime.Now.Ticks;
                comission.DuserId = userId;
                _repository.Comission.Update(comission);

                var productComissionList = _repository.ProductComission
                    .FindByCondition(c => c.ComissionId == comissionId).ToList();

                productComissionList.ForEach(c =>
                {
                    c.Ddate = DateTime.Now.Ticks;
                    c.DuserId = userId;
                    _repository.ProductComission.Update(c);
                });
                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, comissionId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), comissionId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// دریافت سابقه کمیسیون محصول
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("Comission/GetProductComissionHistory")]
        public IActionResult GetProductComissionHistory(long productId)
        {
            try
            {

                var productComissionList = _repository.ProductComission
                    .FindByCondition(c => c.ProductId == productId).Include(c => c.Comission)
                    .Select(c => new { c.Comission.Title, c.Comission.Value }).ToList();


                _logger.LogData(MethodBase.GetCurrentMethod(), productComissionList, null, productId);
                return Ok(productComissionList);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), productId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// ویرایش کمیسیون
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("Comission/UpdateComission")]
        public IActionResult UpdateComission(InsertComissionDto input, long comissionId)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);

                #region Validation
                var validator = new ParamValidator();
                validator.ValidateNull(input.Title, General.Messages_.NullInputMessages_.GeneralNullMessage("عنوان"))
                    .ValidateNull(input.Value, General.Messages_.NullInputMessages_.GeneralNullMessage("مقدار کمیسیون"))
                    .Throw(General.Results_.FieldNullErrorCode());
                if (input.Value > 70)
                    throw new BusinessException("مقدار کمیسیون نباید بیشتر از 70 باشد.", 1001);
                if (input.ProductIdList.Count == 0)
                    throw new BusinessException("محصولی انتخاب نشده است.", 1001);

                #endregion

                var productIdList = new List<Product>();

                var comission = _repository.Comission.FindByCondition(c => c.Id == comissionId).FirstOrDefault();
                if (comission == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                comission.Cdate = DateTime.Now.Ticks;
                comission.CuserId = userId;
                comission.SendEmail = input.SendEmail;
                comission.SendSms = input.SendSms;
                comission.Title = input.Title;
                comission.Value = input.Value;

                var deletedProductComission = _repository.ProductComission
                    .FindByCondition(c => c.ComissionId == comissionId).ToList();

                _repository.ProductComission.DeleteRange(deletedProductComission);



                input.ProductIdList.ForEach(c =>
                {
                    #region Change Product Status

                    var product = _repository.Product.FindByCondition(x => x.Id == c).Include(c => c.Seller).FirstOrDefault();
                    if (product == null)
                        throw new BusinessException("کد محصولات صحیح نیست", 1001);
                    productIdList.Add(product);
                    product.FinalStatusId = 7;
                    _repository.Product.Update(product);

                    #endregion

                    var productComission = new ProductComission
                    {
                        CuserId = userId,
                        Cdate = DateTime.Now.Ticks,
                        ProductId = c,

                    };
                    comission.ProductComission.Add(productComission);

                });

                #region SendSms

                if (input.SendSms)
                {

                    productIdList.ForEach(c =>
                    {
                        var sms = new SendSMS();
                        sms.SendChangeComissionSms(c.Seller.Mobile.Value, c.Seller.Name + " " + c.Seller.Fname, c.Name, comission.Value.Value);
                    });

                }

                #endregion

                #region SendEmail

                if (input.SendEmail)
                {

                    productIdList.ForEach(c =>
                    {
                        var email = new SendEmail();
                        email.SendChangeComissionEmail(c.Seller.Email, c.Seller.Name + " " + c.Seller.Fname, c.Name, comission.Value.Value);
                    });
                }


                #endregion

                _repository.Comission.Create(comission);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, input, comissionId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input, comissionId);
                return BadRequest(e.Message);
            }
        }
    }
}
