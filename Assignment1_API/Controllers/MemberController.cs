using Assignment1_API.ViewModel;
using Assignment1_PRN231.Models;
using AutoMapper;
using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using System.Security.Claims;

namespace Assignment1_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService memberService;
        private readonly IMapper mapper;

        public MemberController(IMemberService memberService, IMapper mapper)
        {
            this.memberService = memberService;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            var configuration = builder.Build();
            var admin = configuration.GetSection("Admin");

            var Email = admin["Email"];
            var Password = admin["Password"];
            try
            {
                if (Email == model.Email && Password == model.Password)
                {
                    return Ok(new
                    {
                        Message = "Login Success",
                        Role = "Admin",
                        Data = new { },
                        JwtToken = JWTManage.GetToken("Admin", "Admin")
                    });
                }

                var account = await memberService.CheckLogin(model.Email, model.Password);
                if (account == null)
                {
                    return StatusCode(404, new
                    {
                        Stutus = "Error",
                        Message = "Login Fail"
                    });
                }

                return Ok(new
                {
                    Message = "Login Success",
                    Role = "User",
                    Data = account,
                    JwtToken = JWTManage.GetToken("" + account.MemberId, "User")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMember(int id)
        {
            try
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(role))
                {
                    return StatusCode(401, new
                    {
                        Status = "Error",
                        Message = "You are not login"
                    });
                }

                if (role != "Admin")
                {
                    var memId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                    return Ok(await memberService.Get(int.Parse(memId)));
                }

                if (role == "Admin")
                {
                    return Ok(await memberService.Get(id));
                }

                return StatusCode(404, new
                {
                    Status = "Error",
                    Message = "Not Found"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMember()
        {
            try
            {
                var listMember = memberService.GetAll();
                return Ok(listMember);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(MemberVM model)
        {
            try
            {
                var member = mapper.Map<Member>(model);
                var check = await memberService.Add(member);
                return check ? Ok(new
                {
                    Message = "Add Success",
                    Data = new
                    {
                        Email = model.Email,
                        CompanyName = model.CompanyName,
                        City = model.City,
                        Country = model.Country
                    }
                }) : Ok(new
                {
                    Message = "Add Faill"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateMember(int id, MemberUpdateVM model)
        {
            try
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var memId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(role))
                {
                    return StatusCode(401, new
                    {
                        Status = "Error",
                        Message = "You are not login"
                    });
                }

                if (role == "Admin" || (role != "Admin" && id == int.Parse(memId)))
                {
                    var member = mapper.Map<Member>(model);
                    var check = await memberService.Update(member);
                    return check ? Ok(new
                    {
                        Message = "Update Success",
                        Data = new
                        {
                            Email = model.Email,
                            CompanyName = model.CompanyName,
                            City = model.City,
                            Country = model.Country
                        }
                    }) : Ok(new
                    {
                        Message = "Update Faill"
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        Status = "Error",
                        Message = "Role Denied"
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMember(int id)
        {
            try
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(role))
                {
                    return StatusCode(401, new
                    {
                        Status = "Not Login"
                    });
                }
                if (role == "Admin")
                {
                    if (id == null)
                    {
                        return StatusCode(404, new
                        {
                            Status = "Error",
                            Message = "Not Found",
                        });
                    }
                    var check = await memberService.Delete(id);
                    return check ? Ok(new
                    {
                        Message = "Delete Success",
                    }) : Ok(new
                    {
                        Message = "Delete Faill"
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        Status = "Error",
                        Message = "Role Denied",
                    });
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }
    }
}
