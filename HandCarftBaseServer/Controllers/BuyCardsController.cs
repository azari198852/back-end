using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Params;
using HandCarftBaseServer.Tools;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HandCarftBaseServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class BuyCardsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILogHandler _logger;

        public BuyCardsController(IMapper mapper, IRepositoryWrapper repository, ILogHandler logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// دریافت لیست کارت های خرید  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("BuyCards/GetBuyCardsList")]
        public IActionResult GetBuyCardsList()
        {
            try
            {
                var res = _repository.Product.FindByCondition(c => c.CatProduct.Rkey == 1 && c.Ddate == null)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Count,
                        c.Price,
                        Status = c.DaDate == null ? "فعال" : "غیرفعال"
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
        /// فعال / غیر فعال کردن کارت های خرید  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("BuyCards/ActiveDeactiveBuyCards")]
        public IActionResult ActiveDeactiveBuyCards(long productId)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var product = _repository.Product.FindByCondition(c => c.Id == productId).FirstOrDefault();
                if (product == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                if (product.DaDate == null)
                {
                    product.DaDate = DateTime.Now.Ticks;
                    product.DaUserId = userId;
                }
                else
                {
                    product.DaDate = null;
                }
                _repository.Product.Update(product);
                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, productId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), productId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// سابقه خرید کارت های خرید  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("BuyCards/BuyCardBoughtHistory")]
        public IActionResult BuyCardBoughtHistory(long productId)
        {
            try
            {

                var res = _repository.CustomerOrderProduct.FindByCondition(c => c.ProductId == productId && c.CustomerOrder.CustomerOrderPayment.Any(x => x.FinalStatusId == 25))
                    .Include(c => c.CustomerOrder).ThenInclude(c => c.Customer)
                    .Select(c => new
                    {
                        CustomerName = c.CustomerOrder.Customer.Name + c.CustomerOrder.Customer.Name,
                        OrderDate = DateTimeFunc.TimeTickToMiladi(c.CustomerOrder.OrderDate.Value),
                        c.CustomerOrder.OrderNo

                    }).ToList();


                _logger.LogData(MethodBase.GetCurrentMethod(), res, null, productId);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), productId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// ثبت کردن کارت خرید  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("BuyCards/InsertBuyCard")]
        public IActionResult InsertBuyCard()
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);


                var buycard = JsonSerializer.Deserialize<InsertBuyCardDto>(HttpContext.Request.Form["BuyCard"]);
                var releventList = JsonSerializer.Deserialize<List<long>>(HttpContext.Request.Form["releventList"]);

                var product = new Product
                {
                    Cdate = DateTime.Now.Ticks,
                    CuserId = userId,
                    FinalStatusId = 10,
                    CatProductId = _repository.CatProduct
                        .FindByCondition(c => c.Rkey == 1 && c.DaDate == null && c.Ddate == null).Select(c => c.Id)
                        .FirstOrDefault(),
                    Description = buycard.Description,
                    Count = buycard.FirstCount,
                    FirstCount = buycard.FirstCount,
                    KeyWords = buycard.KeyWords,
                    MetaDescription = buycard.MetaDescription,
                    MetaTitle = buycard.MetaTitle,
                    Name = buycard.Name,
                    Price = buycard.Price,
                    LastSeenDate = DateTime.Now.Ticks,
                    SeenCount = 0
                };


                releventList.ForEach(c =>
                {
                    var pro = new RelatedProduct
                    {
                        CuserId = ClaimPrincipalFactory.GetUserId(User),
                        Cdate = DateTime.Now.Ticks,
                        DestinProductId = c
                    };
                    product.RelatedProductOriginProduct.Add(pro);

                });


                var fileList = HttpContext.Request.Form.Files.ToList();

                foreach (var uploadFileStatus in fileList.Select(file => FileManeger.FileUploader(file, 1, "BuyCardImages")))
                {
                    if (uploadFileStatus.Status == 200)
                    {
                        var image = new ProductImage
                        {
                            Cdate = DateTime.Now.Ticks,
                            CuserId = userId,
                            ImageUrl = uploadFileStatus.Path,

                        };
                        product.ProductImage.Add(image);
                    }
                    else
                    {
                        throw new BusinessException(uploadFileStatus.Path, 100);
                    }
                }

                _repository.Product.Create(product);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), product.Id, null);
                return Ok(product.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// حذف کردن کارت خرید  
        /// </summary>
        [Authorize]
        [HttpDelete]
        [Route("BuyCards/DeleteBuyCard")]
        public IActionResult DeleteBuyCard(long buyCardId)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var buycard = _repository.Product.FindByCondition(c => c.Id == buyCardId).FirstOrDefault();
                if (buycard == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());


                buycard.Ddate = DateTime.Now.Ticks;
                buycard.DuserId = userId;
                _repository.Product.Update(buycard);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, buyCardId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), buyCardId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// دریافت اطلاعات مربوط به کارت خرید  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("BuyCards/GetBuyCardById")]
        public IActionResult GetBuyCardById(long buyCardId)
        {
            try
            {

                var buycard = _repository.Product.FindByCondition(c => c.Id == buyCardId)
                    .Select(c => new
                    {
                        c.Name,
                        c.Description,
                        c.MetaDescription,
                        c.MetaTitle,
                        c.FirstCount,
                        c.KeyWords,
                        c.Price,
                    }).FirstOrDefault();
                if (buycard == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                var buycardImage = _repository.ProductImage
                    .FindByCondition(c => c.ProductId == buyCardId && c.DaDate == null && c.Ddate == null)
                    .Select(c => new { c.Id, c.ImageUrl }).ToList();

                var buycardRelative = _repository.RelatedProduct.FindByCondition(c => c.OriginProductId == buyCardId).Include(c => c.DestinProductId)
                     .Select(c => new { c.Id, c.DestinProduct.Name }).ToList();

                var result = new
                { BuyCard = buycard, BuycardImageList = buycardImage, RelativeProductList = buycardRelative };
                _logger.LogData(MethodBase.GetCurrentMethod(), result, null, buyCardId);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), buyCardId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// ویرایش کردن کارت خرید  
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("BuyCards/UpdateBuyCard")]
        public IActionResult UpdateBuyCard(long buyCardId)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);


                var buycard = JsonSerializer.Deserialize<InsertBuyCardDto>(HttpContext.Request.Form["BuyCard"]);
                var releventList = JsonSerializer.Deserialize<List<long>>(HttpContext.Request.Form["releventList"]);
                var product = _repository.Product.FindByCondition(c => c.Id == buyCardId).FirstOrDefault();
                if (product == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                product.Mdate = DateTime.Now.Ticks;
                product.MuserId = userId;
                product.FinalStatusId = 10;
                product.Description = buycard.Description;
                product.Count = buycard.FirstCount;
                product.FirstCount = buycard.FirstCount;
                product.KeyWords = buycard.KeyWords;
                product.MetaDescription = buycard.MetaDescription;
                product.MetaTitle = buycard.MetaTitle;
                product.Name = buycard.Name;
                product.Price = buycard.Price;

                var tobedeletedImage = _repository.ProductImage.FindByCondition(c => c.ProductId == buyCardId).ToList();
                _repository.ProductImage.DeleteRange(tobedeletedImage);

                var tobedeletedRelative = _repository.RelatedProduct
                    .FindByCondition(c => c.OriginProductId == buyCardId).ToList();
                _repository.RelatedProduct.DeleteRange(tobedeletedRelative);

                releventList.ForEach(c =>
                {
                    var pro = new RelatedProduct
                    {
                        CuserId = ClaimPrincipalFactory.GetUserId(User),
                        Cdate = DateTime.Now.Ticks,
                        DestinProductId = c
                    };
                    product.RelatedProductOriginProduct.Add(pro);

                });


                var fileList = HttpContext.Request.Form.Files.ToList();

                foreach (var uploadFileStatus in fileList.Select(file => FileManeger.FileUploader(file, 1, "BuyCardImages")))
                {
                    if (uploadFileStatus.Status == 200)
                    {
                        var image = new ProductImage
                        {
                            Cdate = DateTime.Now.Ticks,
                            CuserId = userId,
                            ImageUrl = uploadFileStatus.Path,

                        };
                        product.ProductImage.Add(image);
                    }
                    else
                    {
                        throw new BusinessException(uploadFileStatus.Path, 100);
                    }
                }

                _repository.Product.Update(product);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null,buyCardId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), buyCardId);
                return BadRequest(e.Message);
            }
        }
    }
}
