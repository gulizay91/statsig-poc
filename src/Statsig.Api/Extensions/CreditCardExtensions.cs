namespace Statsig.Api.Extensions;

public static class CreditCardExtensions
{
  public static string Mask(this string card)
  {
    var firstFourDigits = card[..4];
    var maskedDigits = new string('*', card.Length - 4);
    return string.Concat(firstFourDigits, maskedDigits);
  }
}