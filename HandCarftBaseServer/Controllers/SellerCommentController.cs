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
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace HandCarftBaseServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class SellerCommentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILogHandler _logger;

        public SellerCommentController(IMapper mapper, IRepositoryWrapper repository, ILogHandler logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// ثبت بیوگرافی فروشنده 
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("SellerComment/InsertSellerBio")]
        public IActionResult InsertSellerBio(string input)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var sellerId = _repository.Seller.GetSellerIdByUserId(userId);
                if (sellerId == 0)
                {
                    throw new BusinessException(XError.AuthenticationErrors.NotHaveRequestedRole());
                }

                var sellerComment = new SellerComment
                {

                    FinalStatusId = 37,
                    Cdate = DateTime.Now.Ticks,
                    CuserId = userId,
                    CommentType = 1,
                    SellerId = sellerId,
                    Comment = input

                };


                _repository.SellerComment.Create(sellerComment);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, input);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// دریافت اطلاعات بیوگرافی فروشنده 
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("SellerComment/GetSellerBio")]
        public IActionResult GetSellerBio()
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var sellerId = _repository.Seller.GetSellerIdByUserId(userId);
                if (sellerId == 0)
                {
                    throw new BusinessException(XError.AuthenticationErrors.NotHaveRequestedRole());
                }

                var res = _repository.SellerComment.FindByCondition(c => c.SellerId == sellerId && c.CommentType == 1).Include(c => c.FinalStatus).Include(c => c.Seller)
                    .FirstOrDefault();
                if (res == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                var result = _mapper.Map<SellerCommentDto>(res);

                _logger.LogData(MethodBase.GetCurrentMethod(), result, null);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// ویرایش بیوگرافی فروشنده 
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("SellerComment/UpdateSellerBio")]
        public IActionResult UpdateSellerBio(long sellerCommentId, string input)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var sellerId = _repository.Seller.GetSellerIdByUserId(userId);
                if (sellerId == 0)
                {
                    throw new BusinessException(XError.AuthenticationErrors.NotHaveRequestedRole());
                }

                var sellerComment = _repository.SellerComment.FindByCondition(c => c.Id == sellerCommentId && c.CommentType == 1)
                    .FirstOrDefault();
                if (sellerComment == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                sellerComment.FinalStatusId = 37;
                sellerComment.Mdate = DateTime.Now.Ticks;
                sellerComment.MuserId = userId;
                sellerComment.CommentType = 1;
                sellerComment.SellerId = sellerId;
                sellerComment.Comment = input;




                _repository.SellerComment.Update(sellerComment);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, input);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// ثبت نظر فروشنده 
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("SellerComment/InsertSellerComment")]
        public IActionResult InsertSellerComment(string input)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var sellerId = _repository.Seller.GetSellerIdByUserId(userId);
                if (sellerId == 0)
                {
                    throw new BusinessException(XError.AuthenticationErrors.NotHaveRequestedRole());
                }

                var sellerComment = new SellerComment
                {

                    FinalStatusId = 37,
                    Cdate = DateTime.Now.Ticks,
                    CuserId = userId,
                    CommentType = 2,
                    SellerId = sellerId,
                    Comment = input

                };


                _repository.SellerComment.Create(sellerComment);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, input);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// دریافت نظر فروشنده 
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("SellerComment/GetSellerComment")]
        public IActionResult GetSellerComment()
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var sellerId = _repository.Seller.GetSellerIdByUserId(userId);
                if (sellerId == 0)
                {
                    throw new BusinessException(XError.AuthenticationErrors.NotHaveRequestedRole());
                }

                var res = _repository.SellerComment.FindByCondition(c => c.SellerId == sellerId && c.CommentType == 2).Include(c => c.FinalStatus).Include(c => c.Seller)
                    .FirstOrDefault();
                if (res == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                var result = _mapper.Map<SellerCommentDto>(res);

                _logger.LogData(MethodBase.GetCurrentMethod(), result, null);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// ویرایش نظر فروشنده 
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("SellerComment/UpdateSellerComment")]
        public IActionResult UpdateSellerComment(long sellerCommentId, string input)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var sellerId = _repository.Seller.GetSellerIdByUserId(userId);
                if (sellerId == 0)
                {
                    throw new BusinessException(XError.AuthenticationErrors.NotHaveRequestedRole());
                }

                var sellerComment = _repository.SellerComment.FindByCondition(c => c.Id == sellerCommentId && c.CommentType == 2)
                    .FirstOrDefault();
                if (sellerComment == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                sellerComment.FinalStatusId = 37;
                sellerComment.Mdate = DateTime.Now.Ticks;
                sellerComment.MuserId = userId;
                sellerComment.CommentType = 2;
                sellerComment.SellerId = sellerId;
                sellerComment.Comment = input;


                _repository.SellerComment.Update(sellerComment);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, input);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// دریافت لیست نظرات فروشنده 
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("SellerComment/GetSellerCommentList")]
        public IActionResult GetSellerCommentList()
        {
            try
            {

                var res = _repository.SellerComment.FindByCondition(c => c.DaDate == null && c.Ddate == null).Include(c => c.FinalStatus).Include(c => c.Seller)
                    .ToList();
                if (res == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                var result = _mapper.Map<List<SellerCommentDto>>(res);

                _logger.LogData(MethodBase.GetCurrentMethod(), result, null);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// فعال و غیر فعال کردن نظر فروشنده 
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("SellerComment/ActiveDeActiveSellerComment")]
        public IActionResult ActiveDeActiveSellerComment(long sellerCommentId)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var res = _repository.SellerComment.FindByCondition(c => c.Id == sellerCommentId && c.DaDate == null && c.Ddate == null).FirstOrDefault();
                if (res == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                res.FinalStatusId = res.FinalStatusId == 36 ? 37 : 36;
                if (res.FinalStatusId == 36)
                {
                    res.DaDate = null;
                    res.DaUserId = userId;

                }
                else
                {
                    res.DaDate = DateTime.Now.Ticks;
                    res.DaUserId = userId;
                }
                _repository.SellerComment.Update(res);
                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, sellerCommentId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), sellerCommentId);
                return BadRequest(e.Message);
            }
        }


    }
}
