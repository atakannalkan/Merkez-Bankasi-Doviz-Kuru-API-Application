using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.shared.Utilities.Security.Abstract;
using dovizapp.webapi.DTO;
using dovizapp.webapi.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dovizapp.webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<shared.Entities.User> _userManager;
        private readonly SignInManager<shared.Entities.User> _signInManager;
        private readonly ITokenGenerator _tokenGenerator;
        public AuthController(UserManager<shared.Entities.User> userManager, SignInManager<shared.Entities.User> signInManager, ITokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("HATA!: Kayıt Olunamadı !");
            }

            var user = new shared.Entities.User() {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                UserName = registerDTO.UserName
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                return Ok("Hesabınız başarıyla oluşturuldu, hemen giriş yapabilirsiniz !");
            } else {
                return BadRequest($"HATA!: {string.Join(",", result.Errors.Select(e => e.Description).ToList())}");
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("HATA!: Login Olunamadı !");
            }
            
            var userName = await _userManager.FindByNameAsync(loginDTO.UserName);
            if (userName == null)
            {
                return NotFound("Böyle bir kulanıcı bulunamadı !");
            }

            var result = await _signInManager.PasswordSignInAsync(userName, loginDTO.Password, loginDTO.RememberMe, false);
            var roles = await _userManager.GetRolesAsync(userName);            

            return result.Succeeded ? Ok(_tokenGenerator.CreateToken(userName.UserName, roles.FirstOrDefault())) // Kullanıcının role bilgisini veritabanından alıp Token'a aktarıyorum.
            : BadRequest("Kullanıcı adı veya Şifre hatalı !");
        }
        
    }
}