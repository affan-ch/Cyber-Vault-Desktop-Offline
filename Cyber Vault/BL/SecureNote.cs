namespace Cyber_Vault.BL;

internal class SecureNote
{
    public int? Id
    {
        get; set;
    }

    public string? Title
    {    
        get; set;
    }

    public string? Note
    {    
        get; set;
    }

    public string? Category
    {       
        get; set;
    }

    public string? Tags
    {          
        get; set;
    }

    public string? DateCreated
    {       
        get; set;
    }

    public string? DateModified
    {          
        get; set;
    }


    public SecureNote()
    {
    }

    public SecureNote(int? id, string? title, string? note, string? category, string? tags, string? dateCreated, string? dateModified)
    {    
        Id = id;
        Title = title;
        Note = note;
        Category = category;
        Tags = tags;
        DateCreated = dateCreated;
        DateModified = dateModified;
    }

    public SecureNote(string? title, string? note, string? category, string? tags, string? dateCreated, string? dateModified)
    {       
        Title = title;
        Note = note;
        Category = category;
        Tags = tags;
        DateCreated = dateCreated;
        DateModified = dateModified;
    }

    public SecureNote(string? title, string? note, string? category, string? tags)
    {          
        Title = title;
        Note = note;
        Category = category;
        Tags = tags;
    }

}
