using System;
using System.IO;
using CargoTransportation.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CargoTransportation.Helpers
{
    public static class PDFHelper
    {
        // Базовый шрифт для поддержки кириллицы
        private static BaseFont _baseFont;

        static PDFHelper()
        {
            try
            {
                // Пытаемся использовать системный шрифт Arial
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                if (File.Exists(fontPath))
                {
                    _baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                }
                else
                {
                    // Используем встроенный шрифт с кириллицей
                    _baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, "CP1251", BaseFont.NOT_EMBEDDED);
                }
            }
            catch
            {
                // Если ничего не работает, используем стандартный
                _baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
            }
        }

        private static Font GetFont(float size, int style = Font.NORMAL)
        {
            return new Font(_baseFont, size, style);
        }

        public static void GenerateOrderPDF(Order order, string outputPath)
        {
            using (FileStream fs = new FileStream(outputPath, FileMode.Create))
            {
                Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                // Заголовок
                Paragraph title = new Paragraph("ЗАКАЗ НА ГРУЗОПЕРЕВОЗКУ", GetFont(18, Font.BOLD));
                title.Alignment = Element.ALIGN_CENTER;
                doc.Add(title);
                doc.Add(new Paragraph(" ", GetFont(12)));
                doc.Add(new Paragraph(" ", GetFont(12)));

                // Информация о заказе
                doc.Add(new Paragraph("ИНФОРМАЦИЯ О ЗАКАЗЕ:", GetFont(14, Font.BOLD)));
                doc.Add(new Paragraph($"Номер заказа: {order.OrderNumber}", GetFont(12)));
                doc.Add(new Paragraph($"Дата создания: {order.OrderDate:dd.MM.yyyy HH:mm}", GetFont(12)));
                doc.Add(new Paragraph($"Статус: {order.Status?.StatusName ?? "Новый"}", GetFont(12)));
                doc.Add(new Paragraph(" ", GetFont(12)));

                // Маршрут
                doc.Add(new Paragraph("МАРШРУТ:", GetFont(14, Font.BOLD)));
                doc.Add(new Paragraph($"Пункт погрузки: {order.PickupAddress}", GetFont(12)));
                doc.Add(new Paragraph($"Пункт доставки: {order.DeliveryAddress}", GetFont(12)));
                doc.Add(new Paragraph($"Расстояние: {order.Distance} км", GetFont(12)));
                doc.Add(new Paragraph(" ", GetFont(12)));

                // Финансы
                doc.Add(new Paragraph("ФИНАНСЫ:", GetFont(14, Font.BOLD)));
                doc.Add(new Paragraph($"Стоимость перевозки: {order.Price:C}", GetFont(12)));
                doc.Add(new Paragraph(" ", GetFont(12)));

                // Груз
                doc.Add(new Paragraph("ИНФОРМАЦИЯ О ГРУЗЕ:", GetFont(14, Font.BOLD)));
                doc.Add(new Paragraph($"Наименование: {order.Cargo?.Name ?? "Не указано"}", GetFont(12)));
                doc.Add(new Paragraph($"Вес: {order.Cargo?.Weight ?? 0} кг", GetFont(12)));
                doc.Add(new Paragraph(" ", GetFont(12)));

                // Транспорт
                doc.Add(new Paragraph("ТРАНСПОРТ:", GetFont(14, Font.BOLD)));
                doc.Add(new Paragraph($"Госномер: {order.Vehicle?.PlateNumber ?? "Не назначен"}", GetFont(12)));
                doc.Add(new Paragraph($"Марка: {order.Vehicle?.Brand ?? ""} {order.Vehicle?.Model ?? ""}", GetFont(12)));
                doc.Add(new Paragraph($"Грузоподъемность: {order.Vehicle?.LoadCapacity ?? 0} кг", GetFont(12)));
                doc.Add(new Paragraph(" ", GetFont(12)));

                // Клиент
                doc.Add(new Paragraph("КЛИЕНТ:", GetFont(14, Font.BOLD)));
                doc.Add(new Paragraph($"Компания: {order.Client?.CompanyName ?? "Не указано"}", GetFont(12)));
                doc.Add(new Paragraph($"Контакт: {order.Client?.ContactPerson ?? "Не указано"}", GetFont(12)));
                doc.Add(new Paragraph($"Телефон: {order.Client?.ContactPhone ?? "Не указано"}", GetFont(12)));
                doc.Add(new Paragraph(" ", GetFont(12)));

                // Водитель
                doc.Add(new Paragraph("ВОДИТЕЛЬ:", GetFont(14, Font.BOLD)));
                doc.Add(new Paragraph($"ФИО: {order.Driver?.FullName ?? "Не назначен"}", GetFont(12)));
                doc.Add(new Paragraph($"Телефон: {order.Driver?.Phone ?? "Не указан"}", GetFont(12)));
                doc.Add(new Paragraph(" ", GetFont(12)));

                // Данные для QR-кода
                doc.Add(new Paragraph("ДАННЫЕ ДЛЯ QR-КОДА:", GetFont(14, Font.BOLD)));
                doc.Add(new Paragraph($"ЗАКАЗ: {order.OrderNumber}", GetFont(12)));
                doc.Add(new Paragraph($"КЛИЕНТ: {order.Client?.CompanyName ?? "Не указан"}", GetFont(12)));
                doc.Add(new Paragraph($"ОТКУДА: {order.PickupAddress}", GetFont(12)));
                doc.Add(new Paragraph($"КУДА: {order.DeliveryAddress}", GetFont(12)));
                doc.Add(new Paragraph($"ЦЕНА: {order.Price:C}", GetFont(12)));
                doc.Add(new Paragraph($"РАССТОЯНИЕ: {order.Distance} км", GetFont(12)));
                doc.Add(new Paragraph($"ДАТА: {order.OrderDate:dd.MM.yyyy HH:mm}", GetFont(12)));

                doc.Close();
            }
        }
    }
}