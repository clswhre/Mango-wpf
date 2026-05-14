using System.Text;

namespace OOPWPFProject.Models;

public class HistoricalPlace : Place
{

    public HistoricalPlace(DateOnly? date, int? significance) : base(string.Empty, string.Empty, string.Empty)
    {
        YearBuilt = date;
        Significance = significance;
    }

    public HistoricalPlace(string name, string country, string description, DateOnly? date, int? significance)
        : base(name, country, description)
    {
        YearBuilt = date;
        Significance = significance;
    }

    public DateOnly? YearBuilt
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
            messageBuilder.AppendLine( $"Рік побудови: {YearBuilt.Value.Year}" );
        }
        else
        {

            messageBuilder.AppendLine( "Рік побудови: Невідомо" );
        }

        messageBuilder.AppendLine( $"Значимість: {Significance}/10" );

        return messageBuilder.ToString();
    }
}
