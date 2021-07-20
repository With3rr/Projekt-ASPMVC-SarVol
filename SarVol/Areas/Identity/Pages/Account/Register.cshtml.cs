using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SarVol.DataAccess.Repository.IRepository;
using SarVol.Models;
using SarVol.Utility;

namespace SarVol.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
       
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
          RoleManager<IdentityRole> roleManager,IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
          
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
   
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
  
            public string Password { get; set; }

            [DataType(DataType.Password)]
     
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            [Required]
        
            public string Name { get; set; }


          
            public string PhoneNumber { get; set; }

          
            public string Street { get; set; }

           
            public string City { get; set; }

     
            public string PostalCode { get; set; }

    
            public string Country { get; set; }

       
            public int? CompanyId { get; set; }

        
            public string Role { get; set; }

          
            public IEnumerable<SelectListItem> CompanyList { get; set; }

          
            public IEnumerable<SelectListItem> RoleList { get; set; }



        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            
            ReturnUrl = returnUrl;
           
            Input = new InputModel()
            {
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
                RoleList = _roleManager.Roles.Where(i => i.Name != StaticDetails.Role_User_Individual).Select(x => x.Name).Select(i => new SelectListItem { Text = i, Value = i.ToString() })
            };


            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new AppUser 
                {
                    
                    UserName=Input.Email,
                    Email=Input.Email,
                    CompanyId=Input.CompanyId,
                    Street=Input.Street,
                    City=Input.City,
                    Country=Input.Country,
                    PostalCode=Input.PostalCode,
                    Name=Input.Name,
                    PhoneNumber=Input.PhoneNumber,
                    Role=Input.Role
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (!await _roleManager.RoleExistsAsync(StaticDetails.Role_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetails.Role_Employee))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Employee));
                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetails.Role_User_Company))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_User_Company));
                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetails.Role_User_Individual))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_User_Individual));
                    }

                    var ile = _unitOfWork.AppUser.GetAll().Count();
                    //Pierwszy user zostaje Administratorem
                    if (_unitOfWork.AppUser.GetAll().Count() == 1)
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetails.Role_Admin);
                    }
                    else if (user.Role==null)
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetails.Role_User_Individual);
                    }
                    else 
                    {

                        if (user.CompanyId > 0)
                        {
                            await _userManager.AddToRoleAsync(user, StaticDetails.Role_User_Company);
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, user.Role);
                        }

                       
                    }

                    
                   

                  

                   

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);


                    if (user.Role == null)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        ///admin registers new user
                        return RedirectToAction("Index", "User", new { Area = "Admin" });
                    }
                  
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            Input = new InputModel()
            {
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
                RoleList = _roleManager.Roles.Where(i => i.Name != StaticDetails.Role_User_Individual).Select(x => x.Name).Select(i => new SelectListItem { Text = i, Value = i.ToString() })
            };

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
