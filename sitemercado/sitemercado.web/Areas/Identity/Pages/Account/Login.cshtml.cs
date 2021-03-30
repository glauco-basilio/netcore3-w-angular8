using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using sitemercado.web.Models;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace sitemercado.web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [MaxLength(20)]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }


        private class RespostaLoginSiteMercado
        {
            public bool success  { get; set; }

            public  string error { get; set; }
        } 

        private async Task<RespostaLoginSiteMercado> ValidaUsuario(string user, string pwd) 
        { 
            HttpClient c = new HttpClient();
            
            
            c.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", BasicAuthenticationHeaderValue.EncodeCredential(user, pwd));
            var t = c.PostAsync("https://dev.sitemercado.com.br/api/login",null);
            
            var json = await (await t).Content.ReadAsStringAsync();

            
            return JsonConvert.DeserializeObject<RespostaLoginSiteMercado>(json) ;


        }

        readonly string _perfilUnico = "perfilunico";
        private async Task AssertRoleUnica() {

            if(!await _roleManager.RoleExistsAsync(_perfilUnico)) 
            {
                await _roleManager.CreateAsync( new IdentityRole( _perfilUnico) );
            }
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {


                var resposta  = await ValidaUsuario(Input.Name, Input.Password);

                if (resposta.success) 
                { 
                    if(await _userManager.FindByNameAsync(Input.Name) == null) 
                    {
                       var identity =  await _userManager.CreateAsync(new ApplicationUser { UserName = Input.Name }, Input.Password);
                       var user = _userManager.Users.FirstOrDefault(x=>x.UserName == Input.Name);
                            
                            //FindByLoginAsync(Input.Name);

                        await AssertRoleUnica();

                       await _userManager.AddToRoleAsync(user,_perfilUnico);
                    }
                }
                else 
                {
                    ModelState.AddModelError(string.Empty, "Login ou senha não conferem.");
                    return Page();
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Name, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login ou senha não conferem.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
