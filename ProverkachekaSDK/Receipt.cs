using System;
using System.Collections.Generic;

namespace ProverkachekaSDK
{
    public class Receipt
    {

        /// <summary>
        /// Огранизация
        /// </summary>
        public string User { get; set; } = "";

        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; } = "";

        /// <summary>
        /// Тип операции:
        /// 1 - Приход
        /// 2 - Возврат прихода
        /// 3 - Расход
        /// 4 - Возврат расхода
        /// </summary>
        public byte OperationType { get; set; }

        /// <summary>
        /// НДС со ставкой 20% (сумма в коп.)
        /// </summary>
        public int Nds20 { get; set; }

        /// <summary>
        /// НДС со ставкой 10% (сумма в коп.)
        /// </summary>
        public int Nds10 { get; set; }

        /// <summary>
        /// НДС не облагается (сумма в коп.)
        /// </summary>
        public int NdsNo { get; set; }

        /// <summary>
        /// Итого (сумма в коп.)
        /// </summary>
        public int TotalSum { get; set; }

        /// <summary>
        /// Наличные (сумма в коп.)
        /// </summary>
        public int CashTotalSum { get; set; }

        /// <summary>
        /// Картой (сумма в коп.)
        /// </summary>
        public int EcashTotalSum { get; set; }

        /// <summary>
        /// ВИД НАЛОГООБЛОЖЕНИЯ:
        /// 1 - ОСН,
        /// 2 - УСН,
        /// 4 - УСН доход – расход,
        /// 8 - ЕНВД,
        /// 16 - ЕСХН,
        /// 32 - ПСН
        /// </summary>
        public byte TaxationType { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        public byte Region { get; set; }

        /// <summary>
        /// Время
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Место чека
        /// </summary>
        public string RetailPlace { get; set; } = "";


        /// <summary>
        /// Товары
        /// </summary>
        public List<Product> Goods { get; set; } = new List<Product>();

        /// <summary>
        /// Сообщение, если не удалась добавить чек
        /// </summary>
        public string Message { get; } = "";

        public Receipt() {}
        public Receipt(string message) => Message = message;
    }
}
