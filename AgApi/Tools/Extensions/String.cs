
namespace CustomExtensions
{
    //Extension methods must be defined in a static class
    public static class StringExtension
    {

        //
        // Return null if string was blank
        //
        public static string NoBlank(this string Data)
        {
            string result = null;
            if (Data != "") result = Data;
            return result;
        }

        public static string GetEnclosured(this string Data, string OpenChar, string CloseChar, int Ocurrency = 1)
        {
            string result = "";
            int ocur = 0;
            bool getting = false;
            for (int i= 0; i<Data.Length; i++)
            {
                string letter = Data.Substring(i, 1);

                if (letter == CloseChar)
                {
                    getting = false;
                }

                if (getting) result = result + letter;

                if (letter == OpenChar)
                {
                    ocur++;
                    if (ocur==Ocurrency)
                    {
                        getting = true;
                    }
                }
            }

            return result;

        }

        public static string RemoveEnclosured(this string Data, string OpenChar, string CloseChar)
        {
            if (Data != null && Data.Length > 0)
            {
                string result = Data;

                string final = "";
                bool removing = false;
                for (int i = 0; i < Data.Length; i++)
                {
                    string letter = result.Substring(i, 1);
                    if (letter == OpenChar) removing = true;
                    if (!removing) final = final + letter;
                    if (letter == CloseChar) removing = false;
                }
                return final;
            }
            else { return ""; }
        }

        public static string ExchangeEnclosured(this string Data, string What, string With)
        {
            if (Data != null && Data.Length > 0)
            {
                string result = Data;

                string final = "";
                bool removing = false;
                final = result.Replace(What, With);
                return final;
            }
            else { return ""; }
        }

    }
}
