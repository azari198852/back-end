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
using HandCarftBaseServer.Tools;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Entities.Models;

namespace HandCarftBaseServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILogHandler _logger;

        public EmployeeController(IMapper mapper, IRepositoryWrapper repository, ILogHandler logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// دریافت لیست کاربران  
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("Employee/GetEmployeeList")]
        public IActionResult GetEmployeeList()
        {
            try
            {

                var res = _repository.Employee.FindByCondition(c => c.User.DaDate == null)
                    .Include(c => c.User).ThenInclude(c => c.UserRole).ThenInclude(c => c.RoleNavigation)
                    .Select(c => new
                    {
                        c.Id,
                        c.User.FullName,
                        RoleList = string.Join(',', c.User.UserRole.Select(x => x.RoleNavigation.Name).ToList()),
                        Status = c.User.DaDate == null ? "فعال" : "غیرفعال"
                    })
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
        /// ثبت کاربر  
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("Employee/InsertEmployee")]
        public IActionResult InsertEmployee(EmployeeInsertDto input)
        {
            try
            {

                #region Validation
                var validator = new ParamValidator();
                validator.ValidateNull(input.FirstName, General.Messages_.NullInputMessages_.GeneralNullMessage("نام"))
                    .ValidateNull(input.LastName, General.Messages_.NullInputMessages_.GeneralNullMessage("نام خانوادگی"))
                    .ValidateNull(input.UserName, General.Messages_.NullInputMessages_.GeneralNullMessage("نام کاربری"))
                    .ValidateNull(input.Password, General.Messages_.NullInputMessages_.GeneralNullMessage("رمز عبور"))
                    .Throw(General.Results_.FieldNullErrorCode());

                if (input.RoleList.Count == 0)
                    throw new BusinessException(XError.BusinessErrors.RoleNotSelected());
                if (_repository.Users.FindByCondition(c => c.Username == input.UserName).Any())
                    throw new BusinessException(XError.BusinessErrors.UserAlreadyExists());
                #endregion

                var employee = new Employee();

                var user = new Users
                {
                    Cdate = DateTime.Now.Ticks,
                    CuserId = ClaimPrincipalFactory.GetUserId(User),
                    FullName = input.FirstName + " " + input.LastName,
                    Hpassword = input.Password,
                    Username = input.UserName
                };
                input.RoleList.ForEach(c =>
                {
                    var userRole = new UserRole
                    {
                        Cdate = DateTime.Now.Ticks,
                        CuserId = ClaimPrincipalFactory.GetUserId(User),
                        Role = c
                    };
                    user.UserRole.Add(userRole);
                });
                employee.User = user;

                _repository.Employee.Create(employee);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), employee.Id, null, input);
                return Ok(employee.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), input);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// دریافت اطلاعات کاربر   
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("Employee/GetEmployeeById")]
        public IActionResult GetEmployeeById(long employeeId)
        {
            try
            {

                var res = _repository.Employee.FindByCondition(c => c.Id == employeeId)
                    .Include(c => c.User).ThenInclude(c => c.UserRole).ThenInclude(c => c.RoleNavigation)
                    .Select(c => new
                    {
                        c.Id,
                        c.User.FullName,
                        RoleList = string.Join(',', c.User.UserRole.Select(x => x.RoleNavigation.Name).ToList()),
                        Status = c.User.DaDate == null ? "فعال" : "غیرفعال",
                        c.User.Hpassword,
                        c.User.Username
                    })
                    .FirstOrDefault();


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
        /// ویرایش کاربر  
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("Employee/UpdateEmployee")]
        public IActionResult UpdateEmployee(EmployeeInsertDto input)
        {
            try
            {
                var user = _repository.Employee.FindByCondition(c => c.Id == input.Id).Select(c => c.User)
                    .FirstOrDefault();
                #region Validation
                if (user == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                var validator = new ParamValidator();
                validator.ValidateNull(input.FirstName, General.Messages_.NullInputMessages_.GeneralNullMessage("نام"))
                    .ValidateNull(input.LastName, General.Messages_.NullInputMessages_.GeneralNullMessage("نام خانوادگی"))
                    .ValidateNull(input.UserName, General.Messages_.NullInputMessages_.GeneralNullMessage("نام کاربری"))
                    .ValidateNull(input.Password, General.Messages_.NullInputMessages_.GeneralNullMessage("رمز عبور"))
                    .Throw(General.Results_.FieldNullErrorCode());

                if (input.RoleList.Count == 0)
                    throw new BusinessException(XError.BusinessErrors.RoleNotSelected());
                if (_repository.Users.FindByCondition(c => c.Username == input.UserName && c.Id != user.Id).Any())
                    throw new BusinessException(XError.BusinessErrors.UserAlreadyExists());
                #endregion


                user.Mdate = DateTime.Now.Ticks;
                user.MuserId = ClaimPrincipalFactory.GetUserId(User);
                user.FullName = input.FirstName + " " + input.LastName;
                user.Hpassword = input.Password;
                user.Username = input.UserName;

                var toBeDdeleted = _repository.UserRole.FindByCondition(c => c.UserId == user.Id).ToList();
                _repository.UserRole.DeleteRange(toBeDdeleted);

                input.RoleList.ForEach(c =>
                {
                    var userRole = new UserRole
                    {
                        Cdate = DateTime.Now.Ticks,
                        CuserId = ClaimPrincipalFactory.GetUserId(User),
                        Role = c
                    };
                    user.UserRole.Add(userRole);
                });

                _repository.Users.Update(user);
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
        /// حذف کاربر  
        /// </summary>
        [Authorize]
        [HttpDelete]
        [Route("Employee/DeleteEmployee")]
        public IActionResult DeleteEmployee(long employeeId)
        {
            try
            {
                var user = _repository.Employee.FindByCondition(c => c.Id == employeeId).Select(c => c.User)
                    .FirstOrDefault();

                if (user == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());


                user.Ddate = DateTime.Now.Ticks;
                user.DuserId = ClaimPrincipalFactory.GetUserId(User);


                var toBeDdeleted = _repository.UserRole.FindByCondition(c => c.UserId == user.Id).ToList();
                toBeDdeleted.ForEach(c =>
                {
                    c.Ddate = DateTime.Now.Ticks;
                    c.DuserId = ClaimPrincipalFactory.GetUserId(User);

                });

                _repository.UserRole.UpdateRange(toBeDdeleted);
                _repository.Users.Update(user);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, employeeId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), employeeId);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// فعال /غیر فعال کردن کاربر  
        /// </summary>
        [Authorize]
        [HttpPut]
        [Route("Employee/ActiveDeActiveEmployee")]
        public IActionResult ActiveDeActiveEmployee(long employeeId)
        {
            try
            {
                var user = _repository.Employee.FindByCondition(c => c.Id == employeeId).Select(c => c.User)
                    .FirstOrDefault();

                if (user == null)
                    throw new BusinessException(XError.GetDataErrors.NotFound());

                var toBeDdeleted = _repository.UserRole.FindByCondition(c => c.UserId == user.Id).ToList();
                if (user.DaDate == null)
                {
                    user.DaDate = DateTime.Now.Ticks;
                    toBeDdeleted.ForEach(c =>
                    {
                        c.Ddate = DateTime.Now.Ticks;
                        c.DuserId = ClaimPrincipalFactory.GetUserId(User);

                    });
                }
                else
                {
                    user.DaDate = null;
                    toBeDdeleted.ForEach(c =>
                    {
                        c.Ddate = DateTime.Now.Ticks;
                        c.DuserId = ClaimPrincipalFactory.GetUserId(User);

                    });
                }

                user.DaUserId = ClaimPrincipalFactory.GetUserId(User);

                _repository.UserRole.UpdateRange(toBeDdeleted);
                _repository.Users.Update(user);
                _repository.Save();

                _logger.LogData(MethodBase.GetCurrentMethod(), General.Results_.SuccessMessage(), null, employeeId);
                return Ok(General.Results_.SuccessMessage());
            }
            catch (Exception e)
            {
                _logger.LogError(e, MethodBase.GetCurrentMethod(), employeeId);
                return BadRequest(e.Message);
            }
        }

    }
}
