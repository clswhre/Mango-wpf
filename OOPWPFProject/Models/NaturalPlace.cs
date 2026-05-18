using System.Text;

namespace OOPWPFProject.Models;

public class NaturalPlace : Place
{

    public DateOnly? YearFormed
    {
        get; set;
    } 
    public bool? ProtectedStatus
    {
        get; set;
    }

    public NaturalPlace()
    {
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
