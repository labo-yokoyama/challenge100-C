using System;

namespace UnitConverter
{
    /// <summary>
    /// 単位変換ロジックを提供します。
    /// </summary>
    public class UnitConverterModel
    {
        /// <summary>
        /// 指定カテゴリの単位一覧を取得します。
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <returns>単位配列</returns>
        public string[] GetUnits(string category)
        {
            if (category == "長さ")
                return new string[] { "m", "cm", "インチ", "フィート" };
            if (category == "重さ")
                return new string[] { "kg", "g", "ポンド", "オンス" };
            if (category == "温度")
                return new string[] { "℃", "?" };
            return new string[0];
        }
        /// <summary>
        /// 単位変換を実行します。
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <param name="fromUnit">変換元単位</param>
        /// <param name="toUnit">変換先単位</param>
        /// <param name="value">変換元値</param>
        /// <returns>変換後の値</returns>
        public double Convert(string category, string fromUnit, string toUnit, double value)
        {
            if (category == "長さ")
                return ConvertLength(fromUnit, toUnit, value);
            if (category == "重さ")
                return ConvertWeight(fromUnit, toUnit, value);
            if (category == "温度")
                return ConvertTemperature(fromUnit, toUnit, value);
            return value;
        }
        /// <summary>
        /// 長さの単位変換。
        /// </summary>
        private double ConvertLength(string from, string to, double v)
        {
            // まずmに統一
            double m = v;
            if (from == "cm") m = v / 100.0;
            else if (from == "インチ") m = v * 0.0254;
            else if (from == "フィート") m = v * 0.3048;
            // mからtoへ
            if (to == "m") return m;
            if (to == "cm") return m * 100.0;
            if (to == "インチ") return m / 0.0254;
            if (to == "フィート") return m / 0.3048;
            return v;
        }
        /// <summary>
        /// 重さの単位変換。
        /// </summary>
        private double ConvertWeight(string from, string to, double v)
        {
            // まずkgに統一
            double kg = v;
            if (from == "g") kg = v / 1000.0;
            else if (from == "ポンド") kg = v * 0.45359237;
            else if (from == "オンス") kg = v * 0.0283495231;
            // kgからtoへ
            if (to == "kg") return kg;
            if (to == "g") return kg * 1000.0;
            if (to == "ポンド") return kg / 0.45359237;
            if (to == "オンス") return kg / 0.0283495231;
            return v;
        }
        /// <summary>
        /// 温度の単位変換。
        /// </summary>
        private double ConvertTemperature(string from, string to, double v)
        {
            if (from == to) return v;
            if (from == "℃" && to == "?") return v * 9.0 / 5.0 + 32.0;
            if (from == "?" && to == "℃") return (v - 32.0) * 5.0 / 9.0;
            return v;
        }
    }
}
