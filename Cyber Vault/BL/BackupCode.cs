
namespace Cyber_Vault.BL;

internal class BackupCode
{
    public int? Id
    {
        get; set;
    }

    public int? AccountId
    {
        get; set;
    }

    public string? Code
    {
        get; set;
    }

    public int? IsUsed
    {
        get; set;
    }

    public string? DateAdded
    {
        get; set;
    }

    public string? DateUsed
    {
        get; set;
    }

    public string? DateModified
    {
        get; set;
    }

    public BackupCode(int? Id, int? AccountId, string? Code, int? IsUsed, string? DateAdded, string? DateUsed, string? DateModified)
    {
        this.Id = Id;
        this.AccountId = AccountId;
        this.Code = Code;
        this.IsUsed = IsUsed;
        this.DateAdded = DateAdded;
        this.DateUsed = DateUsed;
        this.DateModified = DateModified;
    }

    public BackupCode(int? Id, int? AccountId, string? Code, int? IsUsed, string? DateAdded, string? DateModified)
    {
        this.Id = Id;
        this.AccountId = AccountId;
        this.Code = Code;
        this.IsUsed = IsUsed;
        this.DateAdded = DateAdded;
        this.DateModified = DateModified;
    }

    public BackupCode(int? AccountId, string? Code)
    {    
        this.AccountId = AccountId;
        this.Code = Code;
    }
}
