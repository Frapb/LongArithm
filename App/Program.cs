using Parser;
using Interpreter;

public class Program
{
    public static void Main()
    {
        var parser = new Parser.Parser();
        var calculator = new Interpreter.Interpreter();

        Console.WriteLine("Введите необходимое выражение в одну строчку");
        var input_string = Console.ReadLine();

        if (parser.IsCorrect(input_string) == false)
        {

            Console.WriteLine("Проверьте правильность выражения");
        }
        else
        {
            var tokens = parser.MakeTokens(input_string);
            var result = calculator.Calculate(tokens);
            Console.WriteLine(result);
        }
    }
}