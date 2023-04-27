using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Financier.Converters
{
    public class MccConverter : IValueConverter
    {
        static MccConverter()
        {
            var travel = new List<int> {4011, 4111, 4112, 4131, 4304, 4411, 4415, 4418, 4457, 4468, 4511, 4582, 4722, 4784, 4789, 5962, 7011, 7032, 7033, 7512, 7513, 7519 };
            travel.AddRange(Range(3000, 3299));
            travel.AddRange(Range(3351, 3441));
            travel.AddRange(Range(3501, 3999));

            var sport = new List<int> { 5733, 5735,5941, 7221, 7333, 7395, 7929, 7932, 7933, 7941, 8664 };
            sport.AddRange(Range(5815, 5818));
            sport.AddRange(Range(5945, 5947));
            sport.AddRange(Range(5970, 5973));
            sport.AddRange(Range(7911, 7922));
            sport.AddRange(Range(7991, 7994));
            sport.AddRange(Range(7996, 7999));

            var tmp = new Dictionary<string, int[]>
            {
               { "Медицина", new [] { 4119, 5047, 5122, 5292, 5295, 5912, 5975, 5976, 7297, 7298, 8011, 8021, 8031, 8041, 8042, 8043, 8049, 8050, 8062, 8071, 8099 } },
               { "Продукти", new [] { 5441, 5921, 5411, 5412, 5451, 5422, 5297, 5298, 5331, 5715, 5300, 5462, 5399, 5499, 5311 } },
               { "Краса", new [] { 5977, 7230 } },
               { "Авто та АЗС", new [] { 5172, 5511, 5531, 5532, 5533, 5541, 5542, 5983, 7511, 7523, 7531, 7534, 7535, 7538, 7542, 7549 } },
               { "Одяг та взуття", new [] { 5131, 5137, 5139, 5611, 5621, 5631, 5641, 5651, 5655, 5661, 5681, 5691, 5697, 5698, 5699, 5931, 5948, 5949, 7251, 7296 } },
               { "Мандри", travel.ToArray() },
               { "Розваги та спорт", sport.ToArray() },
               { "Кафе та ресторани", new [] { 5811, 5812,5813} },
               { "Кіно", new [] { 7829, 7832, 7841 } },
               { "Таксі", new [] { 4121 } },
               { "Тварини", new [] { 0742, 5995 } },
               { "Книги", new [] { 2741, 5111, 5192, 5942, 5994 } },
               { "Квіти", new [] { 5992, 5193 } },
               { "Фастфуд", new [] { 5814 } },
               { "Переказ", new [] { 4829 } },
               { "Комунальні", new [] { 4900 } },
               { "Побутова техніка", new [] { 5732 } },
               { "Доставка", new [] { 7399 } },
            };

            MCC = tmp.SelectMany(x => x.Value.Select( y => new KeyValuePair<int, string>(y, x.Key))).ToDictionary(x => x.Key, y => y.Value);
        }

        private static readonly Dictionary<int, string> MCC;


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int mcc = (int)value;
            if (MCC.ContainsKey(mcc))
            {
                  return MCC[mcc];
            }
            return mcc.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<int> Range(int from, int to)
        {
            return Enumerable.Range(from, to - from + 1);
        }
    }
}
