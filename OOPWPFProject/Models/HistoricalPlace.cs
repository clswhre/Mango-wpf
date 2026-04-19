using OOPWPFProject.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OOPWPFProject.Models;

internal class HistoricalPlace : Place
{
    public DateOnly? YearBuilt { get; set; }
    public int Significance { get; set; }

    public override string GetDetails()
    {
        StringBuilder messageBuilder = new(base.GetDetails());
        if ( YearBuilt.HasValue )
        {
            messageBuilder.AppendLine( $"Рік побудови: {YearBuilt.Value.Year}" );
        }
        else
        {
        
            messageBuilder.AppendLine("Рік побудови: Невідомо");
        }

        messageBuilder.AppendLine($"Значимість: {Significance}/10");

        return messageBuilder.ToString();
    }
}