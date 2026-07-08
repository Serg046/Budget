using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Budget.Client.Models;

public class TransactionDocument
{
    [BsonId]
    [JsonIgnore]
    public ObjectId Id { get; set; }

    [BsonElement("transactionId")]
    public string? TransactionId { get; set; }

    [BsonElement("entryReference")]
    public string? EntryReference { get; set; }

    [BsonElement("merchantCategoryCode")]
    public string? MerchantCategoryCode { get; set; }

    [BsonElement("transactionAmount")]
    public Amount TransactionAmount { get; set; } = null!;

    [BsonElement("creditDebitIndicator")]
    public string CreditDebitIndicator { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;

    [BsonElement("bookingDate")]
    public DateOnly? BookingDate { get; set; }

    [BsonElement("valueDate")]
    public DateOnly? ValueDate { get; set; }

    [BsonElement("transactionDate")]
    public DateOnly? TransactionDate { get; set; }

    [BsonElement("balanceAfterTransaction")]
    public Amount? BalanceAfterTransaction { get; set; }

    [BsonElement("referenceNumber")]
    public string? ReferenceNumber { get; set; }

    [BsonElement("referenceNumberSchema")]
    public string? ReferenceNumberSchema { get; set; }

    [BsonElement("remittanceInformation")]
    public List<string> RemittanceInformation { get; set; } = [];

    [BsonElement("creditor")]
    public PartyIdentification? Creditor { get; set; }

    [BsonElement("creditorAccount")]
    public GenericIdentification? CreditorAccount { get; set; }

    [BsonElement("creditorAccountAdditionalIdentification")]
    public GenericIdentification? CreditorAccountAdditionalIdentification { get; set; }

    [BsonElement("creditorAgent")]
    public FinancialInstitutionIdentification? CreditorAgent { get; set; }

    [BsonElement("debtor")]
    public PartyIdentification? Debtor { get; set; }

    [BsonElement("debtorAccount")]
    public GenericIdentification? DebtorAccount { get; set; }

    [BsonElement("debtorAccountAdditionalIdentification")]
    public GenericIdentification? DebtorAccountAdditionalIdentification { get; set; }

    [BsonElement("debtorAgent")]
    public FinancialInstitutionIdentification? DebtorAgent { get; set; }

    [BsonElement("bankTransactionCode")]
    public BankTransactionCode? BankTransactionCode { get; set; }

    [BsonElement("exchangeRate")]
    public ExchangeRate? ExchangeRate { get; set; }

    [BsonElement("note")]
    public string? Note { get; set; }
}

public class Amount
{
    [BsonElement("currency")]
    public string Currency { get; set; } = string.Empty;

    [BsonElement("amount")]
    [JsonPropertyName("amount")]
    public string Value { get; set; } = string.Empty;
}

public class PartyIdentification
{
    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("postalAddress")]
    public PostalAddress? PostalAddress { get; set; }

    [BsonElement("identification")]
    public GenericIdentification? Identification { get; set; }

    [BsonElement("countryOfResidence")]
    public string? CountryOfResidence { get; set; }
}

public class PostalAddress
{
    [BsonElement("addressType")]
    public string? AddressType { get; set; }

    [BsonElement("department")]
    public string? Department { get; set; }

    [BsonElement("subDepartment")]
    public string? SubDepartment { get; set; }

    [BsonElement("streetName")]
    public string? StreetName { get; set; }

    [BsonElement("buildingNumber")]
    public string? BuildingNumber { get; set; }

    [BsonElement("postCode")]
    public string? PostCode { get; set; }

    [BsonElement("townName")]
    public string? TownName { get; set; }

    [BsonElement("countrySubDivision")]
    public string? CountrySubDivision { get; set; }

    [BsonElement("country")]
    public string? Country { get; set; }

    [BsonElement("addressLine")]
    public List<string> AddressLine { get; set; } = [];
}

public class GenericIdentification
{
    [BsonElement("identification")]
    public string Identification { get; set; } = string.Empty;

    [BsonElement("schemeName")]
    public string? SchemeName { get; set; }
}

public class FinancialInstitutionIdentification
{
    [BsonElement("bicFi")]
    public string? BicFi { get; set; }

    [BsonElement("clearingSystemMemberId")]
    public ClearingSystemMemberIdentification? ClearingSystemMemberId { get; set; }

    [BsonElement("name")]
    public string? Name { get; set; }
}

public class ClearingSystemMemberIdentification
{
    [BsonElement("clearingSystemId")]
    public string? ClearingSystemId { get; set; }

    [BsonElement("memberId")]
    public string? MemberId { get; set; }
}

public class BankTransactionCode
{
    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("code")]
    public string? Code { get; set; }

    [BsonElement("subCode")]
    public string? SubCode { get; set; }
}

public class ExchangeRate
{
    [BsonElement("unitCurrency")]
    public string? UnitCurrency { get; set; }

    [BsonElement("rate")]
    [JsonPropertyName("exchange_rate")]
    public string? Rate { get; set; }

    [BsonElement("rateType")]
    public string? RateType { get; set; }

    [BsonElement("contractIdentification")]
    public string? ContractIdentification { get; set; }

    [BsonElement("instructedAmount")]
    public Amount? InstructedAmount { get; set; }
}
