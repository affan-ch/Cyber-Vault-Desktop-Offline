
namespace Cyber_Vault.BL;

internal class CreditCard
{
    public int? Id
    {
        get; set;
    }

    public string? CardHolderName
    {
        get; set;
    }

    public string? CardNumber
    {
        get; set;
    }

    public string? ExpiryMonth
    {
        get; set;
    }

    public string? ExpiryYear
    {
        get; set;
    }

    public string? CVV
    {
        get; set;
    }

    public string? Pin
    {
        get; set;
    }

    public string? CardIssuer
    {
        get; set;
    }

    public string? CardType
    {
        get; set;
    }

    public string? BillingAddress
    {
        get; set;
    }

    public string? City
    {
        get; set;
    }

    public string? State
    {
        get; set;
    }

    public string? ZipCode
    {
        get; set;
    }

    public string? Country
    {
        get; set;
    }

    public string? Notes
    {
        get; set;
    }

    public string? DateAdded
    {
        get; set;
    }

    public string? DateModified
    {
        get; set;
    }


    public CreditCard(int? Id, string? CardHolderName, string? CardNumber, string? ExpiryMonth, string? ExpiryYear, string? CVV, string? Pin, string? CardIssuer, string? CardType, string? BillingAddress, string? City, string? State, string? ZipCode, string? Country, string? Notes, string? DateAdded, string? DateModified)
    {
        this.Id = Id;
        this.CardHolderName = CardHolderName;
        this.CardNumber = CardNumber;
        this.ExpiryMonth = ExpiryMonth;
        this.ExpiryYear = ExpiryYear;
        this.CVV = CVV;
        this.Pin = Pin;
        this.CardIssuer = CardIssuer;
        this.CardType = CardType;
        this.BillingAddress = BillingAddress;
        this.City = City;
        this.State = State;
        this.ZipCode = ZipCode;
        this.Country = Country;
        this.Notes = Notes;
        this.DateAdded = DateAdded;
        this.DateModified = DateModified;
    }

    public CreditCard(int? Id, string? CardHolderName, string? CardNumber, string? ExpiryMonth, string? ExpiryYear, string? CVV, string? Pin, string? CardIssuer, string? CardType, string? BillingAddress, string? City, string? State, string? ZipCode, string? Country, string? Notes)
    {
        this.Id = Id;
        this.CardHolderName = CardHolderName;
        this.CardNumber = CardNumber;
        this.ExpiryMonth = ExpiryMonth;
        this.ExpiryYear = ExpiryYear;
        this.CVV = CVV;
        this.Pin = Pin;
        this.CardIssuer = CardIssuer;
        this.CardType = CardType;
        this.BillingAddress = BillingAddress;
        this.City = City;
        this.State = State;
        this.ZipCode = ZipCode;
        this.Country = Country;
        this.Notes = Notes;
    }


}
