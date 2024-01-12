namespace Cyber_Vault.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
