using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using TransactionsLog.Services.TransactionParser.DTOs;

namespace TransactionsLog.Services.TransactionParser
{
    public class CNABTransactionParser : ITransactionParser
    {

        private bool IsValidCpf(string cpf) => !string.IsNullOrWhiteSpace(cpf) && cpf.Length == 11 && cpf.All(c => char.IsDigit(c));

        public (TransactionRawDTO?, string) ParseRecord(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return (null, "Registro vazio");

            if (line.Length < 63) //assume que o nome da loja deve ter no mínimo um caracter
                return (null, "Quantidade de caracteres menor que o mínimo esperado");

            var errors = new List<string>();

            if (!int.TryParse(line.Substring(0, 1), out var transactionTypeId))
                errors.Add("Tipo de transação em formato inesperado");

            if (!DateTime.TryParseExact(line.Substring(1, 8), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var date))
                errors.Add("Data informada inválida");

            if (!decimal.TryParse(line.Substring(9, 10), out var absoluteValue))
                errors.Add("Valor em formato inesperado");

            var cpf = line.Substring(19, 11);
            if (!IsValidCpf(cpf))
                errors.Add("Cpf inválido");

            var card = line.Substring(30, 12);
            if (!DateTime.TryParseExact(line.Substring(42, 6), "HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var time))
                errors.Add("Hora informada inválida");

            var storeOwnerName = line.Substring(48, 14);
            var storeName = line.Substring(62);

            if (errors.Any())
                return (null, string.Join("; ", errors));

            var transactionDto = new TransactionRawDTO(
                transactionTypeId: transactionTypeId,
                absoluteValue: absoluteValue / 100,
                timestamp: new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, DateTimeKind.Utc),
                cpf: cpf,
                card: card,
                storeName: storeName,
                storeOwnerName: storeOwnerName);

            return (transactionDto, string.Empty);
        }
    }
}
