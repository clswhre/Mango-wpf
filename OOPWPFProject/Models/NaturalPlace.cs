using System.Text;

namespace OOPWPFProject.Models;

public class NaturalPlace(DateOnly? date, bool? protectedStatus) : Place(string.Empty, string.Empty, string.Empty)
{

    public DateOnly? YearFormed
    {
        get; set;
    } = date;
    public bool? ProtectedStatus
    {
        get; set;
    } = protectedStatus;

    public NaturalPlace(string name, string country, string description, DateOnly? date, bool? protectedStatus)
        : this(date, protectedStatus)
    {
        Name = name;
        Country = country;
        Description = description;
    }

    public override string GetDetails()
    {
        StringBuilder messageBuilder = new (base.GetDetails());

        if ( YearFormed.HasValue )
        {
            messageBuilder.AppendLine( $"Рік утворення: {YearFormed.Value.Year}" );
        }
        else
        {
            messageBuilder.AppendLine( "Рік утворення: Невідомо" );
        }

        if ( ProtectedStatus.HasValue && ProtectedStatus == true )
        {
            messageBuilder.AppendLine( $"Об'єкт під захистом" );
        }
        else
        {
            messageBuilder.AppendLine( "Не захищено" );
        }

        return messageBuilder.ToString();
    }
}
