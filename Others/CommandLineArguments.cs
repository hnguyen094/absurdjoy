// https://www.notion.so/Command-Line-Arguments-675590fa411a42e7b8e0c3924c85a1df
namespace absurdjoy {
    /// <summary>
    /// A helper class that lets you pull keys/values from the command line.
    /// Arguments are distinguished by spaces, with double quotes being used to encapsulate spaces. See docs
    /// link below for examples.
    /// 
    /// Note that the first key is equal to the executable name. 
    /// https://docs.microsoft.com/en-us/dotnet/api/system.environment.getcommandlineargs?view=netframework-4.8
    /// </summary>
    public class CommandLineArguments {
        /// <summary>
        /// Check if a key exists in the arguments at all.
        /// </summary>
        /// <param name="key">The key you want to look up</param>
        /// <param name="splitter">The character that seperates the key from the value</param>
        /// <returns>true if the key exists on the command line.</returns>
        public static bool HasKey(string key, char splitter = '=') {
            var split = GetKeyValuePair(key, splitter);
            if (split != null) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the value of a particular key from the command line argument.
        /// </summary>
        /// <param name="key">The key you want to look up</param>
        /// <param name="splitter">The character that seperates the key from the value</param>
        /// <returns>the string literal associated with the key</returns>
        public static string GetValue(string key, char splitter = '=') {
            var split = GetKeyValuePair(key, splitter);
            if (split != null && split.Length > 1) {
                return split[1];
            }

            return null;
        }

        /// <summary>
        /// Returns the raw key/value pair for a given key.
        /// </summary>
        /// <param name="key">The key you want to look up</param>
        /// <param name="splitter">The character that seperates the key from the value</param>
        /// <returns>array containing the key as the first element, and the value as the second element (if any).</returns>
        public static string[] GetKeyValuePair(string key, char splitter = '=') {
            var args = System.Environment.GetCommandLineArgs();

            /*
			// Uncomment this if you want to test arbitrary arguments in a dev environment.
			args = new[]
			{
				"test.exe",
				"myArgument=myValue"
			};
			*/

            for (int i = 0; i < args.Length; i++) {
                if (string.IsNullOrEmpty(args[i])) {
                    continue;
                }

                // Limiting the split to 2 elements requires this array
                char[] splitArray = new[] { splitter };
                // Split into a maximum of two elements
                var split = args[i].Split(splitArray, 2);

                if (split == null || split.Length == 0) {
                    continue;
                }
                if (split[0].ToLowerInvariant() == key.ToLowerInvariant()) {
                    return split;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the name of the executable that launched this application.
        /// </summary>
        public static string GetExecutableName() {
            return System.Environment.GetCommandLineArgs()[0];
        }
    }
}