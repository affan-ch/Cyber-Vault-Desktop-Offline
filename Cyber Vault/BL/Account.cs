
namespace Cyber_Vault.BL;

internal class Account
{
    public int? Id
    {
        get; set; 
    }

    public string? Type
    {
        get; set; 
    }

    public string? Title
    {
        get; set; 
    }

    public string? Domain
    {
        get; set; 
    }

    public string? Name
    {
        get; set; 
    }

    public string? Email
    {
        get; set; 
    }

    public string? Username
    {
        get; set; 
    }

    public string? PhoneNumber
    {
        get; set; 
    }

    public string? Password
    {
        get; set; 
    }

    public string? Pin
    {
        get; set; 
    }

    public string? DateOfBirth
    {
        get; set; 
    }

    public string? RecoveryEmail
    {
        get; set; 
    }

    public string? RecoveryPhoneNumber
    {
        get; set; 
    }

    public string? QrCode
    {
        get; set; 
    }

    public string? Secret
    {
        get; set; 
    }

    public string? Notes
    {
        get; set; 
    }

    public Account(int Id, string Type, string Title, string Domain, string Name, string Email,
        string Username, string PhoneNumber, string Password, string Pin, string DateOfBirth,
        string RecoveryEmail, string RecoveryPhoneNumber, string QrCode, string Secret, string Notes)
    {
        this.Id = Id;
        this.Type = Type;
        this.Title = Title;
        this.Domain = Domain;
        this.Name = Name;
        this.Email = Email;
        this.Username = Username;
        this.PhoneNumber = PhoneNumber;
        this.Password = Password;
        this.Pin = Pin;
        this.DateOfBirth = DateOfBirth;
        this.RecoveryEmail = RecoveryEmail;
        this.RecoveryPhoneNumber = RecoveryPhoneNumber;
        this.QrCode = QrCode;
        this.Secret = Secret;
        this.Notes = Notes;
    }

    public Account(string Type, string Title, string Domain, string Name, string Email,
    string Username, string PhoneNumber, string Password, string Pin, string DateOfBirth,
    string RecoveryEmail, string RecoveryPhoneNumber, string QrCode, string Secret, string Notes)
    {
        this.Type = Type;
        this.Title = Title;
        this.Domain = Domain;
        this.Name = Name;
        this.Email = Email;
        this.Username = Username;
        this.PhoneNumber = PhoneNumber;
        this.Password = Password;
        this.Pin = Pin;
        this.DateOfBirth = DateOfBirth;
        this.RecoveryEmail = RecoveryEmail;
        this.RecoveryPhoneNumber = RecoveryPhoneNumber;
        this.QrCode = QrCode;
        this.Secret = Secret;
        this.Notes = Notes;
    }
}
