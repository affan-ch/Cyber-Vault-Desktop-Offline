
namespace Cyber_Vault.BL;
internal class Document
{
    /* Id, Type, Title, Document, */
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

    public byte[]? Document1
    {
        get; set; 
    }
    public byte[]? Document2
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

    public Document(int? Id, string? Type, string? Title, byte[]? Document1, byte[]? Document2, string? DateAdded, string? DateModified)
    {
        this.Id = Id;
        this.Type = Type;
        this.Title = Title;
        this.Document1 = Document1;
        this.Document2 = Document2;
        this.DateAdded = DateAdded;
        this.DateModified = DateModified;
    }


}
