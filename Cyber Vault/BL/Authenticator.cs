namespace Cyber_Vault.BL;

internal class Authenticator
{
    public string? Type
    {
        get; set;
    }

    public string? Label
    {
        get; set;
    }

    public string? Issuer
    {
        get; set;
    }

    public string? Secret
    {
        get; set;
    }

    public string? Digits
    {
        get; set;
    }

    public string? Algorithm
    {
        get; set;
    }


    public string? Period
    {
        get; set;
    }

    public string? Counter
    {    
        get; set;
    }


    public Authenticator()
    {

    }

    // to string
    public override string ToString()
    {    
        return $"{Type} - {Label} - {Issuer} - {Secret} - {Digits} - {Algorithm} - {Period} - {Counter}";
    }


    
}
