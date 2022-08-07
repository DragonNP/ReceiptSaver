namespace ProverkachekaSDK
{
    public class Product
    {
        /// <summary>
        /// Сумма позиции (в коп.)
        /// </summary>
        public int Sum { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Цена за 1 шт. (в коп.)
        /// </summary>
        public int Price { get; set; }


        /// <summary>
        /// Количество
        /// </summary>
        public int Quantity { get; set; }
    }
}
