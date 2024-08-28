
namespace Cyber_Vault.BL;

class Folder
{
    public int? Id
    {

        get; set;
    }

    public string? Name
    {

        get; set;
    }

    public int? ParentId
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

    public Folder()
    {

    }
}
