using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Web;

namespace Movie_Library_Assignment
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory() + "\\nlog.config";
            var logger = NLog.Web.NLogBuilder.ConfigureNLog(path).GetCurrentClassLogger();

            logger.Info("Program started");

            string choose;

            do {
                Console.WriteLine("Enter 1 to view movies");
                Console.WriteLine("Enter 2 to add data");
                Console.WriteLine("Enter any other key to exit");

                choose = Console.ReadLine();

                var file = "movies.csv";

                if (choose == "1") {
                    if (File.Exists(file))
                    {
                        StreamReader sr = new StreamReader(file);
                        while(!sr.EndOfStream) {
                            string line = sr.ReadLine();
                            string[] movie = line.Split(',');
                            movie = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                            Console.WriteLine($"{movie[0],-10}{movie[1],-150}{movie[2]}");
                        }
                        sr.Close();
                    }
                    else {
                        logger.Warn("File does not exists. {file}", file);
                    }
                }

                else if (choose == "2") {
                    bool addContinue = true;
                    int ID;
                    string title = "";
                    string genres = "";
                    string[] movieCheck;
                    List<int> IDNumber = new List<int>();
                    List<string> titleText = new List<string>();
                    

                    if (File.Exists(file))
                    {
                        StreamReader sr = new StreamReader(file);
                        while(!sr.EndOfStream) {
                            string line = sr.ReadLine();
                            movieCheck = line.Split(',');
                            movieCheck = Regex.Split(line, "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
                            int parseID;
                            if (int.TryParse(movieCheck[0], out parseID)) {
                                IDNumber.Add(Int32.Parse(movieCheck[0]));
                                titleText.Add(movieCheck[1]);
                            }
                            else {continue;}
                        }
                        sr.Close();
                    }
                    else {
                        logger.Warn("File does not exists. {file}", file);
                    }

                    do {
                        Console.WriteLine("Enter movie ID");
                        string IDString = Console.ReadLine();
                        if (!int.TryParse(IDString, out ID)) {
                            logger.Error("Invalid input (integer): {Answer}", IDString);
                            addContinue = false;
                        }
                        else if (ID <= IDNumber[IDNumber.Count - 1]){
                            logger.Warn("Number has to be larger that the final ID number: {number}", IDNumber[IDNumber.Count - 1]);
                            addContinue = false;
                        }
                        else {
                            addContinue = true;
                            do {
                                Console.WriteLine("Enter movie title");
                                string titleString = Console.ReadLine();
                                if (titleString == "") {
                                    logger.Error("No input for title was entered");
                                    addContinue = false;
                                }
                                else if (titleText.Contains(titleString)) {
                                    logger.Warn("The title {title} already exists", titleString);
                                    addContinue = false;
                                }
                                else {
                                    addContinue = true;
                                    if (titleString.Contains(",")) {
                                        title = "\"" + titleString + "\"";
                                    }
                                    else {
                                        title = titleString;
                                    }
                                    do {
                                        List<string> genreList = new List<string>();
                                        string genreContinue;
                                        do {
                                            Console.WriteLine("Enter movie genre");
                                            String genreAdd = Console.ReadLine();
                                            if (genreAdd == "") {
                                                logger.Error("No input for genre was entered");
                                                genreContinue = "Y";
                                            }
                                            else {
                                                genreList.Add(genreAdd);
                                                Console.WriteLine("Would you like to add another (Y/N)");
                                                genreContinue = Console.ReadLine().ToUpper();
                                            }
                                        } while (genreContinue == "Y");
                                        string lastGenre = genreList.LastOrDefault();
                                        foreach(string genre in genreList) {
                                            if (genre.Equals(lastGenre))
                                            {
                                                genres += genre;
                                            }
                                            else
                                            {
                                                genres += genre + "|";
                                            }
                                        }
                                    } while (addContinue == false);
                                }
                            } while (addContinue == false);
                        }
                    } while(addContinue == false);

                    string addLine = ID.ToString() + "," + title + "," + genres + "\n";

                    if (File.Exists(file)) {
                        File.AppendAllText(file, addLine);
                        logger.Info("Movie has been added: " + title);
                    }
                    else {
                        logger.Warn("File does not exists. {file}", file);
                    }
                } 
            } while(choose == "1" || choose == "2");

            logger.Info("Program Ended");
        }
    }
}