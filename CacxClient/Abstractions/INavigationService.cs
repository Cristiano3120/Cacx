using CacxClient.MVVM;

namespace CacxClient.Abstractions;

public interface INavigationService
{
    void NavigateToTOS(RegisterViewModel registerState);
    void NavigateToRegister(RegisterViewModel? registerState);
    void NavigateToVerification();
    void NavigateToLogin();
}