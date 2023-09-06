using LongArifm;

namespace Interpreter
{
    public class Interpreter
    {
        private Stack<BigNumber> numbers = new Stack<BigNumber>();
        private Stack<string> operations = new Stack<string>();
        private Dictionary<string, int> priority_operation = new Dictionary<string, int>()
        {
            ["+"] = 1,
            ["-"] = 1,
            ["*"] = 2,
            ["/"] = 2,
            ["^"] = 3,
        };

        public string Calculate(List<string> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (Is_Number(tokens[i])) // если токен - число, кладём его на стек, преобразовав в BigNumber
                    numbers.Push(new BigNumber(tokens[i]));
                // теперь могут быть только скобки или операторы
                else if (operations.Count == 0) operations.Push(tokens[i]); // если до этого ни разу не встретилась скобка или оператор, добавляем в стек
                else if (tokens[i] == ")")
                {
                    while (operations.Peek() != "(")
                        numbers.Push(Make_Operation(numbers.Pop(), numbers.Pop(), operations.Pop()));               // если встретили закрывающую, то нужно подсчитать всё на стеках вплоть до открывающей
                    operations.Pop(); // убираем "("
                }
                else if (operations.Peek() == ")" || operations.Peek() == "(") operations.Push(tokens[i]); // кладем в операт. тот знак который встреитили
                else if (tokens[i] == "(") operations.Push(tokens[i]);
                // со скобками разобрались, их быть больше не может, считает автор. остались операции
                else if (priority_operation[operations.Peek()] < priority_operation[tokens[i]]) operations.Push(tokens[i]); // если приоритет последней операции на стеке меньше приоритета считываемой операции, положить считываемую на стек
                else
                {
                    numbers.Push(Make_Operation(numbers.Pop(), numbers.Pop(), operations.Pop())); // 5*4 + 3
                    i--;
                }
            }
            while (operations.Count != 0) // токены-то закончились, но операции на стеке ещё могут быть
                numbers.Push(Make_Operation(numbers.Pop(), numbers.Pop(), operations.Pop()));

            return numbers.Peek().ToString();
        }

        private static bool Is_Number(string s)
        {
            if (Char.IsDigit(s[s.Length - 1]))
                return true;
            else
                return false;
        }

        private static BigNumber Make_Operation(BigNumber number2, BigNumber number1, string operation) //второе число это то к которому мы выполняем операцию
        {
            switch (operation)
            {
                case "+": return number1 + number2;
                case "-": return number1 - number2;
                case "*": return number1 * number2;
                case "/": return number1 / number2;
                default: return new BigNumber(0);
            }
        }
    }
}