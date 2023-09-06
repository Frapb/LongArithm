using System.Text;

namespace LongArifm
{
    public class BigNumber
    {
        private List<byte> digits = new List<byte>();
        private Sign sign = Sign.Plus;

        // =========== конструкторы ===========

        public BigNumber(List<byte> bytes)
        {
            digits = bytes.ToList();
            RemoveNulls();
        }

        public BigNumber(Sign sign, List<byte> bytes)
        {
            this.sign = sign;
            digits = bytes;
            RemoveNulls();
        }

        public BigNumber(string s)
        {
            if (s.StartsWith("-"))
            {
                sign = Sign.Minus;
                s = s.Substring(1);
            }

            foreach (var c in s.Reverse())
            {
                digits.Add(Convert.ToByte(c.ToString()));
            }

            RemoveNulls();
        }

        public BigNumber(int x)
        {
            if (x < 0)
            {
                sign = Sign.Minus;
            }

            digits.AddRange(GetBytes((uint)Math.Abs(x)));
        }

        // =========== ========== ===========

        private List<byte> GetBytes(uint num)
        {
            var bytes = new List<byte>();
            do
            {
                bytes.Add((byte)(num % 10));
                num /= 10;
            } while (num > 0);

            return bytes;
        }

        private void RemoveNulls()
        {
            for (var i = digits.Count - 1; i > 0; i--)
            {
                if (digits[i] == 0)
                {
                    digits.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
        }

        // представление числа в экспоненциальном виде
        public static BigNumber Exp(byte val, int exp)
        {
            var bigInt = Zero;
            bigInt.SetByte(exp, val);
            bigInt.RemoveNulls();
            return bigInt;
        }

        //создание нуля или единицы в любом месте программы
        public static BigNumber Zero => new BigNumber(0);
        public static BigNumber One => new BigNumber(1);

        //длина числа
        public int Size => digits.Count;

        //получение цифры по индексу
        public byte GetByte(int i) => i < Size ? digits[i] : (byte)0;

        //установка цифры по индексу
        public void SetByte(int i, byte b)
        {
            while (digits.Count <= i)
            {
                digits.Add(0);
            }

            digits[i] = b;
        }

        //преобразование длинного числа в строку
        public override string ToString()
        {
            if (this == Zero) return "0";
            var s = new StringBuilder(sign == Sign.Plus ? "" : "-");

            for (int i = digits.Count - 1; i >= 0; i--)
            {
                s.Append(Convert.ToString(digits[i]));
            }

            return s.ToString();
        }

        //  ====== Арифметические действия над большими числами ======

        public static BigNumber Add(BigNumber a, BigNumber b)
        {
            var digits = new List<byte>();
            var maxLength = Math.Max(a.Size, b.Size);
            byte t = 0;
            for (int i = 0; i < maxLength; i++)
            {
                byte sum = (byte)(a.GetByte(i) + b.GetByte(i) + t);
                if (sum > 10)
                {
                    sum -= 10;
                    t = 1;
                }
                else
                {
                    t = 0;
                }

                digits.Add(sum);
            }

            if (t > 0)
            {
                digits.Add(t);
            }

            return new BigNumber(a.sign, digits);
        }

        public static BigNumber Substract(BigNumber a, BigNumber b)
        {
            var digits = new List<byte>();

            BigNumber max = Zero;
            BigNumber min = Zero;

            //сравниваем числа игнорируя знак
            var compare = Comparison(a, b, ignoreSign: true);

            switch (compare)
            {
                case -1:
                    min = a;
                    max = b;
                    break;
                case 0:
                    return Zero;
                case 1:
                    min = b;
                    max = a;
                    break;
            }

            //из большего вычитаем меньшее
            var maxLength = Math.Max(a.Size, b.Size);

            var t = 0;
            for (var i = 0; i < maxLength; i++)
            {
                var s = max.GetByte(i) - min.GetByte(i) - t;
                if (s < 0)
                {
                    s += 10;
                    t = 1;
                }
                else
                {
                    t = 0;
                }

                digits.Add((byte)s);
            }

            return new BigNumber(max.sign, digits);
        }

        public static BigNumber Multiply(BigNumber a, BigNumber b)
        {
            var retValue = Zero;

            for (var i = 0; i < a.Size; i++)
            {
                for (int j = 0, carry = 0; (j < b.Size) || (carry > 0); j++)
                {
                    var cur = retValue.GetByte(i + j) + a.GetByte(i) * b.GetByte(j) + carry;
                    retValue.SetByte(i + j, (byte)(cur % 10));
                    carry = cur / 10;
                }
            }

            retValue.sign = a.sign == b.sign ? Sign.Plus : Sign.Minus;
            return retValue;
        }

        public static BigNumber Div(BigNumber a, BigNumber b)
        {
            var retValue = Zero;
            var curValue = Zero;

            for (var i = a.Size - 1; i >= 0; i--)
            {
                curValue += Exp(a.GetByte(i), i);

                var x = 0;
                var l = 0;
                var r = 10;
                while (l <= r)
                {
                    var m = (l + r) / 2;
                    var cur = b * Exp((byte)m, i);
                    if (cur <= curValue)
                    {
                        x = m;
                        l = m + 1;
                    }
                    else
                    {
                        r = m - 1;
                    }
                }

                retValue.SetByte(i, (byte)(x % 10));
                var t = b * Exp((byte)x, i);
                curValue = curValue - t;
            }

            retValue.RemoveNulls();

            retValue.sign = a.sign == b.sign ? Sign.Plus : Sign.Minus;
            return retValue;
        }

        //  ====== =============================================== ======

        //  ====== Сравнение больших чисел ======

        private static int Comparison(BigNumber a, BigNumber b, bool ignoreSign = false)
        {
            return CompareSign(a, b, ignoreSign);
        }

        private static int CompareSign(BigNumber a, BigNumber b, bool ignoreSign = false)
        {
            if (!ignoreSign)
            {
                if (a.sign < b.sign)
                {
                    return -1;
                }
                else if (a.sign > b.sign)
                {
                    return 1;
                }
            }

            return CompareSize(a, b);
        }

        private static int CompareSize(BigNumber a, BigNumber b)
        {
            if (a.Size < b.Size)
            {
                return -1;
            }
            else if (a.Size > b.Size)
            {
                return 1;
            }

            return CompareDigits(a, b);
        }

        private static int CompareDigits(BigNumber a, BigNumber b)
        {
            var maxLength = Math.Max(a.Size, b.Size);
            for (var i = maxLength; i >= 0; i--)
            {
                if (a.GetByte(i) < b.GetByte(i))
                {
                    return -1;
                }
                else if (a.GetByte(i) > b.GetByte(i))
                {
                    return 1;
                }
            }

            return 0;
        }

        //  ====== ======================== ======

        //  ====== Арифметические операторы ======

        // унарный минус(изменение знака числа)
        public static BigNumber operator -(BigNumber a)
        {
            a.sign = a.sign == Sign.Plus ? Sign.Minus : Sign.Plus;
            return a;
        }

        //сложение
        public static BigNumber operator +(BigNumber a, BigNumber b) => a.sign == b.sign ? Add(a, b) : Substract(a, b);

        //вычитание
        public static BigNumber operator -(BigNumber a, BigNumber b) => a + -b;

        //умножение
        public static BigNumber operator *(BigNumber a, BigNumber b) => Multiply(a, b);

        //целочисленное деление(без остатка)
        public static BigNumber operator /(BigNumber a, BigNumber b) => Div(a, b);

        //  ====== ========================= ======

        //  ====== Операторы сравнения ======

        public static bool operator <(BigNumber a, BigNumber b) => Comparison(a, b) < 0;

        public static bool operator >(BigNumber a, BigNumber b) => Comparison(a, b) > 0;

        public static bool operator <=(BigNumber a, BigNumber b) => Comparison(a, b) <= 0;

        public static bool operator >=(BigNumber a, BigNumber b) => Comparison(a, b) >= 0;

        public static bool operator ==(BigNumber a, BigNumber b) => Comparison(a, b) == 0;

        public static bool operator !=(BigNumber a, BigNumber b) => Comparison(a, b) != 0;

        public override bool Equals(object obj) => !(obj is BigNumber) ? false : this == (BigNumber)obj;

        //  ====== =================== ======
    }
}