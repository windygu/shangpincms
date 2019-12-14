using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Service.Common
{
    public class DivideHelper
    {
        public static string Divide(int a, int b, int p)
        {
            if (a <= 0 || b <= 0 || p <= 0)
            {
                return "0";
            }
            System.Text.StringBuilder builder = new StringBuilder(p);
            int count = 0;
            p = p + 2;

            builder.Append(a / b);

            a = a % b;
            if (a != 0)
                builder.Append(".");

            while (a != 0 && count < p)
            {
                a *= 10;

                builder.Append(a / b);
                a = a % b;
                count++;
            }

            return String.Concat(new string[] { (Double.Parse(builder.ToString()) * 100).ToString(), "%" });
        }


        public static Double DivideDecimal(int a, int b, int p)
        {
            if (a <= 0 || b <= 0 || p <= 0)
            {
                return 0;
            }
            System.Text.StringBuilder builder = new StringBuilder(p);
            int count = 0;
            p = p + 2;

            builder.Append(a / b);

            a = a % b;
            if (a != 0)
                builder.Append(".");

            while (a != 0 && count < p)
            {
                a *= 10;

                builder.Append(a / b);
                a = a % b;
                count++;
            }

            return Double.Parse(builder.ToString()) * 100;
        }


        public static Double DivideDecimalValue(int a, int b, int p)
        {
            if (a <= 0 || b <= 0 || p <= 0)
            {
                return 0;
            }
            System.Text.StringBuilder builder = new StringBuilder(p);
            int count = 0;
            p = p + 2;

            builder.Append(a / b);

            a = a % b;
            if (a != 0)
                builder.Append(".");

            while (a != 0 && count < p)
            {
                a *= 10;

                builder.Append(a / b);
                a = a % b;
                count++;
            }

            return Double.Parse(builder.ToString());
        }
    }
}
