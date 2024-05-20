
namespace Cyber_Vault.BL;
internal class DocumentFile
{
    public int? Id
    {
        get; set;
    }

    public int? DocumentId
    {
        get; set;
    }

    public string? FileName
    {
        get; set;
    }

    public string? FileType
    {
        get; set;
    }

    public byte[]? FileContent
    {
        get; set;
    }

    public DocumentFile()
    {

    }

}
