using System.Text;

namespace OOPWPFProject.Models;

public class HistoricalPlace : Place
{
    public HistoricalPlace()
    {
    }

    public int? YearBuilt
    {
        get; set;
    }
    public int? Significance
    {
        get; set;
    }

    public override string GetDetails()
    {
        StringBuilder messageBuilder = new(base.GetDetails());
        if ( YearBuilt.HasValue )
        {
            messageBuilder.AppendLine( $"Рік побудови: {YearBuilt}" );
        }
        else
        {

            messageBuilder.AppendLine( "Рік побудови: Невідомо" );
        }

        messageBuilder.AppendLine( $"Значимість: {Significance}/10" );

        return messageBuilder.ToString();
    }
}
