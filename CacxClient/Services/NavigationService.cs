using CacxClient.Abstractions;
using CacxClient.Extensions;
using CacxClient.MVVM;
using CacxClient.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace CacxClient.Services;

public sealed class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    public void NavigateToVerification(string token)
    {
        VerificationViewModel verificationViewModel = serviceProvider.GetRequiredService<VerificationViewModel>();
        verificationViewModel.Activate(token);

        VerificationWindow verificationWindow = new()
        {
            DataContext = verificationViewModel,
        };

        verificationWindow.SwitchTo();
    }

    public void NavigateToLogin()
    {
        LoginViewModel loginViewModel = serviceProvider.GetRequiredService<LoginViewModel>();
        LoginWindow loginWindow = new()
        {
            DataContext = loginViewModel,
        };

        loginWindow.SwitchTo();
    }

    public void NavigateToRegister(RegisterViewModel? registerState)
    {
        registerState ??= serviceProvider.GetRequiredService<RegisterViewModel>();
        RegisterWindow registerWindow = new()
        {
            DataContext = registerState,
        };

        registerWindow.SwitchTo();
    }

    public void NavigateToTOS(RegisterViewModel registerState)
    {
        TosViewModel tosViewModel = serviceProvider.GetRequiredService<TosViewModel>();
        tosViewModel.Activate(registerState);

        TOSWindow tosWindow = new()
        { 
            DataContext = tosViewModel
        };

        tosWindow.SwitchTo();
    }
}
