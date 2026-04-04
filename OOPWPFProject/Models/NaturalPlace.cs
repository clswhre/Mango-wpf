using OOPWPFProject.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OOPWPFProject.ViewModels;

internal class NaturalPlace : Place
{
    public DateOnly? YearBuilt { get; set; }
    public bool? ProtectedStatus { get; set; }

    public override string GetDetails()
    {
        StringBuilder messageBuilder = new (base.GetDetails());

        if( YearBuilt.HasValue )
        {
            messageBuilder.AppendLine($"Рік утворення: {YearBuilt.Value.Year}");
        }
        else
        {
            messageBuilder.AppendLine("Рік утворення: Невідомо");
        }

        if ( ProtectedStatus.HasValue )
        {
            messageBuilder.AppendLine( $"Об'єкт під захистом" ) ;
        }
        else
        {
            messageBuilder.AppendLine( "Не захищено" ) ;
        }

        return messageBuilder.ToString();
    }
}
