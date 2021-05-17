using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.UIResponse;
using HandCarftBaseServer.ServiceProvider.PostService;
using HandCarftBaseServer.Tools;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HandCarftBaseServer.Controllers
{
    [Route("api/Package")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        public IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILogHandler _logger;

        public PackageController(IMapper mapper, IRepositoryWrapper repository, ILogHandler logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }


        /// <summary>
        ///لیست پیکج های فعال به همراه عکس ها 
        /// </summary>
        [HttpGet]
        [Route("GetPackageList_UI")]
        public ListResult<PackageDto> GetPackageList_UI()
        {
            try
            {

                var time = DateTime.Now.Ticks;
                var res = _repository.Package
                    .FindByCondition(c =>
                        c.DaDate == null && c.Ddate == null && c.StartDateTime < time && time < c.EndDateTime)
                    .Include(c => c.PackageImage).ToList();
                var result = _mapper.Map<List<PackageDto>>(res);

                var finalresult = ListResult<PackageDto>.GetSuccessfulResult(result);
                return finalresult;

            }
            catch (Exception e)
            {
                return ListResult<PackageDto>.GetFailResult(null);

            }
        }

        /// <summary>
        ///لیست محصولات پکیج براساس آیدی پکیج 
        /// </summary>
        [HttpGet]
        [Route("GetPackageProductListById_UI")]
        public ListResult<ProductDto> GetPackageProductListById_UI(long packageId)
        {
            try
            {



                var post = new PostServiceProvider();
                var postpriceparam = new PostGetDeliveryPriceParam
                {
                    Price = 1000000,
                    Weight = 1000,
                    ServiceType = 2,// (int)customerOrder.PostTypeId,
                    ToCityId = 39751
                };
                var postresult = post.GetDeliveryPrice(postpriceparam).Result;


                var res = _repository.PackageProduct
                    .FindByCondition(c => c.PackageId == packageId && c.DaDate == null && c.Ddate == null)
                    .Include(c => c.Product).Select(c => c.Product).ToList();
                var result = _mapper.Map<List<ProductDto>>(res);

                var finalresult = ListResult<ProductDto>.GetSuccessfulResult(result);
                return finalresult;

            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), packageId);
                return ListResult<ProductDto>.GetFailResult(e.Message);

            }
        }

       // [Authorize]
        [HttpPost]
        [Route("InsertPackage")]
        public IActionResult InsertPackage([FromForm] InsertPackageDto input, [FromForm] FormFileCollection fileList)
        {
            try
            {
                var validator = new ParamValidator();
                validator.ValidateNull(input.Name, General.Messages_.NullInputMessages_.GeneralNullMessage("عنوان"))
                    .ValidateNull(input.Price, General.Messages_.NullInputMessages_.GeneralNullMessage("قیمت"))
                    .Throw(General.Results_.FieldNullErrorCode());

                if (input.ProductWithPriceList.Count == 0)
                {
                    throw new BusinessException("محصولی برای پکیج انتخاب نشده است.", 4001);
                }


                var counter = (_repository.Product
                    .FindByCondition(c => c.Coding.ToString().Substring(0, 8) == "11223344")
                    .Count() + 1).ToString();
                counter = counter.PadLeft(4, '0');

                var newproduct = new Product
                {
                    Name = input.Name,
                    Price = input.Price,
                    MetaDescription = input.MetaDesc,
                    KeyWords = input.KeyWord,
                    MetaTitle = input.MetaTitle,
                    Coding = long.Parse("11223344" + counter),
                    IsPackage = true,
                    Cdate = DateTime.Now.Ticks,
                    CuserId = ClaimPrincipalFactory.GetUserId(User)
                };

                input.ProductWithPriceList.ForEach(c =>
                {
                    var newProductPackage = new ProductPackage()
                    {
                        Cdate = DateTime.Now.Ticks,
                        CuserId = ClaimPrincipalFactory.GetUserId(User),
                        DepProductId = c.ProductId,
                        Price = c.Price

                    };

                    newproduct.ProductPackageDepProduct.Add(newProductPackage);

                });

                fileList.ForEach(c =>
                {
                    short fileType = 1;
                    if (FileManeger.IsVideo(c))
                        fileType = 2;
                    var uploadFileStatus = FileManeger.FileUploader(c, fileType, "ProductPackage");
                    if (uploadFileStatus.Status == 200)
                    {
                        var newProductImage = new ProductImage
                        {
                            Cdate = DateTime.Now.Ticks,
                            CuserId = ClaimPrincipalFactory.GetUserId(User),
                            ImageUrl = uploadFileStatus.Path,
                            FileType = fileType

                        };
                        newproduct.ProductImage.Add(newProductImage);
                    }
                    else
                    {
                        throw new BusinessException(uploadFileStatus.Path, 4009);
                    }

                });

                _repository.Product.Create(newproduct);
                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), newproduct.Id, null, input, "****");
                return Ok(newproduct.Id);

            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input, "****");
                return BadRequest(e.Message);

            }
        }

        [Authorize]
        [HttpDelete]
        [Route("DeletePackage")]
        public IActionResult DeletePackage(long packageId)
        {
            try
            {

                var package = _repository.Product
                    .FindByCondition(c => c.Id == packageId && c.Ddate == null && c.IsPackage == true).FirstOrDefault();
                if (package == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                package.Ddate = DateTime.Now.Ticks;
                package.DuserId = ClaimPrincipalFactory.GetUserId(User);
                _repository.Product.Update(package);
                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, packageId);
                return Ok(General.Results_.SuccessMessage());

            }
            catch (Exception e)
            {

                _logger.LogError(e, MethodBase.GetCurrentMethod(), packageId);
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPut]
        [Route("UpdatePackage")]
        public IActionResult UpdatePackage([FromForm] InsertPackageDto input, [FromForm] FormFileCollection fileList)
        {
            try
            {
                var validator = new ParamValidator();
                validator.ValidateNull(input.Name, General.Messages_.NullInputMessages_.GeneralNullMessage("عنوان"))
                    .ValidateNull(input.Price, General.Messages_.NullInputMessages_.GeneralNullMessage("قیمت"))
                    .ValidateNull(input.PackageId, General.Messages_.NullInputMessages_.GeneralNullMessage("آیدی پکیج"))
                    .Throw(General.Results_.FieldNullErrorCode());

                if (input.ProductWithPriceList.Count == 0)
                {
                    throw new BusinessException("محصولی برای پکیج انتخاب نشده است.", 4001);
                }

                var product = _repository.Product.FindByCondition(c => c.Id == input.PackageId.Value && c.Ddate == null)
                     .FirstOrDefault();

                if (product == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                product.Name = input.Name;
                product.Price = input.Price;
                product.MetaDescription = input.MetaDesc;
                product.KeyWords = input.KeyWord;
                product.MetaTitle = input.MetaTitle;
                product.Mdate = DateTime.Now.Ticks;
                product.MuserId = ClaimPrincipalFactory.GetUserId(User);


                var tobeDeletedProductPackeage = _repository.ProductPackage
                    .FindByCondition(c => c.MainProductId == product.Id).ToList();
                tobeDeletedProductPackeage.ForEach(c =>
                {
                    c.Ddate = DateTime.Now.Ticks;
                    c.DuserId = ClaimPrincipalFactory.GetUserId(User);
                });

                _repository.ProductPackage.UpdateRange(tobeDeletedProductPackeage);

                input.ProductWithPriceList.ForEach(c =>
                {
                    var newProductPackage = new ProductPackage()
                    {
                        Cdate = DateTime.Now.Ticks,
                        CuserId = ClaimPrincipalFactory.GetUserId(User),
                        DepProductId = c.ProductId,
                        Price = c.Price

                    };

                    product.ProductPackageDepProduct.Add(newProductPackage);

                });

                var tobeDeletedProductImage = _repository.ProductImage
                    .FindByCondition(c => c.ProductId == product.Id).ToList();
                tobeDeletedProductImage.ForEach(c =>
                {
                    c.Ddate = DateTime.Now.Ticks;
                    c.DuserId = ClaimPrincipalFactory.GetUserId(User);
                });

                _repository.ProductImage.UpdateRange(tobeDeletedProductImage);

                fileList.ForEach(c =>
                {
                    short fileType = 1;
                    if (FileManeger.IsVideo(c))
                        fileType = 2;
                    var uploadFileStatus = FileManeger.FileUploader(c, fileType, "ProductPackage");
                    if (uploadFileStatus.Status == 200)
                    {
                        var newProductImage = new ProductImage
                        {
                            Cdate = DateTime.Now.Ticks,
                            CuserId = ClaimPrincipalFactory.GetUserId(User),
                            ImageUrl = uploadFileStatus.Path,
                            FileType = fileType

                        };
                        product.ProductImage.Add(newProductImage);
                    }
                    else
                    {
                        throw new BusinessException(uploadFileStatus.Path, 4009);
                    }

                });

                _repository.Product.Update(product);
                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, input, "****");
                return Ok(General.Results_.SuccessMessage());

            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input, "****");
                return BadRequest(e.Message);

            }
        }

        [HttpGet]
        [Route("GetPackageList")]
        public IActionResult GetPackageList()
        {
            try
            {

                var res = _repository.Product
                    .FindByCondition(c => c.Ddate == null && c.DaDate == null && c.IsPackage == true).Select(
                        c => new { c.Id, c.Name, c.Price, c.Coding }).OrderByDescending(c => c.Id).ToList();

                _logger.LogData(MethodBase.GetCurrentMethod(), res, null);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);

            }
        }

        [HttpGet]
        [Route("GetPackageFileByPackageId")]
        public IActionResult GetPackageFileByPackageId(long packageId)
        {
            try
            {

                var res = _repository.ProductImage
                    .FindByCondition(c => c.Ddate == null && c.DaDate == null && c.ProductId == packageId).Select(
                        c => new { c.Id, c.FileType, c.ImageUrl }).OrderByDescending(c => c.Id).ToList();

                _logger.LogData(MethodBase.GetCurrentMethod(), res, null);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);

            }
        }

        [HttpGet]
        [Route("GetPackageProductByPackageId")]
        public IActionResult GetPackageProductByPackageId(long packageId)
        {
            try
            {

                var res = _repository.ProductPackage
                    .FindByCondition(c => c.Ddate == null && c.DaDate == null && c.MainProductId == packageId).Include(c => c.DepProduct).Select(
                        c => new { ProductId = c.DepProductId, c.Price, c.DepProduct.Name, c.DepProduct.Coding }).OrderByDescending(c => c.Coding).ToList();

                _logger.LogData(MethodBase.GetCurrentMethod(), res, null, packageId);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), packageId);
                return BadRequest(e.Message);

            }
        }
    }
}
