using System.Data.SQLite;
using System.Runtime.InteropServices;
using Cyber_Vault.BL;
using Cyber_Vault.DB;
using Cyber_Vault.Utils;

namespace Cyber_Vault.DL;

internal class CreditCardDL
{
    private static readonly List<CreditCard> creditCards = new();

    // Add Credit Card
    public static void AddCreditCard(CreditCard creditCard)
    {
        creditCards.Add(creditCard);
    }

    // Update Credit Card
    public static void UpdateCreditCard(CreditCard creditCard)
    {
        var index = creditCards.FindIndex(c => c.Id == creditCard.Id);
        creditCards[index] = creditCard;
    }

    // Delete Credit Card
    public static void DeleteCreditCard(int id)
    {  
        var creditCard = creditCards.FirstOrDefault(c => c.Id == id);
        if (creditCard != null)
        {
            creditCards.Remove(creditCard);
        }
    }

    // Get All Credit Cards
    public static List<CreditCard> GetCreditCards()
    {       
        return creditCards;
    }

    // Get Credit Card by Id
    public static CreditCard? GetCreditCardById(int id)
    {
        return creditCards.FirstOrDefault(c => c.Id == id);
    }

    // Clear the Credit Card List
    public static void ClearCreditCards()
    {
        creditCards.Clear();
    }

    // Get All Credit Cards by Card Type
    public static List<CreditCard> GetCreditCardsByCardType(string cardType)
    {
        return creditCards.Where(c => c.CardType == cardType).ToList();
    }

    // Load Credit Cards Into List from SQLite Database
    public static void LoadCreditCardsFromDatabase()
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection)
        {
            CommandText = "SELECT * FROM CreditCard"
        };

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var UsernamePtr = IntPtr.Zero;
            var PasswordPtr = IntPtr.Zero;
            try
            {
                UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
                PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
                var username = Marshal.PtrToStringUni(UsernamePtr);
                var password = Marshal.PtrToStringUni(PasswordPtr);

                var creditCard = new CreditCard
                (
                    Id: int.Parse(reader["Id"].ToString() ?? "0"),
                    CardHolderName: EncryptionHelper.Decrypt(reader["CardHolderName"].ToString() ?? "", username + password),
                    CardNumber: EncryptionHelper.Decrypt(reader["CardNumber"].ToString() ?? "", username + password),
                    ExpiryMonth: EncryptionHelper.Decrypt(reader["ExpiryMonth"].ToString() ?? "", username + password),
                    ExpiryYear: EncryptionHelper.Decrypt(reader["ExpiryYear"].ToString() ?? "", username + password),
                    CVV: EncryptionHelper.Decrypt(reader["CVV"].ToString() ?? "", username + password),
                    Pin: EncryptionHelper.Decrypt(reader["Pin"].ToString() ?? "", username + password),
                    CardIssuer: EncryptionHelper.Decrypt(reader["CardIssuer"].ToString() ?? "", username + password),
                    CardType: EncryptionHelper.Decrypt(reader["CardType"].ToString() ?? "", username + password),
                    BillingAddress: EncryptionHelper.Decrypt(reader["BillingAddress"].ToString() ?? "", username + password),
                    City: EncryptionHelper.Decrypt(reader["City"].ToString() ?? "", username + password),
                    State: EncryptionHelper.Decrypt(reader["State"].ToString() ?? "", username + password),
                    ZipCode: EncryptionHelper.Decrypt(reader["ZipCode"].ToString() ?? "", username + password),
                    Country: EncryptionHelper.Decrypt(reader["Country"].ToString() ?? "", username + password),
                    Notes: EncryptionHelper.Decrypt(reader["Notes"].ToString() ?? "", username + password),
                    DateAdded: reader["DateAdded"].ToString() ?? "",
                    DateModified: reader["DateModified"].ToString() ?? ""
                );
                creditCards.Add(creditCard);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(UsernamePtr);
                Marshal.ZeroFreeGlobalAllocUnicode(PasswordPtr);
            }
        }

        connection.Close();
    }



    
}
