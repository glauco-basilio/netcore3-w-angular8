//using Microsoft.AspNet.Identity;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//using sitemercado.web.Models;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using coreIdentity = Microsoft.AspNetCore.Identity;

//namespace sitemercado.web.Managers
//{
//    public class CustomSignInManager : SignInManager<ApplicationUser>
//    {
//        public CustomSignInManager(coreIdentity.UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, 
//            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<ApplicationUser>> logger, 
//            IAuthenticationSchemeProvider schemes, 
//            IUserConfirmation<ApplicationUser> confirmation) : base(
//                userManager, contextAccessor,
//                claimsFactory, optionsAccessor, logger,
//                schemes,
//                confirmation
//            )
//        {
//            this.ClaimsFactory = new CustomClaimsPrincipalFactory();
//            userManager.UserValidators
//        }

//        protected override Task<SignInResult> PreSignInCheck(ApplicationUser user)
//        {
//            return Task.FromResult(SignInResult.Success);
//            //return base.PreSignInCheck(user);
//        }


//    }

//    public class CustomUserManager : coreIdentity.UserManager<ApplicationUser>
//    {
//        public CustomUserManager(
//            coreIdentity.IUserStore<ApplicationUser> store, 
//            IOptions<IdentityOptions> optionsAccessor, 
//            IPasswordHasher<ApplicationUser> passwordHasher, 
//            IEnumerable<IUserValidator<ApplicationUser>> userValidators, 
//            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, 
//            ILookupNormalizer keyNormalizer, 
//            IdentityErrorDescriber errors, 
//            IServiceProvider services, 
//            ILogger<coreIdentity.UserManager<ApplicationUser>> logger) :
//            base(
//                store,
//                optionsAccessor,
//                passwordHasher,
//                userValidators,
//                passwordValidators,
//                keyNormalizer, 
//                errors,
//                services,
//                logger
//            )
//        {
//            passwordValidators = new List<CustomPassowrdValidator>();
//        }


//    }

//    public class CustomPassowrdValidator: IPasswordValidator<ApplicationUser>
//    { 
        
//    }

//    public class CustomClaimsPrincipalFactory : IUserClaimsPrincipalFactory<ApplicationUser>
//    {
//        public Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
//        {
//            var result =  new ClaimsIdentity(
//              new[] { 
//                  // adding following 2 claim just for supporting default antiforgery provider
//                  new Claim(ClaimTypes.NameIdentifier, user.UserName),
//                  new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

//                  new Claim(ClaimTypes.Name,user.UserName),

//                  // optionally you could add roles if any
//                  new Claim(ClaimTypes.Role, "RoleName"),
//                  new Claim(ClaimTypes.Role, "AnotherRole"),

//              },
//              DefaultAuthenticationTypes.ApplicationCookie);
            
//            return Task.FromResult(new ClaimsPrincipal(result));
//        }
//    }


//    /*
//    if (new UserManager().IsValid(username, password))
//    {
//        var ident = new ClaimsIdentity(
//          new[] { 
//              // adding following 2 claim just for supporting default antiforgery provider
//              new Claim(ClaimTypes.NameIdentifier, username),
//              new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

//              new Claim(ClaimTypes.Name,username),

//              // optionally you could add roles if any
//              new Claim(ClaimTypes.Role, "RoleName"),
//              new Claim(ClaimTypes.Role, "AnotherRole"),

//          },
//          DefaultAuthenticationTypes.ApplicationCookie);

//        HttpContext.GetOwinContext().Authentication.SignIn(
//           new AuthenticationProperties { IsPersistent = false }, ident);
//        return RedirectToAction("MyAction"); // auth succeed 
//    }
//     */
//}
