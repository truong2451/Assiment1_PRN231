using Assignment1_PRN231.Models;
using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Assignment1_PRN231.Controllers
{
    public class MembersController : Controller
    {
        private readonly HttpClient client = null;
        private string MemberApiUrl = "https://localhost:7113/api/Member/";

        public MembersController()
        {
            client = new HttpClient();
        }

        public IActionResult Login()
        {
            try
            {
                if (!string.IsNullOrEmpty(TempData["LoginFail"] as string))
                {
                    throw new Exception(TempData["LoginFail"] as string);
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrMsg"] = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("https://localhost:7113/api/Member/Login", new { email = email, password = password });
            //HttpResponseMessage response = await client.PostAsJsonAsync("https://localhost:7113/api/Member/Login", $"?email={email}&password={password}");
            string strData = await response.Content.ReadAsStringAsync();

            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(strData);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                TempData["LoginFail"] = json["message"].ToString();
                return RedirectToAction(nameof(Login));
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var tokenJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(json["jwtToken"].ToString());
                HttpContext.Session.SetString("Token", tokenJson["token"].ToString());
                HttpContext.Session.SetString("RefreshToken", tokenJson["refreshToken"].ToString());

                HttpContext.Session.SetString("Role", json["role"].ToString());

                if (json["role"].ToString() == "Admin")
                {
                    HttpContext.Session.SetString("Name", "Admin");
                    return RedirectToAction("ViewProduct", "Products");
                }
                if (json["role"].ToString() == "User")
                {
                    var userJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(json["data"].ToString());
                    HttpContext.Session.SetString("Id", userJson["memberId"].ToString());
                    HttpContext.Session.SetString("Name", "User");
                    return RedirectToAction("ViewProductUser", "Products");
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Role")))
            {
                HttpContext.Session.Remove("Token");
                HttpContext.Session.Remove("RefreshToken");
                HttpContext.Session.Remove("Role");
                HttpContext.Session.Remove("Id");
                HttpContext.Session.Remove("Name");
            }

            return RedirectToAction(nameof(Login));
        }

        // Handle action user

        [HttpGet]
        public async Task<IActionResult> ViewProfile()
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            var userId = HttpContext.Session.GetString("Id");
            if (role == "User")
            {
                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.GetAsync(MemberApiUrl + $"GetMember?id={userId}");
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                Member member = System.Text.Json.JsonSerializer.Deserialize<Member>(strData, options);

                return View(member);
            }
            else
            {
                return RedirectToAction(nameof(Login));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(int id, Member member)
        {
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.PutAsJsonAsync(MemberApiUrl + $"UpdateMember?id={member.MemberId}", member);
            string strData = await response.Content.ReadAsStringAsync();

            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(strData);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                TempData["LoginFail"] = "You are not login";
                return RedirectToAction(nameof(Login));
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (json["message"].ToString() == "Update Success")
                {
                    ViewData["Message"] = "Update Success";
                    return RedirectToAction("ViewProductUser", "Products");
                }
                else
                {
                    return View();
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new Exception(json["message"].ToString());
            }

            return RedirectToAction(nameof(ViewProfile));
        }


        // Handle action admin

        [HttpGet]
        public async Task<IActionResult> ViewMember()
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            if (role == "Admin")
            {
                HttpResponseMessage response = await client.GetAsync(MemberApiUrl + "GetAllMember");
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                List< Member> member = System.Text.Json.JsonSerializer.Deserialize<List<Member>>(strData, options);

                return View(member);
            }
            else
            {
                return RedirectToAction(nameof(Login));
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditMember(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            if (role == "Admin")
            {
                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync(MemberApiUrl + $"GetMember?id={id}");
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                Member member = System.Text.Json.JsonSerializer.Deserialize<Member>(strData, options);

                return View(member);
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMember(int id, Member member)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.PutAsJsonAsync(MemberApiUrl + $"UpdateMember?id={member.MemberId}", member);
                string json = await response.Content.ReadAsStringAsync();

                var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["LoginFail"] = "You are not login";
                    return RedirectToAction("Login", "Members");
                }
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (values["message"].ToString() == "Update Success")
                    {
                        return RedirectToAction(nameof(ViewMember));
                    }
                    else
                    {
                        return View();
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new Exception(values["message"].ToString());
                }
                return RedirectToAction(nameof(ViewMember));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(EditMember));
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateMember()
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            if (role == "Admin")
            {
                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMember(Member member)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.PostAsJsonAsync(MemberApiUrl + "AddMember", member);
                string strData = await response.Content.ReadAsStringAsync();

                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(strData);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["LoginFail"] = "You are not login";
                    return RedirectToAction("Login", "Members");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (json["message"].ToString() == "Add Success")
                    {
                        return RedirectToAction(nameof(ViewMember));
                    }
                    else
                    {
                        return View();
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new Exception(json["message"].ToString());
                }
                return RedirectToAction(nameof(ViewMember));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(CreateMember));
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMem(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            if (role == "Admin")
            {
                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync(MemberApiUrl + $"GetMember?id={id}");
                string strData = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                Member member = System.Text.Json.JsonSerializer.Deserialize<Member>(strData, options);

                return View(member);
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        [HttpPost, ActionName("DeleteMem")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.DeleteAsync(MemberApiUrl + $"DeleteMember?id={id}");
                string strData = await response.Content.ReadAsStringAsync();

                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(strData);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["LoginFail"] = "You are not login";
                    return RedirectToAction("Login", "Members");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (json["message"].ToString() == "Delete Success")
                    {
                        return RedirectToAction(nameof(ViewMember));
                    }
                    else
                    {
                        return View();
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new Exception(json["message"].ToString());
                }
                return RedirectToAction(nameof(ViewMember));
            }
            catch (Exception ex)
            {
                return View();
            }

        }
    }
}