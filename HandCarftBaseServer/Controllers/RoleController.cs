using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class RoleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILogHandler _logger;

        public RoleController(IMapper mapper, IRepositoryWrapper repository, ILogHandler logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// دریافت لیست گروه کاربران  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("Role/GetRoleList")]
        public IActionResult GetRoleList()
        {
            try
            {

                var res = _repository.Role.FindByCondition(c => c.Ddate == null && c.DaDate == null)
                    .Include(c => c.CatRole)
                    .Select(c => new { c.Id, c.Name, c.Rkey, CatRole = c.CatRole.Name })
                    .ToList();


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
        /// ثبت گروه کاربری  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("Role/InsertRole")]
        public IActionResult InsertRole(InsertRoleDto input)
        {
            try
            {

                #region Validation
                var validator = new ParamValidator();
                validator.ValidateNull(input.Name, General.Messages_.NullInputMessages_.GeneralNullMessage("عنوان"))
                    .Throw(General.Results_.FieldNullErrorCode());

                #endregion

                var role = new Role
                {
                    Cdate = DateTime.Now.Ticks,
                    CuserId=ClaimPrincipalFactory.GetUserId(User),
                    Name = input.Name,
                    Rkey = input.Rkey
                };
                _repository.Role.Create(role);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), role.Id, null, input);
                return Ok(role.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// حذف گروه کاربری  
        /// </summary>
        [Authorize]
        [HttpDelete]
        [Route("Role/DeleteRole")]
        public IActionResult DeleteRole(long roleId)
        {
            try
            {
                var role = _repository.Role.FindByCondition(c => c.Id == roleId).FirstOrDefault();
                #region Validation
                if(role==null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                if(_repository.UserRole.FindByCondition(c=>c.Role==roleId && c.Ddate==null && c.DaDate==null).Any())
                    throw new BusinessException("برای این گروه کاربری ، کاربر وچود دارد",4008);
                #endregion

                role.Ddate = DateTime.Now.Ticks;
                role.DuserId = ClaimPrincipalFactory.GetUserId(User);
                
                _repository.Role.Update(role);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, roleId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), roleId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// ویرایش گروه کاربری  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("Role/UpdatetRole")]
        public IActionResult UpdatetRole([FromBody]InsertRoleDto input,[FromQuery] long roleId)
        {
            try
            {
                var role = _repository.Role.FindByCondition(c => c.Id == roleId).FirstOrDefault();
                #region Validation
                var validator = new ParamValidator();
                validator.ValidateNull(input.Name, General.Messages_.NullInputMessages_.GeneralNullMessage("عنوان"))
                    .Throw(General.Results_.FieldNullErrorCode());

                if(role==null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());
                #endregion


                role.Mdate = DateTime.Now.Ticks;
                role.MuserId = ClaimPrincipalFactory.GetUserId(User);
                role.Name = input.Name;
                role.Rkey = input.Rkey;
             
                _repository.Role.Update(role);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, input, roleId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input, roleId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// دریافت گروه کاربری با آیدی  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("Role/GetRoleById")]
        public IActionResult GetRoleById(long roleId)
        {
            try
            {

                var res = _repository.Role.FindByCondition(c => c.Id==roleId)
                    .Select(c => new { c.Id, c.Name, c.Rkey})
                    .FirstOrDefault();


                _logger.LogData(MethodBase.GetCurrentMethod(), res, null, roleId);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), roleId);
                return BadRequest(e.Message);
            }
        }
    }
}
