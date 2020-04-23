/* Name: Ternary Tree Data Structure (Assignment #2)
 * Author: Brian Patrick & Brianna Drew
 * Date Created: February 10th, 2020
 * Last Modified: March 8th, 2020
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TernaryTree
{
    public static class MyGlobals
    {
        public static bool exit_bool, error_bool;
    }

    public interface IContainer<T>
    {
        void MakeEmpty();
        bool Empty();
        int Size();
    }

    //-------------------------------------------------------------------------

    public interface ITrie<T> : IContainer<T>
    {
        bool Insert(string key, T value);
        T Value(string key);
    }

    //-------------------------------------------------------------------------

    class Trie<T> : ITrie<T>
    {
        private Node root;                 // Root node of the Trie
        private int size;                  // Number of values in the Trie

        class Node
        {
            public char ch;                // Character of the key
            public T value;                // Value at Node; otherwise default
            public Node low, middle, high; // Left, middle, and right subtrees

            // Node
            // Creates an empty Node
            // All children are set to null
            // Time complexity:  O(1)

            public Node(char ch)
            {
                this.ch = ch;
                value = default(T);
                low = middle = high = null;
            }
        }

        // Trie
        // Creates an empty Trie
        // Time complexity:  O(1)

        public Trie()
        {
            MakeEmpty();
            size = 0;
        }

        // Public Insert
        // Calls the private Insert which carries out the actual insertion
        // Returns true if successful; false otherwise

        public bool Insert(string key, T value)
        {
            return Insert(ref root, key, 0, value);
        }

        // Private Insert
        // Inserts the key/value pair into the Trie
        // Returns true if the insertion was successful; false otherwise
        // Note: Duplicate keys are ignored

        private bool Insert(ref Node p, string key, int i, T value)
        {
            if (p == null)
                p = new Node(key[i]);

            // Current character of key inserted in left subtree
            if (key[i] < p.ch)
                return Insert(ref p.low, key, i, value);

            // Current character of key inserted in right subtree
            else if (key[i] > p.ch)
                return Insert(ref p.high, key, i, value);

            else if (i + 1 == key.Length)
            // Key found
            {
                // But key/value pair already exists
                if (!p.value.Equals(default(T)))
                    return false;
                else
                {
                    // Place value in node
                    p.value = value;
                    size++;
                    return true;
                }
            }

            else
                // Next character of key inserted in middle subtree
                return Insert(ref p.middle, key, i + 1, value);
        }

        // Value
        // Returns the value associated with a key; otherwise default

        public T Value(string key)
        {
            int i = 0;
            Node p = root;

            while (p != null)
            {
                // Search for current character of the key in left subtree
                if (key[i] < p.ch)
                    p = p.low;

                // Search for current character of the key in right subtree           
                else if (key[i] > p.ch)
                    p = p.high;

                else // if (p.ch == key[i])
                {
                    // Return the value if all characters of the key have been visited 
                    if (++i == key.Length)
                        return p.value;

                    // Move to next character of the key in the middle subtree   
                    p = p.middle;
                }
            }
            return default(T);   // Key too long
        }

        // Contains
        // Returns true if the given key is found in the Trie; false otherwise

        public bool Contains(string key)
        {
            int i = 0;
            Node p = root;

            while (p != null)
            {
                // Search for current character of the key in left subtree
                if (key[i] < p.ch)
                    p = p.low;

                // Search for current character of the key in right subtree           
                else if (key[i] > p.ch)
                    p = p.high;

                else // if (p.ch == key[i])
                {
                    // Return true if the key is associated with a non-default value; false otherwise 
                    if (++i == key.Length)
                        return !p.value.Equals(default(T));

                    // Move to next character of the key in the middle subtree   
                    p = p.middle;
                }
            }
            return false;        // Key too long
        }

        // Partial Match
        // Returns list of keys that match the given pattern

        public List<string> PartialMatch(string pattern)
        {
            List<string> partial_list = new List<string>();
            Regex patt = new Regex(pattern);        // convert passed string to a regular expression pattern
            return PartialMatch(root, patt, "", partial_list);
        }

        private List<string> PartialMatch(Node p, Regex pattern, string key, List<string> partial_list)
        {
            bool match;
            if (p != null)      // if the current node is not null
            {
                PartialMatch(p.low, pattern, key, partial_list);        // traverse left side of tree
                if (!p.value.Equals(default(T)))
                {
                    match = pattern.IsMatch(key + p.ch);        // determine whether or not the key matches the pattern
                    if (match)      // if the key matches the pattern...
                    {
                        partial_list.Add(key + p.ch);       // add the key to the list
                    }
                }
                PartialMatch(p.middle, pattern, key + p.ch, partial_list);      // traverse middle of tree
                PartialMatch(p.high, pattern, key, partial_list);       // traverse right side of tree
                return partial_list;        // return the list of keys
            }
            else { return partial_list; }
        }

        // Autocomplete
        // Returns list of keys that begin with the given prefix

        public List<string> Autocomplete(string prefix)
        {
            List<string> auto_list = new List<string>();
            return Autocomplete(root, prefix, "", auto_list);
        }

        private List<string> Autocomplete(Node p, string prefix, string key, List<string> auto_list)
        {
            bool compare;
            if (p != null)      // if the current node is not null
            {
                Autocomplete(p.low, prefix, key, auto_list);        // traverse the left side of the tree
                if (!p.value.Equals(default(T)))
                {
                    compare = StringCompare(prefix, key + p.ch);        // determine whether or not the key has the prefix
                    if (compare)        // if the key has the prefix...
                    {
                        auto_list.Add(key + p.ch);      // add the key to the list
                    }
                }
                Autocomplete(p.middle, prefix, key + p.ch, auto_list);      // traverse the middle of the tree
                Autocomplete(p.high, prefix, key, auto_list);       // traverse the right side of the tree
                return auto_list;       // return the list of keys
            }
            else { return auto_list; }
        }

        // StringCompare
        // Compares each character of two string to see if they are the same (true if match, false otherwise)
        private bool StringCompare(string prefix, string key)
        {
            if (key.Length < prefix.Length)     // if the length of the key is less than the length of the prefix, we know the key cannot have the prefix
            {
                return false;
            }
            for (int i = 0; i < prefix.Length; i++)     // compare each character of the prefix to the first characters of the key
            {
                if(prefix[i] != key[i])       
                {
                    return false;       // not a matching prefix
                }
            }
            return true;        // matching prefix
        }

        // MakeEmpty
        // Creates an empty Trie
        // Time complexity:  O(1)

        public void MakeEmpty()
        {
            root = null;
        }

        // Empty
        // Returns true if the Trie is empty; false otherwise
        // Time complexity:  O(1)

        public bool Empty()
        {
            return root == null;
        }

        // Size
        // Returns the number of Trie values
        // Time complexity:  O(1)

        public int Size()
        {
            return size;
        }

        // Public Print
        // Calls private Print to carry out the actual printing

        public void Print()
        {
            Print(root, "");
        }

        // Private Print
        // Outputs the key/value pairs ordered by keys 

        private void Print(Node p, string key)
        {
            if (p != null)
            {
                Print(p.low, key);
                if (!p.value.Equals(default(T)))
                    Console.WriteLine(key + p.ch + " " + p.value);
                Print(p.middle, key + p.ch);
                Print(p.high, key);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string choice, key, str_value;
            bool conversion, success;
            int intchoice, value;

            Trie<int> T;
            T = new Trie<int>();

            while (!MyGlobals.exit_bool)
            {
                MyGlobals.error_bool = true;
                Console.WriteLine("******************************************************");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("*                   TERNARY TREES                    *");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("******************************************************");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("*    1 = CREATE A NEW TERNARY TREE                   *");
                Console.WriteLine("*    2 = INSERT KEY/VALUE                            *");
                Console.WriteLine("*    3 = FIND VALUE                                  *");
                Console.WriteLine("*    4 = SEE IF KEY EXISTS                           *");
                Console.WriteLine("*    5 = AUTOCOMPLETE FIND KEYS                      *");
                Console.WriteLine("*    6 = FIND PARTIAL KEY MATCHES                    *");
                Console.WriteLine("*    7 = GET SIZE OF TERNARY TREE                    *");
                Console.WriteLine("*    8 = PRINT TERNARY TREE                          *");
                Console.WriteLine("*    9 = EXIT PROGRAM                                *");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("******************************************************\n");

                do
                {
                    choice = Console.ReadLine();        // get user choice
                    conversion = Int32.TryParse(choice, out intchoice);
                    if (!conversion)        // if the user's entry is not an integer
                    {
                        Console.WriteLine("ERROR: Invalid option. Please try again.\n");
                    }
                    else
                    {
                        MyGlobals.error_bool = false;
                        break;
                    }
                    Console.WriteLine();
                } while (MyGlobals.error_bool);

                MyGlobals.error_bool = true;
                Console.WriteLine();

                switch (intchoice)
                {
                    case 1:     // Create new ternary tree
                        T.MakeEmpty();
                        Console.WriteLine("New Ternary Tree created successfully.\n");
                        break;
                    case 2:     // Insert new key/value pair
                        Console.WriteLine("What key would you like to insert?");
                        key = Console.ReadLine();
                        Console.WriteLine("What value would you like to insert?");
                        do
                        {
                            str_value = Console.ReadLine();
                            conversion = Int32.TryParse(str_value, out value);
                            if (!conversion)
                            {
                                Console.WriteLine("ERROR: Invalid option. Please try again.\n");
                            }
                            else
                            {
                                MyGlobals.error_bool = false;
                                break;
                            }
                        } while (MyGlobals.error_bool);
                        MyGlobals.error_bool = true;
                        Console.WriteLine();
                        success = T.Insert(key, value);
                        if (success)
                        {
                            Console.WriteLine("Successfully inserted " + key + " & " + value + " into the ternary tree.\n");
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Insertion unsuccessful (key/value pair already exists).\n");
                        }
                        break;
                    case 3:     // Find the value associated with a key
                        Console.WriteLine("Which key would you like to find?");
                        key = Console.ReadLine();
                        _ = T.Value(key);
                        Console.WriteLine("\n");
                        break;
                    case 4:     // Determine whether the tree contains a key or not
                        Console.WriteLine("What key would you like to insert?");
                        key = Console.ReadLine();
                        success = T.Contains(key);
                        if (success)
                        {
                            Console.WriteLine("Successfully found " + key + " in the ternary tree.\n");
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Key was not found.\n");
                        }
                        break;
                    case 5:     // Find keys that match a prefix
                        Console.WriteLine("What is the prefix you are looking for in keys?");
                        key = Console.ReadLine();
                        List<string> Autocompletes = T.Autocomplete(key);
                        Console.WriteLine("Keys that match prefix: ");
                        foreach (string element in Autocompletes)
                        {
                            Console.Write(element + " ");
                        }
                        Console.WriteLine("\n");
                        break;
                    case 6:     // Find keys that match a pattern
                        Console.WriteLine("What is the pattern you are looking for in keys?");
                        key = Console.ReadLine();
                        List<string> PartialMatches = T.PartialMatch(key);
                        Console.WriteLine("Keys that match pattern: ");
                        foreach (string element in PartialMatches)
                        {
                            Console.Write(element + " ");
                        }
                        Console.WriteLine("\n");
                        break;
                    case 7:     // Get the size of the current ternary tree
                        Console.WriteLine("Size of Ternary Tree: " + T.Size() + ".\n");
                        break;
                    case 8:     // Print ternary tree
                        T.Print();
                        break;
                    case 9:     // Exit program
                        MyGlobals.exit_bool = true;
                        Console.WriteLine("Exiting program...");
                        Environment.Exit(0);
                        break;
                    default:        // Invalid integer menu choice
                        Console.WriteLine("ERROR: Invalid option. Please try again.\n");
                        break;
                }
            }
        }
    }
}
