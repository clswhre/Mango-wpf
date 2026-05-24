using System.Text;

namespace OOPWPFProject.Models.Places;

public class NaturalPlace : Place
{
	public int? YearFormed { get; set; }
	public bool? ProtectedStatus { get; set; }

	public NaturalPlace() { }

	public override string GetDetails()
	{
		StringBuilder messageBuilder = new(base.GetDetails());

		if (YearFormed.HasValue)
		{
			messageBuilder.AppendLine($"Рік утворення: {YearFormed}");
		}
		else
		{
			messageBuilder.AppendLine("Рік утворення: Невідомо");
		}

		if (ProtectedStatus.HasValue && ProtectedStatus == true)
		{
			messageBuilder.AppendLine($"Об'єкт під захистом");
		}
		else
		{
			messageBuilder.AppendLine("Не захищено");
		}

		return messageBuilder.ToString();
	}
}
