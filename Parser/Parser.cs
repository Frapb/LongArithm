namespace Parser
{
    public class Parser
    {
        private char[] dictionary = { '(', ')', '*', '/', '+', '-', '^', ' ' }; // and numbers

        public bool IsCorrect(string s)
        {
            if (s.Length == 0) return false;
            int open_brackets = 0; // счётчик открытых скобок
            int closed_brackets = 0; // счётчик закрытых скобок
            for (int i = 0; i < s.Length; i++)
            {
                if (!(dictionary.Contains(s[i]) || Char.IsDigit(s[i]))) return false; //проверка - содержит ли слвоарь переданный символ или является ли он цифрой 
                if (s[i] == '(') open_brackets++;
                if (s[i] == ')') closed_brackets++;
                if (open_brackets < closed_brackets) return false; // без этого условия будет допустимо ")("
            }
            if (open_brackets != closed_brackets) return false;
            return true;
        }

        public List<string> MakeTokens(string s)
        {
            List<string> tokens = new List<string>();

            bool is_last_el_number = false;
            bool negative_number = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (Char.IsDigit(s[i]))
                {
                    if (is_last_el_number || negative_number) // предыдущий элемент был цифрой
                    {
                        tokens[tokens.Count - 1] += s[i].ToString(); // к последнему элементу списка tokens дописать цифру 
                        negative_number = false;
                    }
                    else tokens.Add(s[i].ToString()); // это одиночая цифра (или к ней потом будут доисаны другие)
                    is_last_el_number = true;
                }
                else
                {
                    if ((!is_last_el_number) && s[i] == '-') // если предыдущий был не числом, то "-" предвещает отрицательное число
                    {
                        tokens.Add(s[i].ToString());
                        negative_number = true;
                    }
                    else if (s[i] != ' ') tokens.Add(s[i].ToString()); // остались только операции
                    is_last_el_number = false;
                }
            }

            return tokens;
        }
    }
}