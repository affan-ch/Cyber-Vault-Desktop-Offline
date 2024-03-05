
namespace Cyber_Vault.BL;

internal class AccountThumbs
{
    public int? Id
    {
        get; set;
    }

    public string? Domain
    {
        get; set;
    }

    public string? Image
    {
        get; set;
    }

    public AccountThumbs()
    {
    }

    public AccountThumbs(int? id, string? domain, string? image)
    {
        Id = id;
        Domain = domain;
        Image = image;
    }

    public AccountThumbs(string? domain, string? image)
    {
        Domain = domain;
        Image = image;
    }
}
