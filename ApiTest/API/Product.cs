namespace API
{
    public class Product
    {
        public string id { get; set; }
        //Категория: мужские, женские, casio...
        public string category_id { get; set; }
        //Отоббражаемое имя в карточке
        public string title { get; set; }
        //Ссылка (задаётся сервером)
        public string alias { get; set; }
        //Не нашёл куда выводится
        public string content { get; set; }
        //Основная цена
        public string price { get; set; }
        //Старая цена, выводится зачёркнутым если не 0
        public string old_price { get; set; }
        //Я так и не понял за что это отвечает
        public string status { get; set; }
        //Я так и не понял за что это отвечает
        public string keywords { get; set; }
        //Я так и не понял за что это отвечает
        public string description { get; set; }
        //Ссылка на картинку (задаётся сервером если не передано)
        public string img { get; set; }
        //Я так и не понял за что это отвечает
        public string hit { get; set; }
        //Тип часов
        public string cat { get; set; }
    }
}