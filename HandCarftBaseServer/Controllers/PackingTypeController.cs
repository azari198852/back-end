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
using Entities.UIResponse;
using HandCarftBaseServer.ServiceProvider;
using HandCarftBaseServer.Tools;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HandCarftBaseServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class PackingTypeController : ControllerBase
    {
        public IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILogHandler _logger;

        public PackingTypeController(IMapper mapper, IRepositoryWrapper repository, ILogHandler logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        #region Deleted

        //[HttpGet]
        //[Route("PackingType/GetPackingTypeList")]
        //public IActionResult GetPackingTypeList()
        //{

        //    try
        //    {
        //        var res = _repository.PackingType.FindByCondition(c => (c.DaDate == null) && (c.Ddate == null)).Include(c => c.PackingTypeImage).ToList();
        //        var result = _mapper.Map<List<PackingTypeDto>>(res);
        //        return Ok(result);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest("Internal Server Error");
        //    }
        //}

        //[Authorize]
        //[HttpDelete]
        //[Route("PackingType/DeletePackingType")]
        //public IActionResult DeletePackingType(long packingtypeId)
        //{

        //    try
        //    {

        //        var packingtype = _repository.PackingType.FindByCondition(c => c.Id == packingtypeId).FirstOrDefault();
        //        if (packingtype == null) return NotFound();
        //        packingtype.Ddate = DateTime.Now.Ticks;
        //        packingtype.DuserId = ClaimPrincipalFactory.GetUserId(User);
        //        _repository.PackingType.Update(packingtype);
        //        _repository.Save();
        //        return NoContent();


        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest("Internal Server Error");
        //    }
        //}

        //[Authorize]
        //[HttpPost]
        //[Route("PackingType/InsertPackingType")]
        //public IActionResult InsertPackingType(PackingTypeDto packingTypeDto)
        //{

        //    try
        //    {
        //        if (!ModelState.IsValid) return BadRequest(ModelState);
        //        var packingType = _mapper.Map<PackingType>(packingTypeDto);
        //        packingType.Cdate = DateTime.Now.Ticks;
        //        packingType.CuserId = ClaimPrincipalFactory.GetUserId(User);
        //        _repository.PackingType.Create(packingType);
        //        _repository.Save();
        //        return Created("", packingType);

        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest("Internal Server Error");
        //    }
        //}

        //[Authorize]
        //[HttpPut]
        //[Route("PackingType/UpdatePackingType")]
        //public IActionResult UpdatePackingType(PackingTypeDto packingTypeDto)
        //{

        //    try
        //    {
        //        if (!ModelState.IsValid) return BadRequest(ModelState);

        //        var packingType = _repository.PackingType.FindByCondition(c => c.Id == packingTypeDto.Id).FirstOrDefault();
        //        if (packingType == null) return NotFound();
        //        packingType.Name = packingTypeDto.Name;
        //        packingType.Price = packingTypeDto.Price;
        //        packingType.Weight = packingTypeDto.Weight;
        //        packingType.Mdate = DateTime.Now.Ticks;
        //        packingType.MuserId = ClaimPrincipalFactory.GetUserId(User);
        //        _repository.PackingType.Update(packingType);
        //        _repository.Save();
        //        return NoContent();

        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest("Internal Server Error");
        //    }
        //}

        //[Authorize]
        //[HttpPost]
        //[Route("PackingType/UploadPackingTypeImage")]
        //public IActionResult UploadPackingTypeImage()
        //{

        //    try
        //    {
        //        var a = HttpContext.Request.Form.Files[0];

        //        FileManeger.UploadFileStatus uploadFileStatus = FileManeger.FileUploader(a, 1, "PackingTypeImages");
        //        var packingTypeImageDto = JsonSerializer.Deserialize<PackingTypeImageDto>(HttpContext.Request.Form["packingTypeImage"]);

        //        if (uploadFileStatus.Status == 200)
        //        {


        //            var packingTypeImage = _mapper.Map<PackingTypeImage>(packingTypeImageDto);
        //            packingTypeImage.Cdate = DateTime.Now.Ticks;
        //            packingTypeImage.CuserId = ClaimPrincipalFactory.GetUserId(User);
        //            packingTypeImage.ImageFileUrl = uploadFileStatus.Path;
        //            _repository.PackingTypeImage.Create(packingTypeImage);
        //            _repository.Save();
        //            return Created("", packingTypeImage);
        //        }
        //        else
        //        {
        //            return BadRequest("");
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest("Internal Server Error");
        //    }
        //}

        //[Authorize]
        //[HttpDelete]
        //[Route("PackingType/DeletePackingTypeImage")]
        //public IActionResult DeletePackingTypeImage(long packingTypeImageId)
        //{

        //    try
        //    {

        //        var image = _repository.PackingTypeImage.FindByCondition(c => c.Id == packingTypeImageId)
        //            .FirstOrDefault();
        //        var deletedFile = image.ImageFileUrl;
        //        if (image == null) return NotFound();
        //        _repository.PackingTypeImage.Delete(image);
        //        _repository.Save();
        //        FileManeger.FileRemover(new List<string> { deletedFile });
        //        return NoContent();


        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest("Internal Server Error");
        //    }
        //}

        #endregion

        /// <summary>
        /// دریافت لیست بسته بندی 
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("PackingType/GetPackingTypeList")]
        public IActionResult GetPackingTypeList()
        {

            try
            {
                var res = _repository.PackingType.FindByCondition(c => c.DaDate == null && c.Ddate == null)
                    .Include(c => c.PackingTypeImage)
                    .Include(c => c.ProductPackingType)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Price,
                        c.Weight,
                        c.Material,
                        c.Count,
                        c.ColorId,
                        Image = c.PackingTypeImage.Where(c => c.Ddate == null && c.FileType == 2).Select(x => x.ImageFileUrl).FirstOrDefault(),
                        ProductName = string.Join(',', c.ProductPackingType.Select(x => x.Product.Name).ToList()),
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
        /// دریافت بسته بندی با آیدی 
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("PackingType/GetPackingTypeById")]
        public IActionResult GetPackingTypeById(long packingtypeId)
        {

            try
            {
                var res = _repository.PackingType.FindByCondition(c => c.Id == packingtypeId)
                    .Include(c => c.PackingTypeImage)
                    .Include(c => c.Color)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Price,
                        c.Weight,
                        c.Material,
                        c.Count,
                        c.ColorId,
                        Image = c.PackingTypeImage.Where(c => c.Ddate == null && c.FileType == 2).Select(x => x.ImageFileUrl).FirstOrDefault(),
                        Video = c.PackingTypeImage.Where(c => c.Ddate == null && c.FileType == 1).Select(x => x.ImageFileUrl).FirstOrDefault(),
                        Color = c.Color.Name

                    }).FirstOrDefault();
                if (res == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                _logger.LogData(MethodBase.GetCurrentMethod(), res, null, packingtypeId);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);
            }
        }


        /// <summary>
        /// ثبت بسته بندی 
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("PackingType/InsertPackingType")]
        public IActionResult InsertPackingType()
        {

            try
            {
                var packingType = JsonSerializer.Deserialize<PackingType>(HttpContext.Request.Form["PackingType"]);
                var validator = new ParamValidator();
                validator.ValidateNull(packingType.Name,
                        General.Messages_.NullInputMessages_.GeneralNullMessage("نام"))
                    .ValidateNull(packingType.Price,
                        General.Messages_.NullInputMessages_.GeneralNullMessage("قیمت"));

                var photo = HttpContext.Request.Form.Files.GetFile("Photo");
                var video = HttpContext.Request.Form.Files.GetFile("Video");
                var photopath = "";
                var videopath = "";
                if (photo != null)
                {
                    var uploadFileStatus = FileManeger.FileUploader(photo, 1, "PackingTypeImages");
                    if (uploadFileStatus.Status == 200)
                    {
                        photopath = uploadFileStatus.Path;
                    }
                    else
                    {
                        throw new BusinessException(uploadFileStatus.Path, 100);
                    }
                }

                if (video != null)
                {
                    var uploadFileStatus1 = FileManeger.FileUploader(video, 2, "PackingTypeVideo");
                    if (uploadFileStatus1.Status == 200)
                    {
                        videopath = uploadFileStatus1.Path;
                    }
                    else
                    {
                        throw new BusinessException(uploadFileStatus1.Path, 100);
                    }

                }


                packingType.Cdate = DateTime.Now.Ticks;
                packingType.CuserId = ClaimPrincipalFactory.GetUserId(User);

                if (photopath != "")
                {
                    var packingtypeImge = new PackingTypeImage
                    {
                        ImageFileUrl = photopath,
                        Cdate = DateTime.Now.Ticks,
                        CuserId = ClaimPrincipalFactory.GetUserId(User),
                        FileType = 2

                    };
                    packingType.PackingTypeImage.Add(packingtypeImge);

                }

                if (photopath != "")
                {
                    var packingtypeVideo = new PackingTypeImage
                    {
                        ImageFileUrl = videopath,
                        Cdate = DateTime.Now.Ticks,
                        CuserId = ClaimPrincipalFactory.GetUserId(User),
                        FileType = 1

                    };
                    packingType.PackingTypeImage.Add(packingtypeVideo);
                }

                _repository.PackingType.Create(packingType);
                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), packingType.Id, null);
                return Ok(packingType.Id);


            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// ویرایش بسته بندی 
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("PackingType/UpdatePackingType")]
        public IActionResult UpdatePackingType()
        {

            try
            {
                var packingType = JsonSerializer.Deserialize<PackingType>(HttpContext.Request.Form["PackingType"]);
                var validator = new ParamValidator();
                validator.ValidateNull(packingType.Name,
                        General.Messages_.NullInputMessages_.GeneralNullMessage("نام"))
                    .ValidateNull(packingType.Price,
                        General.Messages_.NullInputMessages_.GeneralNullMessage("قیمت"))
                    .ValidateNull(packingType.Id,
                    General.Messages_.NullInputMessages_.GeneralNullMessage("آیدی"));

                var photo = HttpContext.Request.Form.Files.GetFile("Photo");
                var video = HttpContext.Request.Form.Files.GetFile("Video");
                var photopath = "";
                var videopath = "";
                if (photo != null)
                {
                    var uploadFileStatus = FileManeger.FileUploader(photo, 1, "PackingTypeImages");
                    if (uploadFileStatus.Status == 200)
                    {
                        photopath = uploadFileStatus.Path;
                    }
                    else
                    {
                        throw new BusinessException(uploadFileStatus.Path, 100);
                    }
                }

                if (video != null)
                {
                    var uploadFileStatus1 = FileManeger.FileUploader(video, 2, "PackingTypeVideo");
                    if (uploadFileStatus1.Status == 200)
                    {
                        videopath = uploadFileStatus1.Path;
                    }
                    else
                    {
                        throw new BusinessException(uploadFileStatus1.Path, 100);
                    }

                }


                var packing = _repository.PackingType.FindByCondition(c => c.Id == packingType.Id)
                    .Include(c => c.PackingTypeImage).FirstOrDefault();

                if (packing == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                packing.MuserId = DateTime.Now.Ticks;
                packing.Mdate = ClaimPrincipalFactory.GetUserId(User);

                if (photopath != "")
                {
                    var toBeDeletedImage = packing.PackingTypeImage.FirstOrDefault(c => c.FileType == 2);
                    if (toBeDeletedImage != null)
                        _repository.PackingTypeImage.Delete(toBeDeletedImage);

                    var packingtypeImge = new PackingTypeImage
                    {
                        ImageFileUrl = photopath,
                        Cdate = DateTime.Now.Ticks,
                        CuserId = ClaimPrincipalFactory.GetUserId(User),
                        FileType = 1

                    };
                    packing.PackingTypeImage.Add(packingtypeImge);

                }

                if (photopath != "")
                {
                    var toBeDeletedVideo = packing.PackingTypeImage.FirstOrDefault(c => c.FileType == 1);
                    if (toBeDeletedVideo != null)
                        _repository.PackingTypeImage.Delete(toBeDeletedVideo);
                    var packingtypeVideo = new PackingTypeImage
                    {
                        ImageFileUrl = videopath,
                        Cdate = DateTime.Now.Ticks,
                        CuserId = ClaimPrincipalFactory.GetUserId(User),
                        FileType = 2

                    };
                    packing.PackingTypeImage.Add(packingtypeVideo);
                }

                _repository.PackingType.Update(packing);
                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), packing.Id, null);
                return Ok(packing.Id);


            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// حذف بسته بندی 
        /// </summary>
        [Authorize]
        [HttpDelete]
        [Route("PackingType/DeletePackingType")]
        public IActionResult DeletePackingType(long packingtypeId)
        {

            try
            {

                var packingtype = _repository.PackingType.FindByCondition(c => c.Id == packingtypeId).FirstOrDefault();
                var packingtypeImageList = _repository.PackingTypeImage.FindByCondition(c => c.PackingTypeId == packingtypeId).ToList();
                if (packingtype == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                packingtype.Ddate = DateTime.Now.Ticks;
                packingtype.DuserId = ClaimPrincipalFactory.GetUserId(User);

                packingtypeImageList.ForEach(c =>
                {
                    c.Ddate = DateTime.Now.Ticks;
                    c.DuserId = ClaimPrincipalFactory.GetUserId(User);

                });

                _repository.PackingTypeImage.UpdateRange(packingtypeImageList);
                _repository.PackingType.Update(packingtype);
                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, packingtypeId);
                return Ok(General.Results_.SuccessMessage());


            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), packingtypeId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// فعال و غیر فعال کردن بسته بندی 
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("PackingType/ActiveDeActivePackingType")]
        public IActionResult ActiveDeActivePackingType(long packingtypeId)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var res = _repository.PackingType.FindByCondition(c => c.Id == packingtypeId && c.Ddate == null).FirstOrDefault();
                if (res == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                if (res.DaUserId != null)
                {
                    res.DaDate = null;
                    res.DaUserId = userId;

                }
                else
                {
                    res.DaDate = DateTime.Now.Ticks;
                    res.DaUserId = userId;
                }
                _repository.PackingType.Update(res);
                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, packingtypeId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), packingtypeId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// دریافت سابقه محصولات 
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("PackingType/GetPackingTypeHistoryById")]
        public IActionResult GetPackingTypeHistoryById(long packingtypeId)
        {

            try
            {
                var res = _repository.ProductPackingType.FindByCondition(c => c.PackinggTypeId == packingtypeId && c.Ddate == null)
                    .Include(c => c.Product)
                    .Select(c => new
                    {
                        c.Product.Id,
                        c.Product.Name,
                        c.Product.Coding

                    }).ToList();

                _logger.LogData(MethodBase.GetCurrentMethod(), res, null, packingtypeId);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// دریافت لیست بسته بندی های اعمال شده 
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("PackingType/GetProductPackingTypeList")]
        public IActionResult GetProductPackingTypeList()
        {

            try
            {
                var res = _repository.ProductPackingTypeList.FindByCondition(c => c.Ddate == null)
                  .Select(c => new
                  {
                      c.Id,
                      c.Title,
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
        /// فعال و غیر فعال کردن لیست بسته بندی 
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("PackingType/ActiveDeActiveProductPackingTypeList")]
        public IActionResult ActiveDeActiveProductPackingTypeList(long productPackingTypeListId)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var res = _repository.ProductPackingTypeList.FindByCondition(c => c.Id == productPackingTypeListId && c.Ddate == null).FirstOrDefault();
                if (res == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                if (res.DaUserId != null)
                {
                    res.DaDate = null;
                    res.DaUserId = userId;

                }
                else
                {
                    res.DaDate = DateTime.Now.Ticks;
                    res.DaUserId = userId;
                }
                _repository.ProductPackingTypeList.Update(res);
                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, productPackingTypeListId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), productPackingTypeListId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// دریافت لیست محصولات بسته بندی 
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("PackingType/GetProductPackingList")]
        public IActionResult GetProductPackingList(long productPackingTypeListId)
        {

            try
            {
                var res = _repository.ProductPackingType.FindByCondition(c => c.Ddate == null && c.DaDate == null && c.ProductPackingTypeListId == productPackingTypeListId)
                    .Include(c => c.Product)
                    .Select(c => new
                    {
                        c.Id,
                        c.Price,
                        c.Weight,
                        c.ProductId,
                        ProductName = c.Product.Name,
                        ProductCoding = c.Product.Coding

                    }).ToList();

                _logger.LogData(MethodBase.GetCurrentMethod(), res, null, productPackingTypeListId);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), productPackingTypeListId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// ثبت گروهی بسته بندی 
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("PackingType/InsertProductPackingList")]
        public IActionResult InsertProductPackingList(ProductPackingListInsertDto input)
        {

            try
            {

                var validator = new ParamValidator();
                validator.ValidateNull(input.Title,
                        General.Messages_.NullInputMessages_.GeneralNullMessage("نام"))
                    .ValidateNull(input.PackingTypeId,
                        General.Messages_.NullInputMessages_.GeneralNullMessage("کد بسته بندی"))
                    .Throw(General.Results_.FieldNullErrorCode());

                var packingType = _repository.PackingType.FindByCondition(c =>
                    c.Id == input.PackingTypeId && c.DaDate == null && c.Ddate == null).FirstOrDefault();

                if (packingType == null || packingType.Count < 1)
                    throw new BusinessException(XError.BusinessErrors.InvalidPackingType());

                var packingList = new ProductPackingTypeList
                {
                    Cdate = DateTime.Now.Ticks,
                    CuserId = ClaimPrincipalFactory.GetUserId(User),
                    Title = input.Title,

                };
                input.ProductIdList.ForEach(c =>
                {
                    var pp = new ProductPackingType
                    {
                        Cdate = DateTime.Now.Ticks,
                        CuserId = ClaimPrincipalFactory.GetUserId(User),
                        PackinggTypeId = input.PackingTypeId,
                        ProductId = c,
                        Price = packingType.Price
                    };
                    packingList.ProductPackingType.Add(pp);

                });
                packingType.Count -= 1;
                _repository.ProductPackingTypeList.Create(packingList);
                _repository.PackingType.Update(packingType);

                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), packingList.Id, null, input);
                return Ok(packingType.Id);


            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// حذف بسته بندی گروهی 
        /// </summary>
        [Authorize]
        [HttpDelete]
        [Route("PackingType/DeleteProductPackingList")]
        public IActionResult DeleteProductPackingList(long productPackingTypeListId)
        {

            try
            {


                var packingList = _repository.ProductPackingTypeList.FindByCondition(c =>
                    c.Id == productPackingTypeListId).FirstOrDefault();

                if (packingList == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                packingList.Ddate = DateTime.Now.Ticks;
                packingList.DuserId = ClaimPrincipalFactory.GetUserId(User);

                _repository.ProductPackingTypeList.Update(packingList);

                var prodctPacking = _repository.ProductPackingType
                    .FindByCondition(c => c.ProductPackingTypeListId == productPackingTypeListId).ToList();
                prodctPacking.ForEach(c =>
                {

                    c.Ddate = DateTime.Now.Ticks;
                    c.DuserId = ClaimPrincipalFactory.GetUserId(User);
                });
                _repository.ProductPackingType.UpdateRange(prodctPacking);

                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, productPackingTypeListId);
                return Ok(General.Results_.SuccessMessage());


            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), productPackingTypeListId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// دریافت اطلاعات بسته بندی گروهی 
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("PackingType/GetProductPackingListInfoById")]
        public IActionResult GetProductPackingListInfoById(long productPackingTypeListId)
        {

            try
            {
                var res = _repository.ProductPackingTypeList.FindByCondition(c => c.Ddate == null && c.DaDate == null && c.Id == productPackingTypeListId)
                    .Select(c => new
                    {
                        c.Id,
                        c.Title,
                        productList = c.ProductPackingType.Select(c => new { c.Product.Id, c.Product.Coding, c.Product.Name }).ToList()

                    }).FirstOrDefault();

                _logger.LogData(MethodBase.GetCurrentMethod(), res, null, productPackingTypeListId);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), productPackingTypeListId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// ویزایش گروهی بسته بندی 
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("PackingType/UpdateProductPackingList")]
        public IActionResult UpdateProductPackingList(ProductPackingListInsertDto input)
        {

            try
            {

                var validator = new ParamValidator();
                validator.ValidateNull(input.Title,
                        General.Messages_.NullInputMessages_.GeneralNullMessage("نام"))
                    .ValidateNull(input.PackingTypeId,
                        General.Messages_.NullInputMessages_.GeneralNullMessage("کد بسته بندی"))
                    .ValidateNull(input.PackingTypeId,
                        General.Messages_.NullInputMessages_.GeneralNullMessage("آیدی"))
                    .Throw(General.Results_.FieldNullErrorCode());

                var packingList = _repository.ProductPackingTypeList.FindByCondition(c => c.Id == input.Id)
                    .FirstOrDefault();

                if (packingList == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                packingList.Title = input.Title;
                packingList.Mdate = DateTime.Now.Ticks;
                packingList.MuserId = ClaimPrincipalFactory.GetUserId(User);

                var toBeDeleted = _repository.ProductPackingType
                    .FindByCondition(c => c.ProductPackingTypeListId == input.Id).ToList();

                toBeDeleted.ForEach(c =>
                {
                    c.Ddate = DateTime.Now.Ticks;
                    c.DuserId = ClaimPrincipalFactory.GetUserId(User);
                });
                _repository.ProductPackingType.UpdateRange(toBeDeleted);

                var InDbPackingType = _repository.PackingType.FindByCondition(c => c.Id == toBeDeleted.Select(c => c.PackinggTypeId).FirstOrDefault()).FirstOrDefault();
                if (InDbPackingType.Id == input.PackingTypeId)
                {
                    input.ProductIdList.ForEach(c =>
                    {
                        var pp = new ProductPackingType
                        {
                            Cdate = DateTime.Now.Ticks,
                            CuserId = ClaimPrincipalFactory.GetUserId(User),
                            PackinggTypeId = input.PackingTypeId,
                            ProductId = c,
                            Price = InDbPackingType.Price
                        };
                        packingList.ProductPackingType.Add(pp);

                    });

                }
                else
                {
                    var packingType = _repository.PackingType.FindByCondition(c =>
                        c.Id == input.PackingTypeId && c.DaDate == null && c.Ddate == null).FirstOrDefault();
                    if (packingType == null || packingType.Count < 1)
                        throw new BusinessException(XError.BusinessErrors.InvalidPackingType());

                    input.ProductIdList.ForEach(c =>
                    {
                        var pp = new ProductPackingType
                        {
                            Cdate = DateTime.Now.Ticks,
                            CuserId = ClaimPrincipalFactory.GetUserId(User),
                            PackinggTypeId = input.PackingTypeId,
                            ProductId = c,
                            Price = packingType.Price
                        };
                        packingList.ProductPackingType.Add(pp);
                    });
                    packingType.Count -= 1;
                    _repository.PackingType.Update(packingType);
                }

                _repository.ProductPackingTypeList.Update(packingList);

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
        /// دریافت سابقه بسته بندی محصول 
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("PackingType/GetProductPackingTypeHistory")]
        public IActionResult GetProductPackingTypeHistory(long productId)
        {

            try
            {
                var res = _repository.ProductPackingType.FindByCondition(c => c.Ddate == null && c.DaDate == null && c.ProductId == productId)
                    .Include(c=>c.PackinggType)
                    .Select(c => new
                    {
                       c.PackinggType.Name,
                      CreationDate=DateTimeFunc.TimeTickToMiladi(c.Cdate.Value)

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



        #region UI_Methods

        /// <summary>
        ///لیست انواع بسته بندی محصول 
        /// </summary>
        [HttpGet]
        [Route("PackingType/GetPackingTypeList_UI")]
        public ListResult<PackingTypeDto> GetPackingTypeList_UI()
        {

            try
            {
                var res = _repository.PackingType.FindByCondition(c => (c.DaDate == null) && (c.Ddate == null)).Include(c => c.PackingTypeImage).ToList();
                var result = _mapper.Map<List<PackingTypeDto>>(res);
                var finalresult = ListResult<PackingTypeDto>.GetSuccessfulResult(result);
                return finalresult;
            }
            catch (Exception e)
            {
                return ListResult<PackingTypeDto>.GetFailResult(null);
            }
        }

        [HttpGet]
        [Route("PackingType/GetPackingTypeById_UI")]
        public SingleResult<PackingTypeDto> GetPackingTypeById_UI(long packingtypeId)
        {

            try
            {
                var res = _repository.PackingType.FindByCondition(c => c.Id == packingtypeId)
                    .Include(c => c.PackingTypeImage).First();
                var result = _mapper.Map<PackingTypeDto>(res);
                var finalresult = SingleResult<PackingTypeDto>.GetSuccessfulResult(result);
                return finalresult;

            }
            catch (Exception e)
            {
                return SingleResult<PackingTypeDto>.GetFailResult(null);
            }
        }


        #endregion
    }
}
