using System;
using System.IO;
using System.Linq;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class ScreenOutputService : IOutputService
    {
        private readonly IPlanetService _planetService;

        private readonly IMoonService _moonService;

        public ScreenOutputService(IPlanetService planetService, IMoonService moonService)
        {
            _planetService = planetService;
            _moonService = moonService;
        }

        public void OutputAllPlanetsAndTheirMoonsToConsole()
        {
            //The service gets all the planets from the API.
            var planets = _planetService.GetAllPlanets().Where(p => p.Moons.Any()).ToArray();

            //If the planets aren't found, then the function stops and tells that to the user via the console.
            if (!planets.Any())
            {
                Console.WriteLine(OutputString.NoPlanetsFound);
                return;
            }

            //The column sizes and labels for the planets are configured here. 
            var columnSizesForPlanets = new[] { 20, 20, 30, 20 };
            var columnLabelsForPlanets = new[]
            {
                OutputString.PlanetNumber, OutputString.PlanetId, OutputString.PlanetSemiMajorAxis,
                OutputString.TotalMoons
            };

            //The column sizes and labels for the moons are configured here.
            //The second moon's column needs the 2 extra '-' characters so that it's aligned with the planet's column.
            var columnSizesForMoons = new[] { 20, 70 + 2 };
            var columnLabelsForMoons = new[]
            {
                OutputString.MoonNumber, OutputString.MoonId
            };

            //The for loop creates the correct output.
            for (int i = 0, j = 1; i < planets.Length; i++, j++)
            {
                //First the line is created.
                ConsoleWriter.CreateLine(columnSizesForPlanets);

                //Under the line the header is created.
                ConsoleWriter.CreateText(columnLabelsForPlanets, columnSizesForPlanets);

                //Under the header the planet data is shown.
                ConsoleWriter.CreateText(
                    new[]
                    {
                        j.ToString(), CultureInfoUtility.TextInfo.ToTitleCase(planets[i].Id),
                        planets[i].SemiMajorAxis.ToString(),
                        planets[i].Moons.Count.ToString()
                    },
                    columnSizesForPlanets);

                //Under the planet data the header for the moons is created.
                ConsoleWriter.CreateLine(columnSizesForPlanets);
                ConsoleWriter.CreateText(columnLabelsForMoons, columnSizesForMoons);

                //The for loop creates the correct output.
                for (int k = 0, l = 1; k < planets[i].Moons.Count; k++, l++)
                {
                    ConsoleWriter.CreateText(
                        new[]
                        {
                            l.ToString(), CultureInfoUtility.TextInfo.ToTitleCase(planets[i].Moons.ElementAt(k).Id)
                        },
                        columnSizesForMoons);
                }

                //Under the data the footer is created.
                ConsoleWriter.CreateLine(columnSizesForMoons);
                ConsoleWriter.CreateEmptyLines(2);

                /*
                    This is an example of the output for the planet Earth:
                    --------------------+--------------------+------------------------------+--------------------
                    Planet's Number     |Planet's Id         |Planet's Semi-Major Axis      |Total Moons
                    10                  |Terre               |0                             |1
                    --------------------+--------------------+------------------------------+--------------------
                    Moon's Number       |Moon's Id
                    1                   |La Lune
                    --------------------+------------------------------------------------------------------------
                */
            }
        }

        public void OutputAllMoonsAndTheirMassToConsole()
        {
            //The function works the same way as the PrintAllPlanetsAndTheirMoonsToConsole function. You can find more comments there.
            var moons = _moonService.GetAllMoons().ToArray();
            
            if (!moons.Any())
            {
                Console.WriteLine(OutputString.NoMoonsFound);
                return;
            }

            var columnSizesForMoons = new[] { 20, 20, 30, 20 };
            var columnLabelsForMoons = new[]
            {
                OutputString.MoonNumber, OutputString.MoonId, OutputString.MoonMassExponent, OutputString.MoonMassValue
            };

            ConsoleWriter.CreateHeader(columnLabelsForMoons, columnSizesForMoons);

            for (int i = 0, j = 1; i < moons.Length; i++, j++)
            {
                ConsoleWriter.CreateText(
                    new[]
                    {
                        j.ToString(), CultureInfoUtility.TextInfo.ToTitleCase(moons[i].Id),
                        moons[i].MassExponent.ToString(), moons[i].MassValue.ToString()
                    },
                    columnSizesForMoons);
            }

            ConsoleWriter.CreateLine(columnSizesForMoons);
            ConsoleWriter.CreateEmptyLines(2);
            
            /*
                This is an example of the output for the moon around the earth:
                --------------------+--------------------+------------------------------+--------------------
                Moon's Number       |Moon's Id           |Moon's Mass Exponent          |Moon's Mass Value
                --------------------+--------------------+------------------------------+--------------------
                1                   |Lune                |22                            |7,346             
                ...more data...
                --------------------+--------------------+------------------------------+--------------------
             */
        }


        public void OutputAllPlanetsAndTheirAverageMoonTemperatureToConsole()
        {
            var planets = _planetService.GetAllPlanets().ToArray();
            if (!planets.Any())
            {
                Console.WriteLine(OutputString.NoPlanetsFound);
                return;
            }

            var columnSizes = new[] { 20, 30, 30 };
            var columnLabels = new[]
            {
                OutputString.PlanetNumber, OutputString.PlanetId, OutputString.AverageMoonTemperature
            };

            ConsoleWriter.CreateHeader(columnLabels, columnSizes);

            for (int i = 0, j = 1; i < planets.Length; i++, j++)
            {
                var moons = planets[i].Moons;
                var averageTemperature = moons.Any() && moons.Any(moon => moon.Temperature.HasValue)
                    ? moons.Where(moon => moon.Temperature.HasValue).Average(moon => moon.Temperature.Value).ToString("F2")
                    : "-";

                ConsoleWriter.CreateText(new[] { j.ToString(), planets[i].Id, averageTemperature }, columnSizes);
            }

            ConsoleWriter.CreateLine(columnSizes);
            ConsoleWriter.CreateEmptyLines(2);

            /*
                --------------------+--------------------------------------------------
                Planet's Id     |Planet's Average Moon Temprature
                --------------------+--------------------------------------------------
                1                   |0.00
                --------------------+--------------------------------------------------
            */
        }

        public void OutputCombinedDataToConsoleAndFile()
        {
            // File to store the combined output
            string filePath = "output.txt";

            // Clear previous content
            File.WriteAllText(filePath, "");

            // Capture console output
            using (var fileWriter = new StreamWriter(filePath, true))
            using (var consoleWriter = new StringWriter())
            {
                var originalConsoleOut = Console.Out;

                try
                {
                    // Redirect console output to both file and a string writer
                    Console.SetOut(consoleWriter);

                    // Call all methods — their output will go to consoleWriter
                    OutputAllPlanetsAndTheirMoonsToConsole();
                    OutputAllMoonsAndTheirMassToConsole();
                    OutputAllPlanetsAndTheirAverageMoonTemperatureToConsole();

                    // Write everything to file
                    fileWriter.Write(consoleWriter.ToString());
                }
                finally
                {
                    // Restore console output
                    Console.SetOut(originalConsoleOut);
                }
            }

            // Confirm that data is saved
            Console.WriteLine("Data has been combined and saved to output.txt");
        }

    }
}
