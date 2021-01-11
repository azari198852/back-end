using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Params;
using Entities.UIResponse;
using HandCarftBaseServer.Tools;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HandCarftBaseServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        public IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILogHandler _logger;

        public LocationController(IMapper mapper, IRepositoryWrapper repository, ILogHandler logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        #region UI_Methods

        /// <summary>
        ///لیست کشورها 
        /// </summary>
        [HttpGet]
        [Route("Location/GetCountryList_UI")]
        public ListResult<LocationDto> GetCountryList_UI()
        {

            try
            {
                var countrylist = _repository.Location.GetCountryList().ToList();
                var result = _mapper.Map<List<LocationDto>>(countrylist);
                var finalresult = ListResult<LocationDto>.GetSuccessfulResult(result);
                return finalresult;
            }
            catch (Exception e)
            {
                return ListResult<LocationDto>.GetFailResult(null);
            }
        }

        /// <summary>
        ///لیست استان ها براساس آیدی کشور 
        /// </summary>
        [HttpGet]
        [Route("Location/GetProvinceList_UI")]
        public ListResult<LocationDto> GetProvinceList_UI(long? countryId)
        {
            try
            {
                var provincelist = _repository.Location.GetProvinceList(countryId).ToList();
                var result = _mapper.Map<List<LocationDto>>(provincelist);
                var finalresult = ListResult<LocationDto>.GetSuccessfulResult(result);
                return finalresult;

            }
            catch (Exception e)
            {
                return ListResult<LocationDto>.GetFailResult(null);
            }

        }

        /// <summary>
        ///لیست شهرها براساس آیدی استان 
        /// </summary>
        [HttpGet]
        [Route("Location/GetCityList_UI")]
        public ListResult<LocationDto> GetCityList_UI(long provinceId)
        {
            try
            {
                var citylist = _repository.Location.GetCityList(provinceId).ToList();
                var result = _mapper.Map<List<LocationDto>>(citylist);
                var finalresult = ListResult<LocationDto>.GetSuccessfulResult(result);
                return finalresult;
            }
            catch (Exception e)
            {
                return ListResult<LocationDto>.GetFailResult(null);
            }



        }


        #endregion

        /// <summary>
        /// دریافت لیست کشورها  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("Location/GetCountryList")]
        public IActionResult GetCountryList()
        {
            try
            {
                var res = _repository.Location.FindByCondition(c => c.Ddate == null && c.DaDate == null && c.Pid == null).Select(c => new { c.Id, c.Name, c.LocationCode }).ToList();
                _logger.LogData(MethodBase.GetCurrentMethod(), res, null, res);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// ثبت کشور  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("Location/InsertCountry")]
        public IActionResult InsertCountry(CountryInsertDto input)
        {
            try
            {
                var loc = new Location
                {
                    Name = input.Name,
                    EnName = input.EnName,
                    Pid = null,
                    LocationCode = input.LocationCode,
                    PostCode = input.PostCode,
                    CuserId = ClaimPrincipalFactory.GetUserId(User),
                    Cdate = DateTime.Now.Ticks
                };
                _repository.Location.Create(loc);
                _logger.LogData(MethodBase.GetCurrentMethod(), loc.Id, null, input);
                return Ok(loc.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// حذف کشور  
        /// </summary>
        [Authorize]
        [HttpDelete]
        [Route("Location/DeleteCountry")]
        public IActionResult DeleteCountry(long countryId)
        {
            try
            {
                var loc = _repository.Location.FindByCondition(c => c.Id == countryId).FirstOrDefault();
                if (loc == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                loc.Ddate = DateTime.Now.Ticks;
                loc.DuserId = ClaimPrincipalFactory.GetUserId(User);
                _repository.Location.Update(loc);
                _repository.Save();
                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, countryId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), countryId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// ویرایش کشور  
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("Location/UpadteCountry")]
        public IActionResult UpadteCountry([FromBody] CountryInsertDto input, [FromQuery] long countryId)
        {
            try
            {

                var loc = _repository.Location.FindByCondition(c => c.Id == countryId).FirstOrDefault();
                if (loc == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                loc.Name = input.Name;
                loc.EnName = input.EnName;
                loc.Pid = null;
                loc.LocationCode = input.LocationCode;
                loc.PostCode = input.PostCode;
                loc.MuserId = ClaimPrincipalFactory.GetUserId(User);
                loc.Mdate = DateTime.Now.Ticks;

                _repository.Location.Update(loc);
                _logger.LogData(MethodBase.GetCurrentMethod(), loc.Id, null, input, countryId);
                return Ok(loc.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input, countryId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// دریافت اطلاعات کشور با کد  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("Location/GetCountryById")]
        public IActionResult GetCountryByLocationCode(long locationId)
        {
            try
            {
                var loc = _repository.Location.FindByCondition(c => c.Id == locationId).Select(c => new { c.Id, c.LocationCode, c.Name }).FirstOrDefault();
                if (loc == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                _logger.LogData(MethodBase.GetCurrentMethod(), loc, null, locationId);
                return Ok(loc);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), locationId);
                return BadRequest(e.Message);
            }
        }
    }
}
