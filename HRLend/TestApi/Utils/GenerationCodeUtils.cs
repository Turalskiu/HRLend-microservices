using System.Text;

namespace TestApi.Utils
{
    public class GenerationCodeUtils
    {

        public static string Generation(int length)
        {
            Random rand = new Random();

            StringBuilder code = new StringBuilder();

            for(int i = 0; i < length; i++)
            {
                int c = rand.Next(0, 10);
                code.Append(c);
            }

            return code.ToString();
        }
    }
}
