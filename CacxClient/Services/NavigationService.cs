using CacxClient.Abstractions;
using CacxClient.Extensions;
using CacxClient.MVVM;
using CacxClient.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace CacxClient.Services;

public sealed class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    public void NavigateToVerification()
    {
        VerificationViewModel verificationViewModel = serviceProvider.GetRequiredService<VerificationViewModel>();
        VerificationWindow verificationWindow = new(verificationViewModel);

        verificationWindow.SwitchTo();
    }

    public void NavigateToLogin()
    {
        LoginViewModel loginViewModel = serviceProvider.GetRequiredService<LoginViewModel>();
        LoginWindow loginWindow = new(loginViewModel);

        loginWindow.SwitchTo();
    }

    public void NavigateToRegister(RegisterViewModel? registerState)
    {
        registerState ??= serviceProvider.GetRequiredService<RegisterViewModel>();
        RegisterWindow registerWindow = new(registerState);

        registerWindow.SwitchTo();
    }

    public void NavigateToTOS(RegisterViewModel registerState)
    {
        TosViewModel tosViewModel = serviceProvider.GetRequiredService<TosViewModel>();
        tosViewModel.Activate(registerState);

        TOSWindow tosWindow = new(tosViewModel);
        tosWindow.SwitchTo();
    }
}
