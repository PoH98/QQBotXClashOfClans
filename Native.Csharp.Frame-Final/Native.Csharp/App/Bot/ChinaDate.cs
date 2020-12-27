using System;
using System.Collections.Generic;
using System.Globalization;

namespace Native.Csharp.App.Bot
{
    public static class ChinaDate
    {
        private static ChineseLunisolarCalendar china = new ChineseLunisolarCalendar();
        private static Dictionary<string, GHoliday> gHoliday = new Dictionary<string, GHoliday>();
        private static Dictionary<string, NHoliday> nHoliday = new Dictionary<string, NHoliday>();
        private static string[] JQ = { "小寒", "大寒", "立春", "雨水", "惊蛰", "春分", "清明", "谷雨", "立夏", "小满", "芒种", "夏至", "小暑", "大暑", "立秋", "处暑", "白露", "秋分", "寒露", "霜降", "立冬", "小雪", "大雪", "冬至" };
        private static int[] JQData = { 0, 21208, 43467, 63836, 85337, 107014, 128867, 150921, 173149, 195551, 218072, 240693, 263343, 285989, 308563, 331033, 353350, 375494, 397447, 419210, 440795, 462224, 483532, 504758 };

        static ChinaDate()
        {
            //公历节日
            gHoliday.Add("0101", GHoliday.元旦);
            gHoliday.Add("0214", GHoliday.情人节);
            gHoliday.Add("0401", GHoliday.愚人节);
            gHoliday.Add("0501", GHoliday.劳动节);
            gHoliday.Add("0504", GHoliday.青年节);
            gHoliday.Add("0601", GHoliday.儿童节);
            gHoliday.Add("0910", GHoliday.教师节);
            gHoliday.Add("1224", GHoliday.平安夜);
            gHoliday.Add("1225", GHoliday.圣诞节);

            //农历节日
            nHoliday.Add("0101", NHoliday.春节);
            nHoliday.Add("0115", NHoliday.元宵节);
            nHoliday.Add("0505", NHoliday.端午节);
            nHoliday.Add("0815", NHoliday.中秋节);
            nHoliday.Add("0909", NHoliday.重阳节);
            nHoliday.Add("1208", NHoliday.腊八节);
        }

        public enum GHoliday
        {
            无,
            元旦,
            情人节,
            愚人节,
            劳动节,
            青年节,
            儿童节,
            教师节,
            平安夜,
            圣诞节
        }

        public enum NHoliday
        {
            无,
            除夕,
            春节,
            元宵节,
            端午节,
            中秋节,
            重阳节,
            腊八节
        }
        

        /// <summary>
        /// 获取农历年份
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetYear(DateTime dt)
        {
            int yearIndex = china.GetSexagenaryYear(dt);
            string yearTG = " 甲乙丙丁戊己庚辛壬癸";
            string yearDZ = " 子丑寅卯辰巳午未申酉戌亥";
            string yearSX = " 鼠牛虎兔龙蛇马羊猴鸡狗猪";
            int year = china.GetYear(dt);
            int yTG = china.GetCelestialStem(yearIndex);
            int yDZ = china.GetTerrestrialBranch(yearIndex);

            string str = string.Format("[{1}]{2}{3}{0}", year, yearSX[yDZ], yearTG[yTG], yearDZ[yDZ]);
            return str;
        }

        /// <summary>
        /// 获取农历月份
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetMonth(DateTime dt)
        {
            int year = china.GetYear(dt);
            int iMonth = china.GetMonth(dt);
            int leapMonth = china.GetLeapMonth(year);
            bool isLeapMonth = iMonth == leapMonth;
            if (leapMonth != 0 && iMonth >= leapMonth)
            {
                iMonth--;
            }

            string szText = "正二三四五六七八九十";
            string strMonth = isLeapMonth ? "闰" : "";
            if (iMonth <= 10)
            {
                strMonth += szText.Substring(iMonth - 1, 1);
            }
            else if (iMonth == 11)
            {
                strMonth += "十一";
            }
            else
            {
                strMonth += "腊";
            }
            return strMonth + "月";
        }

        /// <summary>
        /// 获取农历日期
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetDay(DateTime dt)
        {
            int iDay = china.GetDayOfMonth(dt);
            string szText1 = "初十廿三";
            string szText2 = "一二三四五六七八九十";
            string strDay;
            if (iDay == 20)
            {
                strDay = "二十";
            }
            else if (iDay == 30)
            {
                strDay = "三十";
            }
            else
            {
                strDay = szText1.Substring((iDay - 1) / 10, 1);
                strDay = strDay + szText2.Substring((iDay - 1) % 10, 1);
            }
            return strDay;
        }

        /// <summary>
        /// 获取节气
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetSolarTerm(DateTime dt)
        {
            DateTime dtBase = new DateTime(1900, 1, 6, 2, 5, 0);
            DateTime dtNew;
            double num;
            int y;
            string strReturn = "";

            y = dt.Year;
            for (int i = 1; i <= 24; i++)
            {
                num = 525948.76 * (y - 1900) + JQData[i - 1];
                dtNew = dtBase.AddMinutes(num);
                if (dtNew.DayOfYear == dt.DayOfYear)
                {
                    strReturn = JQ[i - 1];
                }
            }

            return strReturn;
        }

        /// <summary>
        /// 获取公历节日
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static GHoliday GetHoliday(DateTime dt)
        {
            if (gHoliday.ContainsKey(dt.Month.ToString("00") + dt.Day.ToString("00")))
                return gHoliday[dt.Month.ToString("00") + dt.Day.ToString("00")];
            else return GHoliday.无;
        }

        /// <summary>
        /// 获取农历节日
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static NHoliday GetChinaHoliday(DateTime dt)
        {
            int year = china.GetYear(dt);
            int iMonth = china.GetMonth(dt);
            int leapMonth = china.GetLeapMonth(year);
            int iDay = china.GetDayOfMonth(dt);
            if (china.GetDayOfYear(dt) == china.GetDaysInYear(year))
            {
                return NHoliday.除夕;
            }
            else if (leapMonth != iMonth)
            {
                if (leapMonth != 0 && iMonth >= leapMonth)
                {
                    iMonth--;
                }
                if (nHoliday.ContainsKey(iMonth.ToString("00") + iDay.ToString("00")))
                    return nHoliday[iMonth.ToString("00") + iDay.ToString("00")];
                else
                    return NHoliday.无;
            }
            return NHoliday.无;
        }
    }
}
