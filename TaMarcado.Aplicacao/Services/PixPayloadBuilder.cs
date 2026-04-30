using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TaMarcado.Aplicacao.Services;

public static class PixPayloadBuilder
{
    public static string Build(string keyPix, string merchantName, decimal amount, string txId)
    {
        var cleanName = Sanitize(merchantName, 25);
        var cleanTxId = SanitizeTxId(txId, 25);
        const string city = "BRASIL";

        var merchantAccountInfo =
            Field("00", "BR.GOV.BCB.PIX") +
            Field("01", keyPix);

        var additionalData = Field("05", cleanTxId);

        var payload =
            "000201" +
            "010211" +
            Field("26", merchantAccountInfo) +
            "52040000" +
            "5303986" +
            Field("54", amount.ToString("F2", CultureInfo.InvariantCulture)) +
            "5802BR" +
            Field("59", cleanName) +
            Field("60", city) +
            Field("62", additionalData) +
            "6304";

        return payload + Crc16(payload);
    }

    private static string Field(string id, string value) =>
        id + value.Length.ToString("D2") + value;

    private static string Sanitize(string value, int max)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);

        var clean = Regex.Replace(sb.ToString().Normalize(NormalizationForm.FormC), @"[^a-zA-Z0-9 ]", "").Trim();
        return clean.Length > max ? clean[..max] : clean;
    }

    private static string SanitizeTxId(string value, int max)
    {
        var clean = Regex.Replace(value.Replace("-", ""), @"[^a-zA-Z0-9]", "");
        return clean.Length > max ? clean[..max] : clean;
    }

    private static string Crc16(string payload)
    {
        ushort crc = 0xFFFF;
        foreach (var b in Encoding.UTF8.GetBytes(payload))
        {
            crc ^= (ushort)(b << 8);
            for (var i = 0; i < 8; i++)
                crc = (crc & 0x8000) != 0 ? (ushort)((crc << 1) ^ 0x1021) : (ushort)(crc << 1);
        }
        return crc.ToString("X4");
    }
}
