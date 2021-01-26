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
using HandCarftBaseServer.Tools;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HandCarftBaseServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class DynamicFormsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILogHandler _logger;

        public DynamicFormsController(IMapper mapper, IRepositoryWrapper repository, ILogHandler logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// لیست صفحات استاتیک  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("DynamicForms/GetStaticFormsList")]
        public IActionResult GetStaticFormsList()
        {
            try
            {

                var res = _repository.DynamicForms.FindByCondition(c => c.DaDate == null && c.Ddate == null)
                    .Include(c => c.DynamiFormImage)
                    .OrderByDescending(c => c.Id).ToList();
                var result = _mapper.Map<List<DynamiFormDto>>(res);

                _logger.LogData(MethodBase.GetCurrentMethod(), result, null);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// دریافت صفحه استاتیک با آیدی  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("DynamicForms/GetStaticFormsById")]
        public IActionResult GetStaticFormsById(long dynamicFormsId)
        {
            try
            {

                var res = _repository.DynamicForms.FindByCondition(c => c.DaDate == null && c.Ddate == null)
                    .Include(c => c.DynamiFormImage).FirstOrDefault();
                    
                var result = _mapper.Map<DynamiFormDto>(res);

                _logger.LogData(MethodBase.GetCurrentMethod(), result, null, dynamicFormsId);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), dynamicFormsId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// حدف صفحه استاتیک  
        /// </summary>
        [Authorize]
        [HttpDelete]
        [Route("DynamicForms/DeleteStaticForms")]
        public IActionResult DeleteStaticForms(long dynamicFormsId)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var res = _repository.DynamicForms.FindByCondition(c => c.Id == dynamicFormsId).FirstOrDefault();
                if (res == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                res.Ddate = DateTime.Now.Ticks;
                res.DuserId = userId;
                _repository.DynamicForms.Update(res);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, dynamicFormsId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), dynamicFormsId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// ثبت صفحه استاتیک  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("DynamicForms/InsertDynamicForms")]
        public IActionResult InsertDynamicForms()
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var dynamicForms = JsonSerializer.Deserialize<DynamiFormDto>(HttpContext.Request.Form["DynamicForms"]);
                var _dynamicForms = _mapper.Map<DynamicForms>(dynamicForms);
                _dynamicForms.Cdate = DateTime.Now.Ticks;
                _dynamicForms.CuserId = userId;


                var fileList = HttpContext.Request.Form.Files.ToList();

                foreach (var uploadFileStatus in fileList.Select(file => FileManeger.FileUploader(file, 1, "DynamicFormImages")))
                {
                    if (uploadFileStatus.Status == 200)
                    {
                        var image = new DynamiFormImage
                        {
                            Cdate = DateTime.Now.Ticks,
                            CuserId = userId,
                            ImageUrl = uploadFileStatus.Path
                        };
                        _dynamicForms.DynamiFormImage.Add(image);
                    }
                    else
                    {
                        throw new BusinessException(uploadFileStatus.Path, 100);
                    }
                }

                _repository.DynamicForms.Create(_dynamicForms);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), _dynamicForms.Id, null);
                return Ok(_dynamicForms.Id);


            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);
            }


        }

        /// <summary>
        /// ویرایش صفحه استاتیک  
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("DynamicForms/UpdateDynamicForms")]
        public IActionResult UpdateDynamicForms(long dynamicFormsId)
        {
            try
            {
                var userId = ClaimPrincipalFactory.GetUserId(User);
                var dynamicForms = JsonSerializer.Deserialize<DynamiFormDto>(HttpContext.Request.Form["DynamicForms"]);
                var _dynamicForms = _repository.DynamicForms.FindByCondition(c => c.Id == dynamicFormsId)
                    .FirstOrDefault();
                if (_dynamicForms == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                _dynamicForms.Mdate = DateTime.Now.Ticks;
                _dynamicForms.MuserId = userId;
                _dynamicForms.Description = dynamicForms.Description;
                _dynamicForms.DescriptionMeta= dynamicForms.DescriptionMeta;
                _dynamicForms.Title = dynamicForms.Title;
                _dynamicForms.TitleMetaData = dynamicForms.TitleMetaData;
                _dynamicForms.KeyWords = dynamicForms.KeyWords;
                


                var fileList = HttpContext.Request.Form.Files.ToList();
                if (fileList.Count > 0)
                {

                    foreach (var uploadFileStatus in fileList.Select(file => FileManeger.FileUploader(file, 1, "DynamicFormImages")))
                    {
                        var tobedeletedList = _repository.DynamiFormImage
                            .FindByCondition(c => c.DynamicFormId == dynamicFormsId).ToList();
                        foreach (var item in tobedeletedList)
                        {
                            item.Ddate = DateTime.Now.Ticks;
                            item.DuserId = userId;
                            _repository.DynamiFormImage.Update(item);
                        }
                        if (uploadFileStatus.Status == 200)
                        {
                            var image = new DynamiFormImage
                            {
                                Cdate = DateTime.Now.Ticks,
                                CuserId = userId,
                                ImageUrl = uploadFileStatus.Path
                            };
                            _dynamicForms.DynamiFormImage.Add(image);
                        }
                        else
                        {
                            throw new BusinessException(uploadFileStatus.Path, 100);
                        }
                    }
                }


                _repository.DynamicForms.Create(_dynamicForms);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null,dynamicFormsId);
                return Ok(General.Results_.SuccessMessage());


            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), dynamicFormsId);
                return BadRequest(e.Message);
            }


        }

    }
}
