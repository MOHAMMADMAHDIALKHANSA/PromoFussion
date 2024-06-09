// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using MarketingHub.Data;
using MarketingHub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarketingHub.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context
            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context= context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            public string? Role { get; set; }
            [ValidateNever]
           public IEnumerable<SelectListItem> RoleList { get; set; }
           
            [Required]
            public string Name { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!_roleManager.RoleExistsAsync(MarketingHub.Utility.SD.Role_Cust).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(MarketingHub.Utility.SD.Role_Cust)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(MarketingHub.Utility.SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(MarketingHub.Utility.SD.Role_Agency)).GetAwaiter().GetResult();

            }
            Input = new()
            {
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };


            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);
                user.Name = Input.Name;

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (!String.IsNullOrEmpty(Input.Role))
                    {
                        await _userManager.AddToRoleAsync(user, Input.Role);
                    }
                    else
                    {
                        // Assign user to a default role (e.g., Customer) if no role is specified
                        await _userManager.AddToRoleAsync(user, MarketingHub.Utility.SD.Role_Cust);
                    }

                    // Add specific registration details based on the user's role
                    switch (Input.Role)
                    {
                        case MarketingHub.Utility.SD.Role_Admin:

                            var admin = new Administrator { applicationUser = user, Email=Input.Email,FirstName = Input.Name, Role=Input.Role,ImgUrl= "data:image/png;base64,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" };
                            _context.Administrators.Add(admin);
                            _context.SaveChanges();
                            break;

                        case MarketingHub.Utility.SD.Role_Agency:
                            // Add Marketing Agency-specific registration logic
                            var location = new Location { BuildingName = "default", City = "default", PostalCode = 0, Region = "default", UrlRoute = "default", ImageUrl = "default" };
                            var agency = new MarketingAgency { LocationId=location.LocationId ,Location= location ,Username=Input.Name, applicationUser=user, Email=Input.Email ,ImageUrl= "https://as1.ftcdn.net/v2/jpg/03/46/83/96/1000_F_346839683_6nAPzbhpSkIpb8pmAwufkC7c5eD7wYws.jpg" };
                            _context.MarketingAgency.Add(agency);
                            _context.SaveChanges();
                            break;

                        default:
                            // Add Customer-specific registration logic
                            var customer=new Customer { FirstName=Input.Name,Email=Input.Email,applicationUser=user  };
                            _context.Customers.Add(customer);
                            _context.SaveChanges();
                            break;
                    }

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }


        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
