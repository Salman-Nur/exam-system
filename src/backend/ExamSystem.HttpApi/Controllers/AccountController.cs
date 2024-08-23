using ExamSystem.Application.Common.Options;
using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Application.MembershipFeatures.Enums;
using ExamSystem.HttpApi.Others;
using ExamSystem.HttpApi.RequestHandlers;
using ExamSystem.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharpOutcome.Helpers;

namespace ExamSystem.HttpApi.Controllers;

[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GoogleRecaptchaOptions _googleRecaptchaOptions;
    private readonly IServiceProvider _serviceProvider;

    public AccountController(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory,
        IOptions<GoogleRecaptchaOptions> googleRecaptchaOptions)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
        _googleRecaptchaOptions = googleRecaptchaOptions.Value;
    }

    [HttpPost("signup")]
    [AllowAnonymous]
    [ValidateAngularXsrfToken]
    [ValidationActionFilter<MemberSignupRequest>]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Signup(MemberSignupRequest dto)
    {
        var handler = _serviceProvider.GetRequiredService<SignupRequestHandler>();

        if (await VerifyRecaptchaV3Token(dto.RecaptchaV3ResponseCode) is false)
        {
            return Unauthorized("There was an issue with verifying recaptcha token");
        }

        var result = await handler.ConductSignupAsync(_serviceProvider, dto);

        if (result.TryPickGoodOutcome(out _, out var error))
        {
            return ControllerContext.MakeResponse(StatusCodes.Status201Created);
        }

        if (error.Reason is MembershipErrorReason.DuplicateEmail)
        {
            return Conflict("Email already taken.");
        }

        return BadRequest(new { errors = error.Errors.Select(x => x.Value) });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ValidateAngularXsrfToken]
    [ValidationActionFilter<MemberLoginRequest>]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(MemberLoginRequest dto)
    {
        if (await VerifyRecaptchaV3Token(dto.RecaptchaV3ResponseCode) is false)
        {
            return Unauthorized("There was an issue with verifying recaptcha token");
        }

        var handler = _serviceProvider.GetRequiredService<LoginRequestHandler>();
        var result = await handler.HandleAsync(_serviceProvider, dto, HttpContext.RequestAborted);

        return result.Match(OnGoodOutcome, OnBadOutcome);

        IActionResult OnGoodOutcome((string token, DateTime duration, CookieOptions cookieOptions) data)
        {
            HttpContext.Response.Cookies.Append(
                InfrastructureConstants.AccessTokenCookieKey,
                data.token,
                data.cookieOptions
            );

            return Ok();
        }

        IActionResult OnBadOutcome(BadOutcomeTag tag)
        {
            return tag switch
            {
                BadOutcomeTag.NotFound => Unauthorized("Invalid login attempt"),
                BadOutcomeTag.Unauthorized => Unauthorized("Invalid login attempt"),
                BadOutcomeTag.NotVerified => Unauthorized("Account not confirmed"),
                _ => ControllerContext.MakeResponse(StatusCodes.Status500InternalServerError)
            };
        }
    }

    [HttpPost("confirm")]
    [ValidateAngularXsrfToken]
    [ValidationActionFilter<MemberConfirmAccountRequest>]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConfirmAccount(MemberConfirmAccountRequest dto)
    {
        if (await VerifyRecaptchaV3Token(dto.RecaptchaV3ResponseCode) is false)
        {
            return Unauthorized("There was an issue with verifying recaptcha token");
        }

        var handler = _serviceProvider.GetRequiredService<ConfirmAccountRequestHandler>();
        var result = await handler.ConductConfirmationAsync(_serviceProvider, dto);

        return result.TryPickBadOutcome(out var err)
            ? ControllerContext.MakeResponse(StatusCodes.Status400BadRequest, err.Reason)
            : Ok();
    }

    [HttpPost("check-token")]
    [Authorize]
    [ValidateAngularXsrfToken]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult CheckAccessToken()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(claims);
    }

    [HttpPost("resend-verification-code")]
    [ValidateAngularXsrfToken]
    [ValidationActionFilter<MemberResendVerificationCodeRequest>]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResendVerificationCode(MemberResendVerificationCodeRequest dto)
    {
        if (await VerifyRecaptchaV3Token(dto.RecaptchaV3ResponseCode) is false)
        {
            return Unauthorized("There was an issue with verifying recaptcha token");
        }

        var handler = _serviceProvider.GetRequiredService<ResendVerificationCodeRequestHandler>();
        var result = await handler.ConductResendVerificationCodeAsync(_serviceProvider, dto,
            HttpContext.RequestAborted);

        return result.TryPickBadOutcome(out var err)
            ? ControllerContext.MakeResponse(StatusCodes.Status400BadRequest, err)
            : Ok();
    }

    [HttpPost("forgot-password")]
    [ValidateAngularXsrfToken]
    [ValidationActionFilter<MemberForgotPasswordRequest>]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ForgotPassword(MemberForgotPasswordRequest dto)
    {
        if (await VerifyRecaptchaV3Token(dto.RecaptchaV3ResponseCode) is false)
        {
            return Unauthorized("There was an issue with verifying recaptcha token");
        }

        var handler = _serviceProvider.GetRequiredService<ForgotPasswordRequestHandler>();
        await handler.ConductForgotPasswordRequestAsync(_serviceProvider, dto.Email,
            HttpContext.RequestAborted);

        return Ok();
    }

    [HttpPost("reset-password")]
    [ValidateAngularXsrfToken]
    [ValidationActionFilter<MemberResetPasswordRequest>]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResetPassword(MemberResetPasswordRequest dto)
    {
        if (await VerifyRecaptchaV3Token(dto.RecaptchaV3ResponseCode) is false)
        {
            return Unauthorized("There was an issue with verifying recaptcha token");
        }

        var handler = _serviceProvider.GetRequiredService<ResetPasswordRequestHandler>();
        var result = await handler.ConductResetPasswordAsync(_serviceProvider, dto);

        return result
            ? Ok()
            : ControllerContext.MakeResponse(StatusCodes.Status400BadRequest);
    }

    [HttpPost("logout")]
    [Authorize]
    [ValidateAngularXsrfToken]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete(InfrastructureConstants.AccessTokenCookieKey);
        HttpContext.Response.Cookies.Delete(InfrastructureConstants.XsrfTokenCookieKey);
        return Ok();
    }

    private async Task<bool> VerifyRecaptchaV3Token(string token)
    {
        try
        {
            using var httpClient = _httpClientFactory.CreateClient(GoogleRecaptchaOptions.SectionName);
            var response = await httpClient.PostAsync(
                $"?secret={_googleRecaptchaOptions.SecretKey}&response={token}", null
            );
            return response.IsSuccessStatusCode;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }
}
