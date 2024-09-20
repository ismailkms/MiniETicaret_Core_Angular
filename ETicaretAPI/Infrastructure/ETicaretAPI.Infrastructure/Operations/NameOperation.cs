
namespace ETicaretAPI.Infrastructure.Operations
{
    public static class NameOperation
    {
        public static string CharacterRegulatory(string name)
            => name.Replace("\"", "")
                   .Replace("!", "")
                   .Replace("'", "")
                   .Replace("^", "")
                   .Replace("+", "")
                   .Replace("%", "")
                   .Replace("&", "")
                   .Replace("/", "")
                   .Replace("(", "")
                   .Replace(")", "")
                   .Replace("=", "")
                   .Replace("?", "")
                   .Replace("_", "")
                   .Replace(" ", "-")
                   .Replace("@", "")
                   .Replace("€", "")
                   .Replace("₺", "")
                   .Replace("¨", "")
                   .Replace("~", "")
                   .Replace(",", "")
                   .Replace(";", "")
                   .Replace(":", "")
                   .Replace(".", "-")
                   .Replace("Ö", "o")
                   .Replace("ö", "o")
                   .Replace("ı", "i")
                   .Replace("İ", "i")
                   .Replace("ğ", "g")
                   .Replace("Ğ", "g")
                   .Replace("ß", "")
                   .Replace("æ", "")
                   .Replace("â", "a")
                   .Replace("î", "i")
                   .Replace("ş", "s")
                   .Replace("Ş", "s")
                   .Replace("ç", "c")
                   .Replace("Ç", "c")
                   .Replace("<", "")
                   .Replace(">", "")
                   .Replace("|", "");

    }
}
